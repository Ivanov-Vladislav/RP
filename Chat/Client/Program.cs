using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Text.Json;

namespace Client
{
    class Program
    {
        private const string EndOfMessage = "<EOF>";
        public static void StartClient(string address, int port, string message)
        {
            try
            {
                IPAddress ipAddress = address == "localhost" ? IPAddress.Loopback : IPAddress.Parse(address);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // CREATE
                Socket sender = new Socket(
                    ipAddress.AddressFamily,
                    SocketType.Stream, 
                    ProtocolType.Tcp);

                try
                {
                    // CONNECT
                    sender.Connect(remoteEP);

                    byte[] text = Encoding.UTF8.GetBytes(message + EndOfMessage);

                    // SEND
                    int bytesSent = sender.Send(text);

                    // RECEIVE                   
                    byte[] buf = new byte[1024];
                    string historyStr = null;
                    int bytesRec = 0;

                    do
                    {
                        bytesRec = sender.Receive(buf);
                        historyStr += Encoding.UTF8.GetString(buf, 0, bytesRec);
                    }
                    while (bytesRec > 0);

                    var history = JsonSerializer.Deserialize<List<string>>(historyStr);

                    foreach (var msg in history)
                    {
                        Console.WriteLine(msg);
                    }

                    // RELEASE
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                throw new ArgumentException("Invalid arguments count");
            }

            StartClient(args[0], Int32.Parse(args[1]), args[2]);
        }
    }
}