using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace WhiteboardClient
{
    public class Connector
    {
        public Client client {get; set;}
        private Socket _connectingSocket {get; set; }
        
        public void TryConnect(string ipaddress)
        {
            _connectingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            while (!_connectingSocket.Connected)
            {
                Thread.Sleep(1000);
                try
                {
                    _connectingSocket.Connect(new IPEndPoint(IPAddress.Parse(ipaddress), 8080));
                }
                catch(Exception e)
                {
                    Console.WriteLine("Connection exception: {0}", e.ToString());
                }
            }
            SetUpForListening();
        }

        private void SetUpForListening()
        {
            client = new Client(_connectingSocket);
        }

        public bool Isconnected{get {return _connectingSocket.Connected;}}
    }
}