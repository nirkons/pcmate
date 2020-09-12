using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shell32;


namespace PCMate
{
    public partial class SettingsPage : Form
    {
        //RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        public SettingsPage()
        {
            InitializeComponent();
            //Don't allow resizing
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Check to see the current state (running at startup or not)
            /*
            if (rkApp.GetValue("PCMate") == null)
            {
                // The value doesn't exist, the application is not set to run at startup
                autostartCheckbox.Checked = false;
            }
            else
            {
                // The value exists, the application is set to run at startup
                autostartCheckbox.Checked = true;
            }*/


            string startUpFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            DirectoryInfo di = new DirectoryInfo(startUpFolderPath);
            FileInfo[] files = di.GetFiles("*.lnk");

            foreach (FileInfo fi in files)
            {
                string shortcutTargetFile = GetShortcutTargetFile(fi.FullName);

                if (shortcutTargetFile.EndsWith("PCMate.exe",
                      StringComparison.InvariantCultureIgnoreCase))
                {
                    autostartCheckbox.Checked = true;
                }
                else
                {
                    autostartCheckbox.Checked = false;
                }
            }


            string ipsettings = ReadSetting("ipAddress");


        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (ipCheckBox.Checked == true && ipAddressBox.Text != "")
            {
                if (IsValidIP(ipAddressBox.Text))
                {
                    errorMsg.Visible = false;
                }
                else
                {
                    errorMsg.Text = "Invalid IP Address";
                    errorMsg.Visible = true;
                }
            }

            if (portCheckBox.Checked == true && portTextBox.Text != "")
            {
                if (Regex.IsMatch(portTextBox.Text, @"^\d+$"))
                {
                    errorMsg.Visible = false;
                }
                else
                {
                    errorMsg.Text = "Invalid Port";
                    errorMsg.Visible = true;
                }
            }

            if (!autostartCheckbox.Checked)
            {
                // Remove the value from the registry so that the application doesn't start
                //rkApp.DeleteValue("PCMate", false);
                string startUpFolderPath =  Environment.GetFolderPath(Environment.SpecialFolder.Startup);

                DirectoryInfo di = new DirectoryInfo(startUpFolderPath);
                FileInfo[] files = di.GetFiles("*.lnk");

                foreach (FileInfo fi in files)
                {
                    string shortcutTargetFile = GetShortcutTargetFile(fi.FullName);

                    if (shortcutTargetFile.EndsWith("PCMate.exe",
                          StringComparison.InvariantCultureIgnoreCase))
                    {
                        System.IO.File.Delete(fi.FullName);
                    }
                }
            }
            else
            {
                //rkApp.SetValue("PCMate", Application.ExecutablePath);
                WshShell wshShell = new WshShell();

                IWshRuntimeLibrary.IWshShortcut shortcut;
                string startUpFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                Console.WriteLine(startUpFolderPath);
                // Create the shortcut
                shortcut =
                  (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(
                    startUpFolderPath + "\\" +
                    Application.ProductName + ".lnk");

                shortcut.TargetPath = Application.ExecutablePath;
                shortcut.WorkingDirectory = Application.StartupPath;
                shortcut.Description = "Launch PCMate";
                shortcut.IconLocation = Application.StartupPath + @"\Logo_Icon.ico";
                shortcut.Save();


            }


            if (errorMsg.Visible == false)
            {
                //SAVE
                if (ipCheckBox.Checked)
                {
                    AddUpdateAppSettings("ipAddress", ipAddressBox.Text);
                }
                else
                {
                    RemoveSettings("ipAddress");
                }
                if (portCheckBox.Checked)
                {
                    AddUpdateAppSettings("port", portTextBox.Text);
                }
                else
                {
                    RemoveSettings("port");
                }

                if (minimizedCheckBox.Checked == true)
                {
                    AddUpdateAppSettings("startminimized", "true");
                }
                else
                {
                    RemoveSettings("startminimized");
                }

                if (AudioDevicesChkbox.Checked == true)
                {
                    AddUpdateAppSettings("audiodevicesrefresh", "true");
                }
                else
                {
                    RemoveSettings("audiodevicesrefresh");
                }

                this.Close();
            
            }



        }

        private void ipAddressBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void portTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void portCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.portCheckBox.Checked == true)
            {
                portTextBox.Enabled = true;
            }
            else
            {
                portTextBox.Enabled = false;

            }
        }

