// Standard namespaces for basic functionality, diagnostics, interop services, and Windows Forms.
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
// Library for capturing global keyboard and mouse events.
using Gma.System.MouseKeyHook;

namespace AutoClicker
{
    // Partial class for the main form of the auto clicker.
    public partial class Form1 : Form
    {
        // Global hook for capturing key and mouse events (for setting inputs).
        private IKeyboardMouseEvents globalHook;
        // Separate hook for listening to the designated hotkey globally.
        private IKeyboardMouseEvents hotkeyHook;
        // Stores the key or mouse input selected for spamming.
        private string detectedInput;
        // Stores the key or mouse input selected as the hotkey to toggle spamming.
        private string detectedInputHotkey;
        // Reference to the currently active button (either settings or hotkey) for capturing input.
        private Button targetButton;

        // Event handler references for proper unsubscription later.
        private KeyEventHandler keyDownHandler;
        private MouseEventHandler mouseDownHandler;
        private MouseEventHandler mouseWheelHandler;

        // Event handlers specifically for the hotkey listener.
        private KeyEventHandler hotkeyListener;
        private MouseEventHandler hotkeymouseDownHandler;
        private MouseEventHandler hotkeymouseWheelHandler;

        // Flag to indicate whether we are currently in the process of setting a hotkey.
        private bool isSettingHotkey = false;
        // Timer used to trigger spam events at specified intervals.
        private System.Windows.Forms.Timer spamTimer;
        // Boolean flag to indicate if spamming is currently active.
        private bool isSpamming = false;
        // Counter to track how many times the spam action has been performed.
        private int counter = 0;

        // Constructor for the Form1 class.
        public Form1()
        {
            InitializeComponent();  // Standard initialization of form components.
            StartHotkeyListener();  // Begin listening for the hotkey as soon as the program launches.

            // Initialize the spam timer and attach the Tick event handler.
            spamTimer = new System.Windows.Forms.Timer();
            spamTimer.Tick += SpamTimer_Tick;

            // Set default hotkey to F6 and update the hotkey button text.
            detectedInputHotkey = "F6";
            hotkeyButton.Text = detectedInputHotkey;

            // Disable the stop button initially.
            btnStop.Enabled = false;
        }

        /// <summary>
        /// Stops the spamming when the Stop button is clicked.
        /// </summary>
        private void btnStop_Click(object sender, EventArgs e)
        {
            // Set flag to indicate spamming should stop.
            isSpamming = true;
            StartStopSpamming(); // Toggle spamming state.
            btnStop.Enabled = false; // Disable Stop button.
            btnStart.Enabled = true; // Enable Start button.
        }

        /// <summary>
        /// Starts the spamming when the Start button is clicked.
        /// </summary>
        private void btnStart_Click(object sender, EventArgs e)
        {
            // Set flag to indicate spamming should start.
            isSpamming = false;
            StartStopSpamming(); // Toggle spamming state.
            int interval = GetSpamInterval();
            // If a key is selected and the interval is valid, disable the start button and enable the stop button.
            if (settingsButton1.Text != "empty" && interval != 0)
            {
                btnStart.Enabled = false;
                btnStop.Enabled = true;
            }
        }

        // Below are stub event handlers for various UI controls.
        private void hoursLabel_Click(object sender, EventArgs e) { }
        private void hoursTBox_TextChanged(object sender, EventArgs e) { }
        private void minutesTBox_TextChanged(object sender, EventArgs e) { }
        private void settingLabel1_Click(object sender, EventArgs e) { }
        private void millisTBox_TextChanged(object sender, EventArgs e) { }
        private void secondsTBox_TextChanged(object sender, EventArgs e) { }

        /// <summary>
        /// Initiates listening for key or mouse input when the settings button is clicked.
        /// </summary>
        private void settingsButton1_Click(object sender, EventArgs e)
        {
            // Pass the button instance to the universal listening method.
            StartListening((Button)sender);
        }

        /// <summary>
        /// Initiates listening for key or mouse input when the hotkey button is clicked.
        /// </summary>
        private void hotkeyButton_Click(object sender, EventArgs e)
        {
            // Pass the button instance to the universal listening method.
            StartListening((Button)sender);
        }

