using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Gma.System.MouseKeyHook; // Needed for global hooks

namespace AutoClicker
{
    public partial class Form1 : Form
    {
        private IKeyboardMouseEvents globalHook;
        private IKeyboardMouseEvents hotkeyHook; // Separate hook for hotkey tracking
        private string detectedInput;
        private string detectedInputHotkey;
        private Button targetButton; // Store the selected button dynamically

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

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        // Mouse Event Constants
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const uint MOUSEEVENTF_XDOWN = 0x0080;
        private const uint MOUSEEVENTF_XUP = 0x0100;

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

            // If setting the hotkey, activate the flag
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
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Prevents accidental button click
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
                    targetButton.Text = "Key: " + detectedInputHotkey;
                }
            }

            Debug.WriteLine($"Stored Input: {detectedInput}, Hotkey: {detectedInputHotkey}");
            CleanupInputListeners();
        }

        // Start Listening for the Hotkey Globally
        private void StartHotkeyListener()
        {
            if (hotkeyHook == null) // Prevent multiple instances
            {
                Debug.WriteLine("Starting hotkey listener...");
                hotkeyHook = Hook.GlobalEvents();

                // Listen for keyboard hotkey
                hotkeyListener = (sender, e) =>
                {
                    if (isSettingHotkey)
                    {
                        isSettingHotkey = false;
                        return; // Ignore input while setting the hotkey
                    }

                    if (!string.IsNullOrEmpty(detectedInputHotkey) && e.KeyCode.ToString() == detectedInputHotkey)
                    {
                        StartStopSpamming(); // Start or stop spamming
                    }
                };

                // Listen for mouse button hotkey
                hotkeymouseDownHandler = (sender, e) =>
                {
                    if (isSettingHotkey)
                    {
                        isSettingHotkey = false;
                        return; // Ignore input while setting the hotkey
                    }
                    if (!string.IsNullOrEmpty(detectedInputHotkey) && e.Button.ToString() == detectedInputHotkey)
                    {
                        StartStopSpamming(); // Start or stop spamming
                    }
                };

                // Listen for mouse wheel hotkey
                hotkeymouseWheelHandler = (sender, e) =>
                {
                    if (isSettingHotkey)
                    {
                        isSettingHotkey = false;
                        return; // Ignore input while setting the hotkey
                    }
                    string wheelDirection = e.Delta > 0 ? "MouseWheelUp" : "MouseWheelDown";
                    if (!string.IsNullOrEmpty(detectedInputHotkey) && detectedInputHotkey == wheelDirection)
                    {
                        StartStopSpamming(); // Start or stop spamming
                    }
                };

                // Attach global event listeners
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
            string formattedKey = ConvertToSendKeysFormat(detectedInput);

            if (maxClickBox.Checked)
            {
                if (counter < numericUpDown1.Value)
                {
                    counter++;
                    ExecuteKeyOrMouseAction(formattedKey);
                    
                }
                else
                {
                    StartStopSpamming(); // Stop when max count is reached
                   
                }
            }
            else if (infiniteClickBox.Checked)
            {
                ExecuteKeyOrMouseAction(formattedKey);
            }
        }

        private void ExecuteKeyOrMouseAction(string formattedKey)
        {
            Debug.WriteLine("execution");
            if (!string.IsNullOrEmpty(formattedKey))
            {
                SendKeys.SendWait(formattedKey);
                Debug.WriteLine($"Spammed: {formattedKey}");
            }
            else
            {
                Debug.WriteLine($"Spammed: {detectedInput} (Mouse Click)");
            }
        }

        private string ConvertToSendKeysFormat(string key)
        {
            switch (key)
            {
                // Special Keyboard Keys
                case "Enter": return "{ENTER}";
                case "Space": return " ";
                case "Backspace": return "{BACKSPACE}";
                case "Tab": return "{TAB}";
                case "Escape": return "{ESC}";
                case "Delete": return "{DELETE}";
                case "Up": return "{UP}";
                case "Down": return "{DOWN}";
                case "Left": return "{LEFT}";
                case "Right": return "{RIGHT}";
                case "Home": return "{HOME}";
                case "End": return "{END}";
                case "PageUp": return "{PGUP}";
                case "PageDown": return "{PGDN}";
                case "Insert": return "{INSERT}";
                case "PrintScreen": return "{PRTSC}";
                case "ScrollLock": return "{SCROLLLOCK}";
                case "Pause": return "{PAUSE}";
                case "NumLock": return "{NUMLOCK}";
                case "CapsLock": return "{CAPSLOCK}";

                // Function Keys
                case "F1": return "{F1}";
                case "F2": return "{F2}";
                case "F3": return "{F3}";
                case "F4": return "{F4}";
                case "F5": return "{F5}";
                case "F6": return "{F6}";
                case "F7": return "{F7}";
                case "F8": return "{F8}";
                case "F9": return "{F9}";
                case "F10": return "{F10}";
                case "F11": return "{F11}";
                case "F12": return "{F12}";

                // Left & Right Shift Keys
                case "LShiftKey": return "+";
                case "RShiftKey": return "+";

                // Mouse Inputs - Calls mouse_event instead of returning a key
                case "LeftClick":
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    return null;
                case "RightClick":
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                    return null;
                case "Middle":
                    mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
                    return null;
                case "MouseWheelUp":
                    mouse_event(MOUSEEVENTF_XDOWN, 0, 0, 1, 0);
                    return null;
                case "MouseWheelDown":
                    mouse_event(MOUSEEVENTF_XDOWN, 0, 0, unchecked((uint)-1), 0);
                    return null;
                case "XButton1":
                    mouse_event(MOUSEEVENTF_XDOWN, 0, 0, 1, 0);
                    mouse_event(MOUSEEVENTF_XUP, 0, 0, 1, 0);
                    return null;
                case "XButton2":
                    mouse_event(MOUSEEVENTF_XDOWN, 0, 0, 2, 0);
                    mouse_event(MOUSEEVENTF_XUP, 0, 0, 2, 0);
                    return null;

                default:
                    // If it's a single letter or digit, just return it
                    if (key.Length == 1) return key;
                    return ""; // Invalid key
            }
        }

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
