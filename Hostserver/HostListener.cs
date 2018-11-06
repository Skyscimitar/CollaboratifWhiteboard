using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hostserver
{
    public class HostListener
    {
        public Socket ListenerSocket{get; private set; }
        public short Port = 8080;
        private readonly float width;
        private readonly float height;

        public HostListener(double size_x, double size_y)
        {
            width = (float)size_x;
            height = (float)size_y;
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
                ListenerSocket.BeginAccept(AcceptNewConnection, ListenerSocket);
                SendSize(NewConnectionSocket);
                RestoreWhiteboard(ClientController.ClientList[-1]);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            } 
        }

        private void SendSize(Socket socket)
        {
            PacketSender sender = new PacketSender(socket);
            JObject json = new JObject(new JProperty("type", "SIZE"),
                      new JProperty("content", new JObject(
                          new JProperty("width", width),
                          new JProperty("height", height))));
            sender.Send(json.ToString());
        }

        private void RestoreWhiteboard(Client client)
        {
            int id = client.Id;
            PacketSender sender = new PacketSender(ClientController.ClientList[0].Socket);
            JObject json = new JObject(new JProperty("type", "REQUEST_STATUS"),
                new JProperty("content", new JObject(new JProperty("id", id))));
            sender.Send(json.ToString());
        }
    }
}