        /// <summary>
        /// Sets up a global hook to capture the next key or mouse event for input selection.
        /// </summary>
        private void StartListening(Button button)
        {
            // Clean up any existing listeners to ensure only one is active at a time.
            CleanupInputListeners();

            // Set the target button to update its text based on captured input.
            targetButton = button;
            targetButton.Text = "Press any key or button";

            // If the hotkey button is active, set the flag so the hook knows to capture a hotkey.
            if (targetButton.Name == "hotkeyButton")
            {
                isSettingHotkey = true;
            }

            // Create a global hook for keyboard and mouse events.
            globalHook = Hook.GlobalEvents();
            // Assign event handlers for key down, mouse button down, and mouse wheel events.
            keyDownHandler = (sender, e) => GlobalHook_KeyDown(sender, e, targetButton);
            mouseDownHandler = (sender, e) => GlobalHook_MouseDown(sender, e, targetButton);
            mouseWheelHandler = (sender, e) => GlobalHook_MouseWheel(sender, e, targetButton);

            // Subscribe the event handlers to the global hook.
            globalHook.KeyDown += keyDownHandler;
            globalHook.MouseDown += mouseDownHandler;
            globalHook.MouseWheel += mouseWheelHandler;

            Debug.WriteLine("Listening for key/mouse input...");
        }

        /// <summary>
        /// Handles global key down events and updates the target button with the selected key.
        /// </summary>
        private void GlobalHook_KeyDown(object sender, KeyEventArgs e, Button targetButton)
        {
            // Suppress Space or Enter keys to prevent accidental activation.
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }

            // If the settings button is used, update the detected input.
            if (targetButton.Name == "settingsButton1")
            {
                detectedInput = e.KeyCode.ToString();
                targetButton.Text = "Key: " + detectedInput;
            }
            // Otherwise, update the hotkey input.
            else if (targetButton.Name == "hotkeyButton")
            {
                detectedInputHotkey = e.KeyCode.ToString();
                targetButton.Text = "Key: " + detectedInputHotkey;
            }

            Debug.WriteLine($"Stored Input: {detectedInput}, Hotkey: {detectedInputHotkey}");
            // Remove the global hook after capturing input.
            CleanupInputListeners();
        }

        /// <summary>
        /// Handles global mouse button down events and updates the target button with the selected mouse button.
        /// </summary>
        private void GlobalHook_MouseDown(object sender, MouseEventArgs e, Button targetButton)
        {
            // For settings button, determine which mouse button was pressed.
            if (targetButton.Name == "settingsButton1")
            {
                detectedInput = e.Button.ToString();
                // Rename standard buttons for clarity.
                if (detectedInput == "Left")
                {
                    detectedInput = "LeftClick";
                }
                else if (detectedInput == "Right")
                {
                    detectedInput = "RightClick";
                }
                targetButton.Text = "Mouse: " + detectedInput;
            }
            // For hotkey button, store the mouse button pressed.
            else if (targetButton.Name == "hotkeyButton")
            {
                detectedInputHotkey = e.Button.ToString();
                targetButton.Text = "Mouse: " + detectedInputHotkey;
            }

            Debug.WriteLine($"Stored Input: {detectedInput}, Hotkey: {detectedInputHotkey}");
            // Clean up the hook after capturing the input.
            CleanupInputListeners();
        }

        /// <summary>
        /// Handles global mouse wheel events and updates the target button with the wheel direction.
        /// </summary>
        private void GlobalHook_MouseWheel(object sender, MouseEventArgs e, Button targetButton)
        {
            // Determine if the wheel was scrolled up or down.
            if (e.Delta > 0)
            {
                if (targetButton.Name == "settingsButton1")
                {
                    detectedInput = "MouseWheelUp";
                    targetButton.Text = "Mouse: " + detectedInput;
                }
                else if (targetButton.Name == "hotkeyButton")
                {
                    detectedInputHotkey = "MouseWheelUp";
                    targetButton.Text = "Mouse: " + detectedInputHotkey;
                }
            }
            else
            {
                if (targetButton.Name == "settingsButton1")
                {
                    detectedInput = "MouseWheelDown";
                    targetButton.Text = "Mouse: " + detectedInput;
                }
                else if (targetButton.Name == "hotkeyButton")
                {
                    detectedInputHotkey = "MouseWheelDown";
                    targetButton.Text = "Mouse: " + detectedInputHotkey;
                }
            }

            Debug.WriteLine($"Stored Input: {detectedInput}, Hotkey: {detectedInputHotkey}");
            // Remove the hook after input is captured.
            CleanupInputListeners();
        }

