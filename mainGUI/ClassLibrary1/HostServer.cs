using System;
using System.Net.Sockets;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Network
{
    public class HostServer
    {
        public HostListener Listener { get; }

        public HostServer(double size_x, double size_y)
        {
            Listener = new HostListener(size_x, size_y);
        }


        public void StartListening()
        {
            Listener.StartNewWhiteboardListener();
            PacketReceivedEventHandler.OnServerReceivePacket += ReceivePacket;
            while (true)
            {
            }
        }

        public List<ServerClient> GetClientList()
        {
            return ServerClientController.ClientList;
        }

        public void SendData(string data, Socket sendSocket)
        {
            PacketSender sender = new PacketSender(sendSocket);
            sender.Send(data);
        }

        public static void ReceivePacket(Object o, PacketReceivedEventArgs args)
        {
            Dictionary<string, object> pdict = JsonConvert.DeserializeObject<Dictionary<string, object>>(args.Data);
            if ((string)pdict["type"] == "REQUEST_STATUS")
            {
                //if the client is requesting the current state of the whiteboard
                ServerClient c = ServerClientController.ClientList[0]; //the "host" client
                PacketSender sender = new PacketSender(c.Socket);
                sender.Send(args.Data);
            }
            else if ((string)pdict["type"] == "RESTORE")
            {
                //transfer the current state of the whiteboard to the client who
                //requested it
                int id = int.Parse(pdict["client_id"].ToString());
                ServerClient c = ServerClientController.ClientList[id];
                PacketSender sender = new PacketSender(c.Socket);
                sender.Send(args.Data);
            }
            else
            {
                //if a new shape is drawn, we simply need to transfer it to the clients
                foreach (ServerClient c in ServerClientController.ClientList)
                {
                    if (c.Id != args.Id)
                    {
                        PacketSender sender = new PacketSender(c.Socket);
                        sender.Send(args.Data);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
    }
}
