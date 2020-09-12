namespace PCMate
{
    partial class SettingsPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsPage));
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.autostartCheckbox = new System.Windows.Forms.CheckBox();
            this.portCheckBox = new System.Windows.Forms.CheckBox();
            this.ipCheckBox = new System.Windows.Forms.CheckBox();
            this.ipAddressBox = new System.Windows.Forms.TextBox();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.errorMsg = new System.Windows.Forms.Label();
            this.minimizedCheckBox = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.AudioDevicesChkbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(21, 182);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 36);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(106, 182);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 36);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // autostartCheckbox
            // 
            this.autostartCheckbox.AutoSize = true;
            this.autostartCheckbox.Location = new System.Drawing.Point(52, 94);
            this.autostartCheckbox.Name = "autostartCheckbox";
            this.autostartCheckbox.Size = new System.Drawing.Size(117, 17);
            this.autostartCheckbox.TabIndex = 2;
            this.autostartCheckbox.Text = "Start with computer";
            this.autostartCheckbox.UseVisualStyleBackColor = true;
            this.autostartCheckbox.CheckedChanged += new System.EventHandler(this.autostartCheckbox_CheckedChanged);
            // 
            // portCheckBox
            // 
            this.portCheckBox.AutoSize = true;
            this.portCheckBox.Location = new System.Drawing.Point(52, 62);
            this.portCheckBox.Name = "portCheckBox";
            this.portCheckBox.Size = new System.Drawing.Size(15, 14);
            this.portCheckBox.TabIndex = 3;
            this.portCheckBox.UseVisualStyleBackColor = true;
            this.portCheckBox.CheckedChanged += new System.EventHandler(this.portCheckBox_CheckedChanged);
            // 
            // ipCheckBox
            // 
            this.ipCheckBox.AutoSize = true;
            this.ipCheckBox.Location = new System.Drawing.Point(52, 29);
            this.ipCheckBox.Name = "ipCheckBox";
            this.ipCheckBox.Size = new System.Drawing.Size(15, 14);
            this.ipCheckBox.TabIndex = 4;
            this.ipCheckBox.UseVisualStyleBackColor = true;
            this.ipCheckBox.CheckedChanged += new System.EventHandler(this.ipCheckBox_CheckedChanged);
            // 
            // ipAddressBox
            // 
            this.ipAddressBox.Enabled = false;
            this.ipAddressBox.Location = new System.Drawing.Point(83, 26);
            this.ipAddressBox.Name = "ipAddressBox";
            this.ipAddressBox.Size = new System.Drawing.Size(100, 20);
            this.ipAddressBox.TabIndex = 5;
            this.ipAddressBox.TextChanged += new System.EventHandler(this.ipAddressBox_TextChanged);
            // 
            // portTextBox
            // 
            this.portTextBox.Enabled = false;
            this.portTextBox.Location = new System.Drawing.Point(83, 62);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(100, 20);
            this.portTextBox.TabIndex = 6;
            this.portTextBox.TextChanged += new System.EventHandler(this.portTextBox_TextChanged);
            // 
            // errorMsg
            // 
            this.errorMsg.AutoSize = true;
            this.errorMsg.ForeColor = System.Drawing.Color.Red;
            this.errorMsg.Location = new System.Drawing.Point(49, 166);
            this.errorMsg.Name = "errorMsg";
            this.errorMsg.Size = new System.Drawing.Size(0, 13);
            this.errorMsg.TabIndex = 7;
            this.errorMsg.Visible = false;
            // 
            // minimizedCheckBox
            // 
            this.minimizedCheckBox.AutoSize = true;
            this.minimizedCheckBox.Location = new System.Drawing.Point(52, 123);
            this.minimizedCheckBox.Name = "minimizedCheckBox";
            this.minimizedCheckBox.Size = new System.Drawing.Size(96, 17);
            this.minimizedCheckBox.TabIndex = 8;
            this.minimizedCheckBox.Text = "Start minimized";
            this.minimizedCheckBox.UseVisualStyleBackColor = true;
            this.minimizedCheckBox.CheckedChanged += new System.EventHandler(this.minimizedCheckBox_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(190, 182);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 36);
            this.button1.TabIndex = 9;
            this.button1.Text = "Restore Defaults";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // AudioDevicesChkbox
            // 
            this.AudioDevicesChkbox.AutoSize = true;
            this.AudioDevicesChkbox.Location = new System.Drawing.Point(52, 151);
            this.AudioDevicesChkbox.Name = "AudioDevicesChkbox";
            this.AudioDevicesChkbox.Size = new System.Drawing.Size(182, 17);
            this.AudioDevicesChkbox.TabIndex = 10;
            this.AudioDevicesChkbox.Text = "Refresh audio devices on startup";
            this.AudioDevicesChkbox.UseVisualStyleBackColor = true;
            this.AudioDevicesChkbox.CheckedChanged += new System.EventHandler(this.AudioDevicesChkbox_CheckedChanged);
            // 
            // SettingsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(294, 230);
            this.Controls.Add(this.AudioDevicesChkbox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.minimizedCheckBox);
            this.Controls.Add(this.errorMsg);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.ipAddressBox);
            this.Controls.Add(this.ipCheckBox);
            this.Controls.Add(this.portCheckBox);
            this.Controls.Add(this.autostartCheckbox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.saveButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsPage";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsPage_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox autostartCheckbox;
        private System.Windows.Forms.CheckBox portCheckBox;
        private System.Windows.Forms.CheckBox ipCheckBox;
        private System.Windows.Forms.TextBox ipAddressBox;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Label errorMsg;
        private System.Windows.Forms.CheckBox minimizedCheckBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox AudioDevicesChkbox;
    }
}