        /// <summary>
        /// Sets up a global hook to listen for the designated hotkey input even when the form is not focused.
        /// </summary>
        private void StartHotkeyListener()
        {
            // Ensure only one hotkey hook is created.
            if (hotkeyHook == null)
            {
                Debug.WriteLine("Starting hotkey listener...");
                hotkeyHook = Hook.GlobalEvents();

                // Listen for key down events for the hotkey.
                hotkeyListener = (sender, e) =>
                {
                    // If currently setting the hotkey, ignore these events.
                    if (isSettingHotkey)
                    {
                        isSettingHotkey = false;
                        return;
                    }

                    // If the pressed key matches the designated hotkey, toggle spamming.
                    if (!string.IsNullOrEmpty(detectedInputHotkey) && e.KeyCode.ToString() == detectedInputHotkey)
                    {
                        StartStopSpamming();
                    }
                };

                // Listen for mouse button events for the hotkey.
                hotkeymouseDownHandler = (sender, e) =>
                {
                    if (isSettingHotkey)
                    {
                        isSettingHotkey = false;
                        return;
                    }
                    if (!string.IsNullOrEmpty(detectedInputHotkey) && e.Button.ToString() == detectedInputHotkey)
                    {
                        StartStopSpamming();
                    }
                };

                // Listen for mouse wheel events for the hotkey.
                hotkeymouseWheelHandler = (sender, e) =>
                {
                    if (isSettingHotkey)
                    {
                        isSettingHotkey = false;
                        return;
                    }
                    // Determine the wheel direction.
                    string wheelDirection = e.Delta > 0 ? "MouseWheelUp" : "MouseWheelDown";
                    if (!string.IsNullOrEmpty(detectedInputHotkey) && detectedInputHotkey == wheelDirection)
                    {
                        StartStopSpamming();
                    }
                };

                // Subscribe the hotkey event handlers to the hotkey hook.
                hotkeyHook.KeyDown += hotkeyListener;
                hotkeyHook.MouseDown += hotkeymouseDownHandler;
                hotkeyHook.MouseWheel += hotkeymouseWheelHandler;
            }
        }

        /// <summary>
        /// Unsubscribes and disposes of the global hook used for input selection.
        /// </summary>
        private void CleanupInputListeners()
        {
            if (globalHook != null)
            {
                globalHook.KeyDown -= keyDownHandler;
                globalHook.MouseDown -= mouseDownHandler;
                globalHook.MouseWheel -= mouseWheelHandler;
                globalHook.Dispose();
                globalHook = null;
            }
        }

        /// <summary>
        /// Ensures hooks are disposed when the form is closing.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            CleanupHotkeyListener();
            CleanupInputListeners();
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Unsubscribes and disposes of the hotkey hook.
        /// </summary>
        private void CleanupHotkeyListener()
        {
            if (hotkeyHook != null)
            {
                hotkeyHook.KeyDown -= hotkeyListener;
                hotkeyHook.MouseDown -= hotkeymouseDownHandler;
                hotkeyHook.MouseWheel -= hotkeymouseWheelHandler;
                hotkeyHook.Dispose();
                hotkeyHook = null;
            }
        }

        /// <summary>
        /// Calculates the spam interval based on user input from hours, minutes, seconds, and milliseconds text boxes.
        /// </summary>
        private int GetSpamInterval()
        {
            int hours, minutes, seconds, milliseconds;

            // Parse each textbox input and default to 0 if parsing fails.
            if (!int.TryParse(hoursTBox.Text, out hours))
            {
                hours = 0;
            }
            if (!int.TryParse(minutesTBox.Text, out minutes))
            {
                minutes = 0;
            }
            if (!int.TryParse(secondsTBox.Text, out seconds))
            {
                seconds = 0;
            }
            if (!int.TryParse(millisTBox.Text, out milliseconds))
            {
                milliseconds = 0;
            }

            // Convert the total time to milliseconds.
            return (hours * 3600000) + (minutes * 60000) + (seconds * 1000) + milliseconds;
        }

