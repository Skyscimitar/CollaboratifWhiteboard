using System;
using System.Collections.Generic;
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

    }
}