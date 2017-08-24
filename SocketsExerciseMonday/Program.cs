using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SocketsExerciseMonday
{
    class Program
    {
        Socket s;
        public bool Startup(IPAddress ip, int port)
        {
            try
            {
                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.Bind(new IPEndPoint(ip, port));
                s.Listen(10);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            for (;;)
            {
                Console.WriteLine("ready");
                Socket newSocket = s.Accept();
                Communicate(newSocket);
            }
        }

        void Communicate(Socket clSock)
        {
            try
            {
                IPEndPoint remoteIPEndPoint = clSock.RemoteEndPoint as IPEndPoint;
                IPEndPoint localIPEndPoint = clSock.LocalEndPoint as IPEndPoint;
                
                byte[] buffer = new byte[1024];
                while (clSock.Receive(buffer) > 0)
                {
                    string str = Encoding.ASCII.GetString(buffer);
                    int i = str.IndexOf('\0');
                    if (i >= 0)
                    {
                        str = str.Substring(0, i);
                    }
                    if (str.Equals("time"))
                    {
                        DateTime dt = DateTime.Now;
                        byte[] msg = Encoding.ASCII.GetBytes(String.Format("{0:HH:mm:ss}", dt));
                        clSock.Send(msg);
                    }
                    else if(str.Equals("date"))
                    {
                        DateTime dt = DateTime.Now;
                        byte[] msg = Encoding.ASCII.GetBytes(String.Format("{0:d/M/yyyy}", dt));
                        clSock.Send(msg);
                    }
                    Console.WriteLine("IPAddress " + localIPEndPoint.Address + "Port " + localIPEndPoint.Port);
                }
                clSock.Shutdown(SocketShutdown.Both);
                clSock.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void Main(string[] args)
        {
            Program server = new Program();
            if (!server.Startup(IPAddress.Loopback, 50000))
                Console.WriteLine("Starting echo server failed");
        }
    }
}
