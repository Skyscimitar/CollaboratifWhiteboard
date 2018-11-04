using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Hostserver
{
    public class HostServer
    {
        private HostListener listener;

        public HostServer()
        {
            listener = new HostListener();
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
            foreach(Client c in ClientController.ClientList)
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