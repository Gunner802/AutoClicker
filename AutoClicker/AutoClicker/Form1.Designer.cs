namespace AutoClicker
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnStart = new Button();
            btnStop = new Button();
            tiBox = new GroupBox();
            millisLabel = new Label();
            secondsLabel = new Label();
            minutesLabel = new Label();
            hoursLabel = new Label();
            millisTBox = new TextBox();
            minutesTBox = new TextBox();
            secondsTBox = new TextBox();
            hoursTBox = new TextBox();
            settingsBox = new GroupBox();
            settingsButton1 = new Button();
            settingLabel1 = new Label();
            hotkeyButton = new Button();
            tiBox.SuspendLayout();
            settingsBox.SuspendLayout();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(76, 387);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(127, 53);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(272, 387);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(127, 53);
            btnStop.TabIndex = 1;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // tiBox
            // 
            tiBox.Controls.Add(millisLabel);
            tiBox.Controls.Add(secondsLabel);
            tiBox.Controls.Add(minutesLabel);
            tiBox.Controls.Add(hoursLabel);
            tiBox.Controls.Add(millisTBox);
            tiBox.Controls.Add(minutesTBox);
            tiBox.Controls.Add(secondsTBox);
            tiBox.Controls.Add(hoursTBox);
            tiBox.Location = new Point(12, 12);
            tiBox.Name = "tiBox";
            tiBox.Size = new Size(473, 62);
            tiBox.TabIndex = 2;
            tiBox.TabStop = false;
            tiBox.Text = "Time Intervals";
            // 
            // millisLabel
            // 
            millisLabel.AutoSize = true;
            millisLabel.Location = new Point(393, 22);
            millisLabel.Name = "millisLabel";
            millisLabel.Size = new Size(73, 15);
            millisLabel.TabIndex = 6;
            millisLabel.Text = "milliseconds";
            // 
            // secondsLabel
            // 
            secondsLabel.AutoSize = true;
            secondsLabel.Location = new Point(279, 22);
            secondsLabel.Name = "secondsLabel";
            secondsLabel.Size = new Size(50, 15);
            secondsLabel.TabIndex = 5;
            secondsLabel.Text = "seconds";
            // 
            // minutesLabel
            // 
            minutesLabel.AutoSize = true;
            minutesLabel.Location = new Point(165, 22);
            minutesLabel.Name = "minutesLabel";
            minutesLabel.Size = new Size(50, 15);
            minutesLabel.TabIndex = 4;
            minutesLabel.Text = "minutes";
            // 
            // hoursLabel
            // 
            hoursLabel.AutoSize = true;
            hoursLabel.Location = new Point(64, 22);
            hoursLabel.Name = "hoursLabel";
            hoursLabel.Size = new Size(37, 15);
            hoursLabel.TabIndex = 3;
            hoursLabel.Text = "hours";
            hoursLabel.Click += hoursLabel_Click;
            // 
            // millisTBox
            // 
            millisTBox.Location = new Point(335, 22);
            millisTBox.Name = "millisTBox";
            millisTBox.Size = new Size(52, 23);
            millisTBox.TabIndex = 3;
            millisTBox.Text = "1000";
            millisTBox.TextChanged += millisTBox_TextChanged;
            // 
            // minutesTBox
            // 
            minutesTBox.Location = new Point(107, 22);
            minutesTBox.Name = "minutesTBox";
            minutesTBox.Size = new Size(52, 23);
            minutesTBox.TabIndex = 2;
            minutesTBox.Text = "0";
            minutesTBox.TextChanged += minutesTBox_TextChanged;
            // 
            // secondsTBox
            // 
            secondsTBox.Location = new Point(221, 22);
            secondsTBox.Name = "secondsTBox";
            secondsTBox.Size = new Size(52, 23);
            secondsTBox.TabIndex = 1;
            secondsTBox.Text = "0";
            secondsTBox.TextChanged += secondsTBox_TextChanged;
            // 
            // hoursTBox
            // 
            hoursTBox.Location = new Point(6, 22);
            hoursTBox.Name = "hoursTBox";
            hoursTBox.Size = new Size(52, 23);
            hoursTBox.TabIndex = 0;
            hoursTBox.Text = "0";
            hoursTBox.TextChanged += hoursTBox_TextChanged;
            // 
            // settingsBox
            // 
            settingsBox.Controls.Add(settingsButton1);
            settingsBox.Controls.Add(settingLabel1);
            settingsBox.Location = new Point(12, 95);
            settingsBox.Name = "settingsBox";
            settingsBox.Size = new Size(466, 94);
            settingsBox.TabIndex = 3;
            settingsBox.TabStop = false;
            settingsBox.Text = "Settings";
            // 
            // settingsButton1
            // 
            settingsButton1.Location = new Point(101, 15);
            settingsButton1.Name = "settingsButton1";
            settingsButton1.Size = new Size(144, 23);
            settingsButton1.TabIndex = 1;
            settingsButton1.Text = "empty";
            settingsButton1.UseVisualStyleBackColor = true;
            settingsButton1.Click += settingsButton1_Click;
            // 
            // settingLabel1
            // 
            settingLabel1.AutoSize = true;
            settingLabel1.Location = new Point(6, 19);
            settingLabel1.Name = "settingLabel1";
            settingLabel1.Size = new Size(89, 15);
            settingLabel1.TabIndex = 0;
            settingLabel1.Text = "Button to Click:";
            settingLabel1.Click += settingLabel1_Click;
            // 
            // hotkeyButton
            // 
            hotkeyButton.Location = new Point(177, 459);
            hotkeyButton.Name = "hotkeyButton";
            hotkeyButton.Size = new Size(127, 53);
            hotkeyButton.TabIndex = 4;
            hotkeyButton.Text = "Hotkey";
            hotkeyButton.UseVisualStyleBackColor = true;
            hotkeyButton.Click += hotkeyButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(497, 524);
            Controls.Add(hotkeyButton);
            Controls.Add(settingsBox);
            Controls.Add(tiBox);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Auto Clicker";
            tiBox.ResumeLayout(false);
            tiBox.PerformLayout();
            settingsBox.ResumeLayout(false);
            settingsBox.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button btnStart;
        private Button btnStop;
        private GroupBox tiBox;
        private TextBox hoursTBox;
        private Label minutesLabel;
        private Label hoursLabel;
        private TextBox millisTBox;
        private TextBox minutesTBox;
        private TextBox secondsTBox;
        private Label millisLabel;
        private Label secondsLabel;
        private GroupBox settingsBox;
        private Label settingLabel1;
        private Button settingsButton1;
        private Button hotkeyButton;
    }
}
