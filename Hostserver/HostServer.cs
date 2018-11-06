using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hostserver
{
    public class HostServer
    {
        public HostListener listener { get; }

        public HostServer(double size_x, double size_y)
        {
            listener = new HostListener(size_x, size_y);
        }


        public void StartListening()
        {
            listener.StartNewWhiteboardListener();
            PackageReceivedHandler.OnReceivePackage += ReceivePackage;
            while (true)
            {
            }
        }

        public List<Client> getClientList()
        {
            return ClientController.ClientList;
        }

        public void SendData(string data, Socket sendSocket)
        {
            PacketSender sender = new PacketSender(sendSocket);
            sender.Send(data);
        }

        public static void ReceivePackage(Object o, PackageReceivedEventArgs args)
        {
            Dictionary<string, object> pdict = JsonConvert.DeserializeObject<Dictionary<string, object>>(args.data);
            if ((string)pdict["type"] == "REQUEST_STATUS")
            {
                //if the client is requesting the current state of the whiteboard
                Client c = ClientController.ClientList[0]; //the "host" client
                PacketSender sender = new PacketSender(c.Socket);
                sender.Send(args.data);
            }
            else if ((string)pdict["type"] == "RESTORE")
            {
                //transfer the current state of the whiteboard to the client who
                //requested it
                int id = int.Parse(pdict["client_id"].ToString());
                Client c = ClientController.ClientList[id];
                PacketSender sender = new PacketSender(c.Socket);
                sender.Send(args.data);
            }
            else
            {
                //if a new shape is drawn, we simply need to transfer it to the clients
                foreach (Client c in ClientController.ClientList)
                {
                    if (c.Id != args.id)
                    {
                        PacketSender sender = new PacketSender(c.Socket);
                        sender.Send(args.data);
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