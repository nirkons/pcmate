using AudioSwitcher.AudioApi.CoreAudio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCMate
{
    public enum MonitorState
    {
        MonitorStateOn = -1,
        MonitorStateOff = 2,
        MonitorStateStandBy = 1
    }
    public class Actions
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public static int WM_SYSCOMMAND = 0x0112;
        public static int SC_MONITORPOWER = 0xF170;

        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);


        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        public static void Screencontrol(int s)
        {
            //-1 = screen on
            //2 = screen off
            SendMessage(0xFFFF, 0x112, 0xF170, s);
        }

        public static void PressMediaKeys(string key)
        {
            var KEYEVENTF_KEYUP = 0x0002;

            var mediaPlayPause = (byte)Keys.MediaPlayPause;
            var mediaNextTrack = (byte)Keys.MediaNextTrack;
            var mediaPreviousTrack = (byte)Keys.MediaPreviousTrack;

            if (key == "play" || key == "pause")
            {
                keybd_event(mediaPlayPause, mediaPlayPause, 0, 0);
                keybd_event(mediaPlayPause, mediaPlayPause, KEYEVENTF_KEYUP, 0);
            }
            else if (key == "next")
            {
                keybd_event(mediaNextTrack, mediaNextTrack, 0, 0);
                keybd_event(mediaNextTrack, mediaNextTrack, KEYEVENTF_KEYUP, 0);
            }
            else if (key == "previous")
            {
                keybd_event(mediaPreviousTrack, mediaPreviousTrack, 0, 0);
                keybd_event(mediaPreviousTrack, mediaPreviousTrack, KEYEVENTF_KEYUP, 0);
            }
        }

        public static void Launchprog(string ipath, string par)
        {
            var proc = System.Diagnostics.Process.Start(ipath, par);
        }

        public static void Setvol(double newvol)
        {
            CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
            //Debug.WriteLine("Current Volume:" + defaultPlaybackDevice.Volume);
            if (newvol > 100)
            {
                newvol = 100;
            }
            else if (newvol < 0)
            {
                newvol = 0;
            }
            //OLD AudioSwitcher dll which is synchronous 
            //defaultPlaybackDevice.Volume = newvol;
            defaultPlaybackDevice.SetVolumeAsync(newvol);

        }

        public static void Changevol(double changevol)
        {
            CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
            double newvol = defaultPlaybackDevice.Volume + changevol;

            if (changevol > 100)
            {
                changevol = 100;
            }
            else if (changevol < 0)
            {
                changevol = 0;
            }
            //OLD AudioSwitcher dll which is synchronous 
            //defaultPlaybackDevice.Volume = newvol;
            defaultPlaybackDevice.SetVolumeAsync(newvol);
        }

        public static void Delay(string timeformat)
        {
            string[] timelist = timeformat.Split(':');
            //0 - hours
            //1 - minutes
            //2 - seconds
            int totalwait = (int.Parse(timelist[0]) * 3600000) + (int.Parse(timelist[1]) * 60000) + (int.Parse(timelist[2]) * 1000);

            System.Threading.Thread.Sleep(totalwait);
        }

        public static void Computercontrol(string func)
        {
            if (func == "shutdown")
            {
                Process.Start("shutdown", "/s /t 10");

            }
            else if (func == "sleep")
            {
                SetSuspendState(false, true, true);
                //Alternative
                //Application.SetSuspendState(PowerState.Suspend, true, true);

            }
            else if (func == "hibernate")
            {
                SetSuspendState(true, true, true);
                //Alternative
                //Application.SetSuspendState(PowerState.Hibernate, true, true);

            }
        }

        public static void SendWebook(string url)
        {
            // Create a request for the URL.
            WebRequest request = WebRequest.Create(url);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;

            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            // Get the stream containing content returned by the server.
            // The using block ensures the stream is automatically closed.
            using (Stream dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                Console.WriteLine(responseFromServer);
            }

            // Close the response.
            response.Close();
        }


        public static void Audiodevice(string device)
        {
            try
            {
                IEnumerable<CoreAudioDevice> devices = new CoreAudioController().GetPlaybackDevices();
                foreach (CoreAudioDevice d in devices)
                {
                    if (d.FullName == device)
                    {
                        d.SetAsDefault();
                    }
                }
            }
            catch
            {
                Console.WriteLine("Could not set audio device");
            }



        }

    }
}