        private void autostartCheckbox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void SettingsPage_Load(object sender, EventArgs e)
        {

            string ipsettings = ReadSetting("ipAddress");
            if (ipsettings != "Not Found" && ipsettings != "")
            {
                ipAddressBox.Text = ipsettings;
                ipCheckBox.Checked = true;
            }
            else
            {
                //IP address text box event handlers
                this.ipAddressBox.Enter += new EventHandler(ipAddressBox_Enter);
                this.ipAddressBox.Leave += new EventHandler(ipAddressBox_Leave);
                ipAddressBox_SetText();
            }


            string portsettings = ReadSetting("port");
            if (portsettings != "Not Found"&& portsettings != "")
            {
                portTextBox.Text = portsettings;
                portCheckBox.Checked = true;
            }
            else
            {
                //Port number text box event handler
                this.portTextBox.Enter += new EventHandler(portTextBox_Enter);
                this.portTextBox.Leave += new EventHandler(portTextBox_Leave);
                portTextBox_SetText();
            }

            string minimized = ReadSetting("startminimized");
            if (minimized != "Not Found" && minimized != "")
            {
                minimizedCheckBox.Checked = true;
            }
            else
            {
                minimizedCheckBox.Checked = false;
            }

            string audiodevicesrefresh = ReadSetting("audiodevicesrefresh");
            if (audiodevicesrefresh != "Not Found" && audiodevicesrefresh != "")
            {
                AudioDevicesChkbox.Checked = true;
            }


        }

        protected void ipAddressBox_SetText()
        {
            this.ipAddressBox.Text = "Local IP Address";
            ipAddressBox.ForeColor = Color.Gray;
        }

        private void ipAddressBox_Enter(object sender, EventArgs e)
        {
            if (ipAddressBox.ForeColor == Color.Black)
                return;
            ipAddressBox.Text = "";
            ipAddressBox.ForeColor = Color.Black;
        }
        private void ipAddressBox_Leave(object sender, EventArgs e)
        {
            if (ipAddressBox.Text.Trim() == "")
                ipAddressBox_SetText();
        }

        private void ipCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ipCheckBox.Checked == true)
            {
                ipAddressBox.Enabled = true;
            }
            else
            {
                ipAddressBox.Enabled = false;

            }
        }
        //PORT
        protected void portTextBox_SetText()
        {
            this.portTextBox.Text = "Port";
            portTextBox.ForeColor = Color.Gray;
        }

        private void portTextBox_Enter(object sender, EventArgs e)
        {
            if (portTextBox.ForeColor == Color.Black)
                return;
            portTextBox.Text = "";
            portTextBox.ForeColor = Color.Black;
        }
        private void portTextBox_Leave(object sender, EventArgs e)
        {
            if (portTextBox.Text.Trim() == "")
                portTextBox_SetText();
        }

        public bool IsValidIP(string addr)
        {
            //create our match pattern
            string pattern = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";
            //create our Regular Expression object
            Regex check = new Regex(pattern);
            //boolean variable to hold the status
            bool valid = false;
            //check to make sure an ip address was provided
            if (addr == "")
            {
                //no address provided so return false
                valid = false;
            }
            else
            {
                //address provided so use the IsMatch Method
                //of the Regular Expression object
                valid = check.IsMatch(addr, 0);
            }
            //return the results
            return valid;
        }


        static string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? "Not Found";
                return result;
            }
            catch (ConfigurationErrorsException)
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = "Not Found";
                return result;
            }
        }

        static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        static void RemoveSettings(string key)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] != null)
                {
                    settings.Remove(key);
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            portTextBox_SetText();
            ipAddressBox_SetText();
            portTextBox.Enabled = false;
            ipAddressBox.Enabled = false;
            ipCheckBox.Checked = false;
            portCheckBox.Checked = false;
            autostartCheckbox.Checked = false;
            minimizedCheckBox.Checked = false;
        }

        private void minimizedCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }
            
        public string GetShortcutTargetFile(string shortcutFilename)
        {
            string pathOnly = Path.GetDirectoryName(shortcutFilename);
            string filenameOnly = Path.GetFileName(shortcutFilename);

            Shell32.Shell shell = new Shell32.ShellClass();
            Shell32.Folder folder = shell.NameSpace(pathOnly);
            Shell32.FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                Shell32.ShellLinkObject link =
                  (Shell32.ShellLinkObject)folderItem.GetLink;
                return link.Path;
            }

            return String.Empty; // Not found
        }

        private void AudioDevicesChkbox_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
