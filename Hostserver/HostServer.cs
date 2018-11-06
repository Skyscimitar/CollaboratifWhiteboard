using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Hostserver
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
            PackageReceivedHandler.OnReceivePackage += ReceivePackage;
            while (true)
            {
            }
        }

        public List<Client> GetClientList()
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