using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace AutoClicker
{
    public partial class Form1 : Form
    {
        private IKeyboardMouseEvents globalHook;
        private IKeyboardMouseEvents hotkeyHook;
        private string detectedInput;
        private string detectedInputHotkey;
        private Button targetButton;

        // Store event handler references for proper unsubscription
        private KeyEventHandler keyDownHandler;
        private MouseEventHandler mouseDownHandler;
        private MouseEventHandler mouseWheelHandler;

        private KeyEventHandler hotkeyListener;
        private MouseEventHandler hotkeymouseDownHandler;
        private MouseEventHandler hotkeymouseWheelHandler;

        private bool isSettingHotkey = false;
        private System.Windows.Forms.Timer spamTimer;
        private bool isSpamming = false;
        private int counter = 0;

        public Form1()
        {
            InitializeComponent();
            StartHotkeyListener(); // Start listening for hotkey when the program launches

            spamTimer = new System.Windows.Forms.Timer();
            spamTimer.Tick += SpamTimer_Tick;

            detectedInputHotkey = "F6";
            hotkeyButton.Text = detectedInputHotkey;

            btnStop.Enabled = false;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            isSpamming = true;
            StartStopSpamming();
            btnStop.Enabled = false;
            btnStart.Enabled = true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            isSpamming = false;
            StartStopSpamming();
            int interval = GetSpamInterval();
            if (settingsButton1.Text != "empty" && interval != 0)
            {
                btnStart.Enabled = false;
                btnStop.Enabled = true;
            }
        }

        // (Other UI event handlers remain unchanged)
        private void hoursLabel_Click(object sender, EventArgs e) { }
        private void hoursTBox_TextChanged(object sender, EventArgs e) { }
        private void minutesTBox_TextChanged(object sender, EventArgs e) { }
        private void settingLabel1_Click(object sender, EventArgs e) { }
        private void millisTBox_TextChanged(object sender, EventArgs e) { }
        private void secondsTBox_TextChanged(object sender, EventArgs e) { }

        // Start listening for key/mouse selection on settingsButton1
        private void settingsButton1_Click(object sender, EventArgs e)
        {
            StartListening((Button)sender);
        }

        // Start listening for key/mouse selection on hotkeyButton
        private void hotkeyButton_Click(object sender, EventArgs e)
        {
            StartListening((Button)sender);
        }

        // Universal function to start listening for key/mouse input
        private void StartListening(Button button)
        {
            CleanupInputListeners(); // Ensure only one hook is active

            targetButton = button;
            targetButton.Text = "Press any key or button";

            if (targetButton.Name == "hotkeyButton")
            {
                isSettingHotkey = true;
            }

            globalHook = Hook.GlobalEvents();
            keyDownHandler = (sender, e) => GlobalHook_KeyDown(sender, e, targetButton);
            mouseDownHandler = (sender, e) => GlobalHook_MouseDown(sender, e, targetButton);
            mouseWheelHandler = (sender, e) => GlobalHook_MouseWheel(sender, e, targetButton);

            globalHook.KeyDown += keyDownHandler;
            globalHook.MouseDown += mouseDownHandler;
            globalHook.MouseWheel += mouseWheelHandler;

            Debug.WriteLine("Listening for key/mouse input...");
        }

        // Global Key Detection (For Settings & Hotkey)
        private void GlobalHook_KeyDown(object sender, KeyEventArgs e, Button targetButton)
        {
            // Prevent accidental click triggers
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }

            if (targetButton.Name == "settingsButton1")
            {
                detectedInput = e.KeyCode.ToString();
                targetButton.Text = "Key: " + detectedInput;
            }
            else if (targetButton.Name == "hotkeyButton")
            {
                detectedInputHotkey = e.KeyCode.ToString();
                targetButton.Text = "Key: " + detectedInputHotkey;
            }

            Debug.WriteLine($"Stored Input: {detectedInput}, Hotkey: {detectedInputHotkey}");
            CleanupInputListeners();
        }

        // Global Mouse Button Detection
        private void GlobalHook_MouseDown(object sender, MouseEventArgs e, Button targetButton)
        {
            if (targetButton.Name == "settingsButton1")
            {
                detectedInput = e.Button.ToString();
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
            else if (targetButton.Name == "hotkeyButton")
            {
                detectedInputHotkey = e.Button.ToString();
                targetButton.Text = "Mouse: " + detectedInputHotkey;
            }

            Debug.WriteLine($"Stored Input: {detectedInput}, Hotkey: {detectedInputHotkey}");
            CleanupInputListeners();
        }

        // Global Mouse Wheel Detection
        private void GlobalHook_MouseWheel(object sender, MouseEventArgs e, Button targetButton)
        {
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
            CleanupInputListeners();
        }

        // Start Listening for the Hotkey Globally
        private void StartHotkeyListener()
        {
            if (hotkeyHook == null)
            {
                Debug.WriteLine("Starting hotkey listener...");
                hotkeyHook = Hook.GlobalEvents();

                hotkeyListener = (sender, e) =>
                {
                    if (isSettingHotkey)
                    {
                        isSettingHotkey = false;
                        return;
                    }

                    if (!string.IsNullOrEmpty(detectedInputHotkey) && e.KeyCode.ToString() == detectedInputHotkey)
                    {
                        StartStopSpamming();
                    }
                };

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

                hotkeymouseWheelHandler = (sender, e) =>
                {
                    if (isSettingHotkey)
                    {
                        isSettingHotkey = false;
                        return;
                    }
                    string wheelDirection = e.Delta > 0 ? "MouseWheelUp" : "MouseWheelDown";
                    if (!string.IsNullOrEmpty(detectedInputHotkey) && detectedInputHotkey == wheelDirection)
                    {
                        StartStopSpamming();
                    }
                };

                hotkeyHook.KeyDown += hotkeyListener;
                hotkeyHook.MouseDown += hotkeymouseDownHandler;
                hotkeyHook.MouseWheel += hotkeymouseWheelHandler;
            }
        }

        // Cleanup input listeners to prevent memory leaks
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

        // Cleanup the hotkey listener on program close
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            CleanupHotkeyListener();
            CleanupInputListeners();
            base.OnFormClosing(e);
        }

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

        private int GetSpamInterval()
        {
            int hours, minutes, seconds, milliseconds;

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

            return (hours * 3600000) + (minutes * 60000) + (seconds * 1000) + milliseconds;
        }

        private void StartStopSpamming()
        {
            if (isSpamming)
            {
                spamTimer.Stop();
                isSpamming = false;
                Debug.WriteLine("Stopped spamming.");
                counter = 0;
            }
            else
            {
                int interval = GetSpamInterval();
                if (interval <= 0)
                {
                    MessageBox.Show("Please enter a valid time interval!", "Invalid Interval", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(detectedInput))
                {
                    MessageBox.Show("Please select a key to spam first!", "No Key Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                spamTimer.Interval = interval;
                spamTimer.Start();
                isSpamming = true;
                Debug.WriteLine($"Started spamming {detectedInput} every {interval}ms.");
            }
        }

        private void SpamTimer_Tick(object sender, EventArgs e)
        {
            if (maxClickBox.Checked)
            {
                if (counter < numericUpDown1.Value)
                {
                    counter++;
                    ExecuteKeyOrMouseAction(detectedInput);
                }
                else
                {
                    StartStopSpamming(); // Stop when max count is reached
                }
            }
            else if (infiniteClickBox.Checked)
            {
                ExecuteKeyOrMouseAction(detectedInput);
            }
        }

        // --- Revised Input Simulation using SendInput ---

        // This method examines the stored input and sends it via SendInput
        private void ExecuteKeyOrMouseAction(string input)
        {
            if (string.IsNullOrEmpty(input))
                return;

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
                    // Assume a keyboard key – attempt to parse the string to a Keys enum
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

        // Sends a keyboard key press (down and up) using SendInput
        private void SendKey(ushort keyCode)
        {
            INPUT[] inputs = new INPUT[2];

            // Key down
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].u.ki.wVk = keyCode;
            inputs[0].u.ki.wScan = 0;
            inputs[0].u.ki.dwFlags = 0; // key down
            inputs[0].u.ki.time = 0;
            inputs[0].u.ki.dwExtraInfo = IntPtr.Zero;

            // Key up
            inputs[1].type = INPUT_KEYBOARD;
            inputs[1].u.ki.wVk = keyCode;
            inputs[1].u.ki.wScan = 0;
            inputs[1].u.ki.dwFlags = KEYEVENTF_KEYUP;
            inputs[1].u.ki.time = 0;
            inputs[1].u.ki.dwExtraInfo = IntPtr.Zero;

            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        // Sends a mouse click by issuing a button down and up event via SendInput
        private void SendMouseClick(uint downFlag, uint upFlag)
        {
            INPUT[] inputs = new INPUT[2];

            // Button down
            inputs[0].type = INPUT_MOUSE;
            inputs[0].u.mi.dx = 0;
            inputs[0].u.mi.dy = 0;
            inputs[0].u.mi.mouseData = 0;
            inputs[0].u.mi.dwFlags = downFlag;
            inputs[0].u.mi.time = 0;
            inputs[0].u.mi.dwExtraInfo = IntPtr.Zero;

            // Button up
            inputs[1].type = INPUT_MOUSE;
            inputs[1].u.mi.dx = 0;
            inputs[1].u.mi.dy = 0;
            inputs[1].u.mi.mouseData = 0;
            inputs[1].u.mi.dwFlags = upFlag;
            inputs[1].u.mi.time = 0;
            inputs[1].u.mi.dwExtraInfo = IntPtr.Zero;

            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        // Sends a mouse wheel event via SendInput.
        private void SendMouseWheel(bool wheelUp)
        {
            const int WHEEL_DELTA = 120;
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = INPUT_MOUSE;
            inputs[0].u.mi.dx = 0;
            inputs[0].u.mi.dy = 0;
            inputs[0].u.mi.mouseData = (uint)(wheelUp ? WHEEL_DELTA : -WHEEL_DELTA);
            inputs[0].u.mi.dwFlags = MOUSEEVENTF_WHEEL;
            inputs[0].u.mi.time = 0;
            inputs[0].u.mi.dwExtraInfo = IntPtr.Zero;
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        // --- Definitions and Imports for SendInput ---

        private const uint INPUT_MOUSE = 0;
        private const uint INPUT_KEYBOARD = 1;
        private const uint INPUT_HARDWARE = 2;

        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_SCANCODE = 0x0008;

        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const uint MOUSEEVENTF_WHEEL = 0x0800;

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public InputUnion u;
        }

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

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        private void infiniteClickBox_CheckedChanged(object sender, EventArgs e)
        {
            if (infiniteClickBox.Checked)
            {
                maxClickBox.Checked = false;
            }
        }

        private void maxClickBox_CheckedChanged(object sender, EventArgs e)
        {
            if (maxClickBox.Checked)
            {
                infiniteClickBox.Checked = false;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
        }
    }
}
