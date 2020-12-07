using System;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using System.Linq;
using System.Configuration;

namespace PCMate
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            //Check if another instance is running and kill this one if it is
            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                System.Media.SystemSounds.Asterisk.Play();
                MessageBox.Show("Another instance of this application is already running", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

            //Read Settings
            string key = "audiodevicesrefresh";
            string result = null;
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                 result = appSettings[key] ?? "Not Found";
            }
            catch (ConfigurationErrorsException)
            {
                var appSettings = ConfigurationManager.AppSettings;
                 result = "Not Found";
            }
            if (result == "true")
            {
                //Get audio devices
                List<string> audiodevices = new List<string>();
                try
                {
                    IEnumerable<CoreAudioDevice> devices = new CoreAudioController().GetPlaybackDevices();

                    foreach (CoreAudioDevice d in devices)
                    {
                        if (d.State.ToString() != "Disabled")
                        {
                            audiodevices.Add(d.FullName);
                        }
                        //Console.WriteLine(d.FullName);
                    }
                    Globals.audiodevices = audiodevices;

                    //Serialize list into Json
                    var jsonaudio = JsonConvert.SerializeObject(audiodevices);
                    File.WriteAllText(@"audiodevices.json", jsonaudio);

                }
                catch
                {
                    audiodevices.Add("No audio devices found");
                    Globals.audiodevices = audiodevices;
                }
            }
            else
            {
                LoadAudioDevicesJson();
                //Globals.audiodevices.ForEach(Console.WriteLine);

            }



            //Load DB on startup
            dynamic json = LoadJson();

            //Run Form
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainApp());


        }

        public static dynamic LoadJson()
        {
            //Try to read the database file and if failed create an empty one
            try
            {
                using (StreamReader r = new StreamReader("db.json"))
                {
                    string json = r.ReadToEnd();
                    Globals.jsondb = JsonConvert.DeserializeObject(json);
                    return Globals.jsondb;
                }
            }
            catch (FileNotFoundException)
            {
                File.WriteAllText(@"db.json", "[]");
                Globals.jsondb = "[]";
                return Globals.jsondb;

            }

        }

        public static dynamic LoadAudioDevicesJson()
        {
            //Try to read the database file and if failed create an empty one
            try
            {
                using (StreamReader r = new StreamReader("audiodevices.json"))
                {
                    string json = r.ReadToEnd();
                    Globals.audiodevices = JsonConvert.DeserializeObject<List<string>>(json);
        
                    return Globals.audiodevices;
                }
            }
            catch (FileNotFoundException)
            {
                List<string> audiodevices = new List<string>();
                try
                {
                    IEnumerable<CoreAudioDevice> devices = new CoreAudioController().GetPlaybackDevices();

                    foreach (CoreAudioDevice d in devices)
                    {
                        if (d.State.ToString() != "Disabled")
                        {
                            audiodevices.Add(d.FullName);
                        }
                        //Console.WriteLine(d.FullName);
                    }
                    Globals.audiodevices = audiodevices;

                    //Serialize list into Json
                    var jsonaudio = JsonConvert.SerializeObject(audiodevices);
                    File.WriteAllText(@"audiodevices.json", jsonaudio);
                    return Globals.audiodevices;

                }
                catch
                {
                    Globals.audiodevices = new List<string>();
                    Globals.audiodevices.Add("No audio devices found, Check settings page");
                    return Globals.audiodevices;
                }
            }

        }




    }

}