        /// <summary>
        /// Starts or stops the spamming process based on the current state.
        /// </summary>
        private void StartStopSpamming()
        {
            if (isSpamming)
            {
                // If spamming is active, stop the timer and reset counter.
                spamTimer.Stop();
                isSpamming = false;
                Debug.WriteLine("Stopped spamming.");
                counter = 0;
            }
            else
            {
                int interval = GetSpamInterval();
                // Ensure a valid interval is entered.
                if (interval <= 0)
                {
                    MessageBox.Show("Please enter a valid time interval!", "Invalid Interval", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // Ensure a key/mouse input is selected.
                if (string.IsNullOrEmpty(detectedInput))
                {
                    MessageBox.Show("Please select a key to spam first!", "No Key Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // Set the timer interval and start spamming.
                spamTimer.Interval = interval;
                spamTimer.Start();
                isSpamming = true;
                Debug.WriteLine($"Started spamming {detectedInput} every {interval}ms.");
            }
        }

        /// <summary>
        /// Timer event handler that performs the spam action at each tick.
        /// </summary>
        private void SpamTimer_Tick(object sender, EventArgs e)
        {
            // If a maximum click count is enabled...
            if (maxClickBox.Checked)
            {
                if (counter < numericUpDown1.Value)
                {
                    counter++;
                    ExecuteKeyOrMouseAction(detectedInput);
                }
                else
                {
                    // Stop spamming once the maximum count is reached.
                    StartStopSpamming();
                }
            }
            // If infinite clicking is enabled, execute without count limitation.
            else if (infiniteClickBox.Checked)
            {
                ExecuteKeyOrMouseAction(detectedInput);
            }
        }

        // --- Revised Input Simulation using SendInput ---

        /// <summary>
        /// Determines the type of input (keyboard or mouse) and sends the appropriate simulated input.
        /// </summary>
        private void ExecuteKeyOrMouseAction(string input)
        {
            if (string.IsNullOrEmpty(input))
                return;

            // Use a switch-case to handle mouse actions and default to keyboard keys.
            switch (input)
            {
                case "LeftClick":
                    SendMouseClick(MOUSEEVENTF_LEFTDOWN, MOUSEEVENTF_LEFTUP);
                    break;
                case "RightClick":
                    SendMouseClick(MOUSEEVENTF_RIGHTDOWN, MOUSEEVENTF_RIGHTUP);
                    break;
                case "Middle":
                    SendMouseClick(MOUSEEVENTF_MIDDLEDOWN, MOUSEEVENTF_MIDDLEUP);
                    break;
                case "MouseWheelUp":
                    SendMouseWheel(true);
                    break;
                case "MouseWheelDown":
                    SendMouseWheel(false);
                    break;
                default:
                    // For keyboard keys, attempt to parse the input into a Keys enum.
                    try
                    {
                        Keys key = (Keys)Enum.Parse(typeof(Keys), input);
                        ushort vk = (ushort)key;
                        SendKey(vk);
                    }
                    catch
                    {
                        Debug.WriteLine("Unknown key: " + input);
                    }
                    break;
            }
        }

        /// <summary>
        /// Simulates a keyboard key press (both down and up events) using the SendInput Win32 API.
        /// </summary>
        private void SendKey(ushort keyCode)
        {
            // Prepare an array with two INPUT structures: one for key down, one for key up.
            INPUT[] inputs = new INPUT[2];

            // Configure key-down event.
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].u.ki.wVk = keyCode;
            inputs[0].u.ki.wScan = 0;
            inputs[0].u.ki.dwFlags = 0; // 0 for key press down.
            inputs[0].u.ki.time = 0;
            inputs[0].u.ki.dwExtraInfo = IntPtr.Zero;

            // Configure key-up event.
            inputs[1].type = INPUT_KEYBOARD;
            inputs[1].u.ki.wVk = keyCode;
            inputs[1].u.ki.wScan = 0;
            inputs[1].u.ki.dwFlags = KEYEVENTF_KEYUP; // KEYEVENTF_KEYUP flag for key release.
            inputs[1].u.ki.time = 0;
            inputs[1].u.ki.dwExtraInfo = IntPtr.Zero;

            // Send the key press events.
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        /// <summary>
        /// Simulates a mouse click by sending a button down and up event using the SendInput Win32 API.
        /// </summary>
        private void SendMouseClick(uint downFlag, uint upFlag)
        {
            // Prepare an array with two INPUT structures for mouse button down and up.
            INPUT[] inputs = new INPUT[2];

            // Configure mouse button down event.
            inputs[0].type = INPUT_MOUSE;
            inputs[0].u.mi.dx = 0;
            inputs[0].u.mi.dy = 0;
            inputs[0].u.mi.mouseData = 0;
            inputs[0].u.mi.dwFlags = downFlag;
            inputs[0].u.mi.time = 0;
            inputs[0].u.mi.dwExtraInfo = IntPtr.Zero;

            // Configure mouse button up event.
            inputs[1].type = INPUT_MOUSE;
            inputs[1].u.mi.dx = 0;
            inputs[1].u.mi.dy = 0;
            inputs[1].u.mi.mouseData = 0;
            inputs[1].u.mi.dwFlags = upFlag;
            inputs[1].u.mi.time = 0;
            inputs[1].u.mi.dwExtraInfo = IntPtr.Zero;

            // Send the mouse click events.
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        /// <summary>
        /// Simulates a mouse wheel event using the SendInput Win32 API.
        /// </summary>
        private void SendMouseWheel(bool wheelUp)
        {
            const int WHEEL_DELTA = 120; // Standard delta value for one notch of the mouse wheel.
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = INPUT_MOUSE;
            inputs[0].u.mi.dx = 0;
            inputs[0].u.mi.dy = 0;
            // Set the mouseData field based on wheel direction.
            inputs[0].u.mi.mouseData = (uint)(wheelUp ? WHEEL_DELTA : -WHEEL_DELTA);
            inputs[0].u.mi.dwFlags = MOUSEEVENTF_WHEEL;
            inputs[0].u.mi.time = 0;
            inputs[0].u.mi.dwExtraInfo = IntPtr.Zero;
            // Send the mouse wheel event.
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        // --- Definitions and Imports for SendInput ---

        // Input type constants.
        private const uint INPUT_MOUSE = 0;
        private const uint INPUT_KEYBOARD = 1;
        private const uint INPUT_HARDWARE = 2;

        // Keyboard event flags.
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_SCANCODE = 0x0008;

        // Mouse event flags.
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const uint MOUSEEVENTF_WHEEL = 0x0800;

        // Structure for SendInput API.
        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;   // Specifies the type of input (mouse, keyboard, or hardware).
            public InputUnion u; // Union holding the specific input data.
        }

        // Union for different input types.
        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        // Structure for mouse input.
        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;           // Mouse x-coordinate movement.
            public int dy;           // Mouse y-coordinate movement.
            public uint mouseData;   // Additional data (wheel delta, etc.).
            public uint dwFlags;     // Flags to specify various aspects of mouse motion and button clicks.
            public uint time;        // Timestamp for the event.
            public IntPtr dwExtraInfo; // Additional application-defined information.
        }

        // Structure for keyboard input.
        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;       // Virtual key code.
            public ushort wScan;     // Hardware scan code.
            public uint dwFlags;     // Flags specifying various aspects of the key press.
            public uint time;        // Timestamp for the event.
            public IntPtr dwExtraInfo; // Additional application-defined information.
        }

        // Structure for hardware input.
        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        // Import the SendInput function from user32.dll.
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        /// <summary>
        /// When infinite clicking is enabled, unchecks the maximum click count checkbox.
        /// </summary>
        private void infiniteClickBox_CheckedChanged(object sender, EventArgs e)
        {
            if (infiniteClickBox.Checked)
            {
                maxClickBox.Checked = false;
            }
        }

        /// <summary>
        /// When maximum click count is enabled, unchecks the infinite clicking checkbox.
        /// </summary>
        private void maxClickBox_CheckedChanged(object sender, EventArgs e)
        {
            if (maxClickBox.Checked)
            {
                infiniteClickBox.Checked = false;
            }
        }

        // Handler for numericUpDown control value changes (if needed).
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            // No implementation needed here unless additional functionality is desired.
        }
    }
}
