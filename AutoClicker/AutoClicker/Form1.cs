using System;
using System.Diagnostics;
using System.Windows.Forms;
using Gma.System.MouseKeyHook; // This is needed for global hooks

namespace AutoClicker
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        private IKeyboardMouseEvents globalHook;
        private string detectedInput;



        public Form1()
        {
            InitializeComponent();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {

        }
        private void btnStart_Click(object sender, EventArgs e)
        {

        }

        private void hoursLabel_Click(object sender, EventArgs e)
        {

        }

        private void hoursTBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void minutesTBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void settingsButton1_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                button.Text = "Press any key or button";

                // Initialize the global event hook
                globalHook = Hook.GlobalEvents();

                // Listen for any key press
                globalHook.KeyDown += GlobalHook_KeyDown;

                // Listen for any mouse button press
                globalHook.MouseDown += GlobalHook_MouseDown;

                // Listen for mouse wheel scroll
                globalHook.MouseWheel += GlobalHook_MouseWheel;

                Debug.WriteLine("Listening for any key or mouse button press...");



            }

        }

        private void settingLabel1_Click(object sender, EventArgs e)
        {

        }

        private void millisTBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void secondsTBox_TextChanged(object sender, EventArgs e)
        {

        }
        private void hotkeyButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                button.Text = "Press any key or button";

                // Initialize the global event hook
                globalHook = Hook.GlobalEvents();

                // Listen for any key press
                globalHook.KeyDown += GlobalHook_KeyDown;

                // Listen for any mouse button press
                globalHook.MouseDown += GlobalHook_MouseDown;

                // Listen for mouse wheel scroll
                globalHook.MouseWheel += GlobalHook_MouseWheel;

                Debug.WriteLine("Listening for any key or mouse button press...");



            }

        }

        //Customs functions below
        private void GlobalHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Prevents the button click effect
            }

            detectedInput = "Key: " + e.KeyCode.ToString(); // Store input
            settingsButton1.Text = detectedInput; // Update button text

            Debug.WriteLine(detectedInput);
            CleanupInputListeners();
        }

        private void GlobalHook_MouseDown(object sender, MouseEventArgs e)
        {
            detectedInput = "Mouse: " + e.Button.ToString(); // Store input
            settingsButton1.Text = detectedInput; // Update button text

            Debug.WriteLine(detectedInput);
            CleanupInputListeners();
        }

        private void GlobalHook_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                settingsButton1.Text = "Scrolled Up"; // Display user-friendly text
                detectedInput = "MouseWheelUp"; // Store for automation
            }
            else
            {
                settingsButton1.Text = "Scrolled Down"; // Display user-friendly text
                detectedInput = "MouseWheelDown"; // Store for automation
            }

            Debug.WriteLine($"Mouse Wheel Moved: {e.Delta}, Stored Input: {detectedInput}");
            CleanupInputListeners();
        }

        private void CleanupInputListeners()
        {
            if (globalHook != null)
            {
                globalHook.KeyDown -= GlobalHook_KeyDown;
                globalHook.MouseDown -= GlobalHook_MouseDown;
                globalHook.MouseWheel -= GlobalHook_MouseWheel;
                globalHook.Dispose();
                globalHook = null;
            }

            // Reset button text
            // settingsButton1.Text = "Start Listening";
        }

        
    }
}
