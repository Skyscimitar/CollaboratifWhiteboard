using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

namespace Hostserver
{
    public class HostListener
    {
        public Socket ListenerSocket{get; private set; }
        public short Port = 8080;

        public HostListener()
        {
            ListenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void StartNewWhiteboardListener()
        {
            Debug.WriteLine("Server:Started Listening on port {0}, protocol: {1}", Port, ProtocolType.Tcp);
            ListenerSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            ListenerSocket.Listen(10);
            ListenerSocket.BeginAccept(AcceptNewConnection, ListenerSocket);
        }

        private void AcceptNewConnection(IAsyncResult ar)
        {
         try 
         {
             Debug.WriteLine("Server:Accepted Connection on port {0}, protocol: {1}", Port, ProtocolType.Tcp);
             Socket NewConnectionSocket = ListenerSocket.EndAccept(ar);
             ClientController.AddClient(NewConnectionSocket);
         }
         catch (Exception e)
         {
             Debug.WriteLine(e.ToString());
         } 
        }
    }
}
