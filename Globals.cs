using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCMate
{
    class Globals
    {
        public static dynamic jsondb;

        public static List<string> audiodevices;

        //array containing the list of values for the flow Main Branch intent dropdown control
        public static string[,] arrayOfUserIntentValueNames = new string[,] { { "Slow down", "turn on" }, { "Speed up", "turn on" }, { "Rewind", "turn on" }, { "Fast Forward", "turn on" }, { "Restart", "restart" }, { "Reload", "restart" }, { "Reboot", "restart" }, { "Reset", "restart" }, { "Set", "set" }, { "Mute", "mute" }, { "Shutdown", "shutdown" }, { "Power Off", "shutdown" }, { "Unlock", "unlock" }, { "Lock", "lock" }, { "Decrease", "decrease" }, { "Increase", "increase" }, { "Change", "change" }, { "Switch", "change" },  { "Adjust", "change" }, { "Turn Off", "turn off" }, { "Stop", "turn off" }, { "Pause", "turn off" }, { "Close", "turn off" }, { "Disable", "turn off" }, { "Deactivate", "turn off" }, { "Disconnect", "turn off" }, { "Switch Off", "turn off" }, { "Disconnect", "turn off" }, { "Turn On", "turn on" }, { "Play", "turn on" }, { "Launch", "turn on" }, { "Open", "turn on" }, { "Enable", "turn on" }, { "Reconnect", "turn on" }, { "Connect", "turn on" }, { "Activate", "turn on" }, { "Switch On", "turn on" } };

        //array containing the list of values and types for the flow Sub Branch Branch actions dropdown control
        public static string[,] arrayOfUserActionValueNames = new string[,] { { "Open Program", "launch","" }, { "Set Volume", "setvolume", "" }, { "Increase Volume", "changevolume", "increase" }, { "Decrease Volume", "changevolume", "decrease" }, { "Change Audio Device", "changeaudiodevice", "" }, { "Turn On Screen", "screencontrol", "on" }, { "Turn Off Screen", "screencontrol", "off" }, { "Delay", "timetask", "delay" }, { "Shutdown", "computertask", "shutdown" }, { "Sleep", "computertask", "sleep" }, { "Hibernate", "computertask", "hibernate" }, { "Media Key - Play/Pause", "computertask", "play" }, { "Media Key - Next", "computertask", "next" }, { "Media Key - Previous", "computertask", "previous" }, { "Webhook", "webhook", "webhook" } };

    }
}
