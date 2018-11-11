using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Network
{
    /// <summary>
    /// Handle incoming connections, handles transferring packets to the correct client
    /// </summary>
    public class HostListener
    {
        public Socket ListenerSocket { get; }
        public const short Port = 8080;
        private readonly float width;
        private readonly float height;

        public HostListener(double size_x, double size_y)
        {
            width = (float)size_x;
            height = (float)size_y;
            ListenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// start the listener -> begin accepting connections from other instances of the application
        /// </summary>
        public void StartNewWhiteboardListener()
        {
            Debug.WriteLine("Server:Started Listening on port {0}, protocol: {1}", Port, ProtocolType.Tcp);
            ListenerSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            ListenerSocket.Listen(10);
            ListenerSocket.BeginAccept(AcceptNewConnection, ListenerSocket);
        }

        /// <summary>
        /// AsyncCallback triggered when a new client connects. Adds the client to the client list and sets it up.
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptNewConnection(IAsyncResult ar)
        {
            try
            {
                Debug.WriteLine("Server:Accepted Connection on port {0}, protocol: {1}", Port, ProtocolType.Tcp);
                Socket NewConnectionSocket = ListenerSocket.EndAccept(ar);
                ServerClientController.AddClient(NewConnectionSocket);
                ListenerSocket.BeginAccept(AcceptNewConnection, ListenerSocket);
                SendSize(NewConnectionSocket);
                if (ServerClientController.ClientList.Count > 1)
                {
                    RestoreWhiteboard(ServerClientController.ClientList[ServerClientController.ClientList.Count - 1]);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Send the size of the whiteboard to a new client so the UI can correctly rescale the drawings
        /// </summary>
        /// <param name="socket"></param>
        private void SendSize(Socket socket)
        {
            PacketSender sender = new PacketSender(socket);
            JObject json = new JObject(new JProperty("type", "SIZE"),
                      new JProperty("content", new JObject(
                          new JProperty("width", width),
                          new JProperty("height", height))));
            sender.Send(json.ToString());
        }

        /// <summary>
        /// Initiates the whiteboard restoration process so a new client receives the current status of the whiteboard
        /// when connecting
        /// </summary>
        /// <param name="client"></param>
        private void RestoreWhiteboard(ServerClient client)
        {
            int id = client.Id;
            PacketSender sender = new PacketSender(ServerClientController.ClientList[0].Socket);
            JObject json = new JObject(new JProperty("type", "REQUEST_STATUS"),
                new JProperty("content", new JObject(new JProperty("id", id.ToString()))));
            sender.Send(json.ToString());
        }
    }
}
