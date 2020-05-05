using Newtonsoft.Json;
using PCMate;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

//Adapted from Microsoft's Asynchronous server socket example here:
//https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
//And modified to fit this project's needs



//State object for reading client data asynchronously  
public class StateObject
{
    // Client  socket.  
    public Socket workSocket = null;
    // Size of receive buffer.  
    public const int BufferSize = 1024;
    // Receive buffer.  
    public byte[] buffer = new byte[BufferSize];
    // Received data string.  
    public StringBuilder sb = new StringBuilder();
}

public class AsynchronousSocketListener
{


    // Thread signal.  
    public static ManualResetEvent allDone = new ManualResetEvent(false);


    //public event EventHandler LastMessageChanged;

    public delegate void newUtterance();
    public static event newUtterance updateGlobal;

    public static string lastmessage;

    public AsynchronousSocketListener()
    {
        StartListening();
    }

    public static void StartListening()
    {
        // Data buffer for incoming data.  
        byte[] bytes = new Byte[1024];

        // Establish the local endpoint for the socket.  
        // The DNS name of the computer  
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

        IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];

        //Get local network IP
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            //169.254.0.1
            try
            {
                string ip = "1.1.1.1";
                try
                {
                    var appSettings = ConfigurationManager.AppSettings;
                    ip = appSettings["ipAddress"] ?? "1.1.1.1";
                }
                catch (ConfigurationErrorsException)
                {
                }

                socket.Connect(ip, 65530);

                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                ipAddress = endPoint.Address;
                Console.WriteLine(ipAddress);

            }
            catch (Exception e)
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipAddress = ip;
                    }
                }
            }
        }
        string port = "6667";
        try
        {
            var appSettings = ConfigurationManager.AppSettings;
            port = appSettings["port"] ?? "6667";
        }
        catch (ConfigurationErrorsException)
        {
        }

        int iport = int.Parse(port);

        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, iport);

        // Create a TCP/IP socket.  
        Socket listener = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming connections.  
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);

            while (true)
            {
                // Set the event to nonsignaled state.  
                allDone.Reset();

                // Start an asynchronous socket to listen for connections.  
                Console.WriteLine("Waiting for a connection...");
                listener.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    listener);

                // Wait until a connection is made before continuing.  
                allDone.WaitOne();
            }

        }
        catch (Exception e)
        {
            // Get stack trace for the exception with source file information
            var st = new StackTrace(e, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            Console.WriteLine(line);


        }

    }

    public static void AcceptCallback(IAsyncResult ar)
    {
        // Signal the main thread to continue.  
        allDone.Set();

        // Get the socket that handles the client request.  
        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);

        // Create the state object.  
        StateObject state = new StateObject();
        state.workSocket = handler;
        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
            new AsyncCallback(ReadCallback), state);
    }

    public static void ReadCallback(IAsyncResult ar)
    {

        String content = String.Empty;

        // Retrieve the state object and the handler socket  
        // from the asynchronous state object.  
        StateObject state = (StateObject)ar.AsyncState;
        Socket handler = state.workSocket;

        // Read data from the client socket.   
        int bytesRead = handler.EndReceive(ar);

        if (bytesRead > 0)
        {
            // There  might be more data, so store the data received so far.  
            state.sb.Append(Encoding.ASCII.GetString(
                state.buffer, 0, bytesRead));

            // Check for end-of-file tag. If it is not there, read   
            // more data.  
            content = state.sb.ToString();

            if (content.IndexOf("}") > -1)
            {
                // All the data has been read from the   
                // client. Display it on the console.  
                //Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);
                string json = content.Substring(content.IndexOf('{'));

                //Sometimes the json arrives without quotes
                if (!json.Contains("\""))
                {
                    // Exchange all words with words surrounded by `"`
                    // ((\\w+\\s+)*\\w+) becomes ((\w+\s+)*\w+), ie all words with possible spaces
                    var match = "((\\w+\\s+)*\\w+)";
                    // \"$1\" becomes "$1" which is the sequence for replace the first group (ie the one
                    // we caught above) with the same group but with extra "
                    var replacement = "\"$1\"";
                    json = Regex.Replace(json, match, replacement);
                }

                Console.WriteLine(json);

                // Parse it.
                dynamic obj = JsonConvert.DeserializeObject(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore });
                //dynamic obj = Json.Decode(correctJSON);
                Console.WriteLine(obj);

                //Send(handler, content);
                Send(handler, "HTTP/1.1 200 OK\r\n");

                //PERFORM ACTION

                string intent = obj["intent"];
                string utterance = obj["utterance"];
                string number = obj["number"];

                if (intent == "null")
                {
                    lastmessage = "Intent " + intent +" is not supported";
                }
                else
                {
                    lastmessage = utterance;

                }

                updateGlobal();


                foreach (var flow in Globals.jsondb)
                {
                    if (flow.intent == intent)
                    {

                        //Split utterance synonyms
                        string[] multiutterances = Convert.ToString(flow.utterance).Split(',');
                        bool utterancematch = false;
                        foreach (string word in multiutterances)
                        {
                            //Console.WriteLine(word.Trim().ToLower());
                            if (utterance.ToLower() == word.Trim().ToLower())
                            {
                                utterancematch = true;
                            }
                        }

                        Console.WriteLine("Match:" + utterancematch);
                        if (utterancematch)
                        {
                            foreach (var record in flow.actionlist)
                            {
                                string actiontype = record.actiontype;
                                string param1 = record.param1;
                                string param2 = record.param2;
                                string param3 = record.param3;
                                string param4 = record.param4;

                                if (actiontype == "launch")
                                {
                                    string ipath = param1;
                                    string par = record.param2;
                                    Actions.Launchprog(ipath, par);
                                }

                                if (actiontype == "screencontrol")
                                {
                                    if (param1 == "off")
                                    {
                                        Actions.Screencontrol(2);
                                    }
                                    else
                                    {
                                        Actions.Screencontrol(-1);
                                    }
                                }

                                if (actiontype == "setvolume")
                                {
                                    int i;
                                    if (param1 == "{ Voice Controlled }")
                                    {
                                        if (int.TryParse(number, out i))
                                        {
                                            double vol = Convert.ToDouble(number);
                                            Actions.Setvol(vol);
                                        }
                                    }
                                    else
                                    {
                                        if (int.TryParse(param1, out i))
                                        {
                                            double vol = Convert.ToDouble(param1);
                                            Actions.Setvol(vol);
                                        }

                                    }
                                    
                                }

                                if (actiontype == "changevolume")
                                {
                                    int i;
                                    if (param1 == "increase")
                                    {
                                        if (int.TryParse(Convert.ToString(record.param2), out i))
                                        {
                                            //keybd_event((byte)Keys.VolumeUp, 0, 0, 0); // increase volume by 1 step (2 volume)
                                            double incvol = record.param2;
                                            Actions.Changevol(incvol);
                                        }
                                        else
                                        {
                                            double incvol = 2;
                                            Actions.Changevol(incvol);
                                        }

                                    }
                                    else if (param1 == "decrease")
                                    {
                                        if (int.TryParse(Convert.ToString(record.param2), out i))
                                        {
                                            //keybd_event((byte)Keys.VolumeDown, 0, 0, 0); // decrease volume by 1 step (2 volume)
                                            double decvol = record.param2;
                                            if (decvol < 0)
                                            {
                                                Actions.Changevol(decvol);
                                            }
                                            else
                                            {
                                                Actions.Changevol((decvol * -1));
                                            }
                                        }
                                        else
                                        {
                                            double decvol = -2;
                                            Actions.Changevol(decvol);
                                        }


                                    }


                                }

                                if (actiontype == "computertask")
                                {
                                    if (param1 == "shutdown")
                                    {
                                        Actions.Computercontrol(param1);
                                    }
                                    else if (param1 == "sleep")
                                    {
                                        Actions.Computercontrol(param1);
                                    }
                                    else if (param1 == "hibernate")
                                    {
                                        Actions.Computercontrol(param1);
                                    }
                                    else if (param1 == "play")
                                    {
                                        Actions.PressMediaKeys("play");
                                    }
                                    else if (param1 == "pause")
                                    {
                                        Actions.PressMediaKeys("pause");
                                    }
                                    else if (param1 == "next")
                                    {
                                        Actions.PressMediaKeys("next");
                                    }
                                    else if (param1 == "previous")
                                    {
                                        Actions.PressMediaKeys("previous");
                                    }
                                }

                                if (actiontype == "timetask")
                                {
                                    if (param1 == "delay")
                                    {
                                        Actions.Delay(Convert.ToString(record.param2));
                                    }
                                }

                                if (actiontype == "webhook")
                                {
                                    Actions.SendWebook(param1);
                                }

                                if (actiontype == "changeaudiodevice")
                                {
                                    Actions.Audiodevice(param1);
                                }

                            }
                        }
                    }
                }
            }
            else
            {
                // Not all data received. Get more.  
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
            }
        }
    }

    private static void Send(Socket handler, String data)
    {
        //SEND BACK


        // Convert the string data to byte data using ASCII encoding.  
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.  
        handler.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), handler);
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.  
            Socket handler = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device. 
            int bytesSent = handler.EndSend(ar);
            //Console.WriteLine("Sent {0} bytes to client.", bytesSent);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();

        }
        catch (Exception e)
        {
            // Get stack trace for the exception with source file information
            var st = new StackTrace(e, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            Console.WriteLine(line);

        }
    }
}