using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chain
{
    class Program
    {
        private static Socket _listener;
        private static Socket _sender;

        static void Main(string[] args)
        {
            try
            {
                Arguments arguments = ParseArgs(args);

                CreateListenerConnection(arguments.NextPort);
                CreateSenderConnection(arguments.ListeningPort, arguments.NextHost);

                if (arguments.IsInitiator) 
                {
                    WorkAsInitiator();
                }
                else
                {
                    WorkAsOrdinaryProcess();
                }

                _sender.Shutdown(SocketShutdown.Both);
                _sender.Close();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }

            System.Console.ReadLine();
        }

        private static Arguments ParseArgs(string[] args)
        {
            if (args.Length < 2 || args.Length > 4)
            {                
                throw new ArithmeticException("Invalid argumants count");
            }

            Arguments arguments = new Arguments();
            arguments.ListeningPort = Int32.Parse(args[0]);
            arguments.NextHost = args[1];
            arguments.NextPort = Int32.Parse(args[2]);

            if (args.Length == 4 && args[3] == "true")
            {
                arguments.IsInitiator = true;
            }

            return arguments;
        }

        private static void CreateListenerConnection(int listeningPort)
        {
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, listeningPort);

            _listener = new Socket(
                ipAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            _listener.Bind(localEndPoint);
            _listener.Listen(10); 
        }

        private static void CreateSenderConnection(int nextPort, string nextHost) 
        {   
            IPAddress ipAddress = (nextHost == "localhost") ? IPAddress.Loopback : IPAddress.Parse(nextHost);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, nextPort);

            _sender = new Socket(
                ipAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            var result = _sender.BeginConnect(remoteEP, null, null);

            if (!result.AsyncWaitHandle.WaitOne(1000, true))
            {   
                throw new Exception("Can not connect to socket");
            }                    
        } 

        private static void WorkAsInitiator()
        {
            string x = Console.ReadLine();

            _sender.Send(Encoding.UTF8.GetBytes(x));
        
            Socket handler = _listener.Accept();
            byte[] buf = new byte[4];           
            handler.Receive(buf);   

            string y = Encoding.UTF8.GetString(buf);

            _sender.Send(Encoding.UTF8.GetBytes(y));

            Console.WriteLine(y);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        private static void WorkAsOrdinaryProcess()
        {
            string x = Console.ReadLine();

            Socket handler = _listener.Accept(); 
            byte[] buf = new byte[4];
            handler.Receive(buf);

            string y = Encoding.UTF8.GetString(buf);

            _sender.Send(Encoding.UTF8.GetBytes(Math.Max(Convert.ToInt32(x), Convert.ToInt32(y)).ToString()));
        
            handler.Receive(buf);

            _sender.Send(buf);

            Console.WriteLine(Encoding.UTF8.GetString(buf));

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }
}