using System;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using System.Linq;

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
                MessageBox.Show("Another instance of this application is already running", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

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
            }
            catch
            {
                audiodevices.Add("No audio devices found");
                Globals.audiodevices = audiodevices;
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


        

    }

}
