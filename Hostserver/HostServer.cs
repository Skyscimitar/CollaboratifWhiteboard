using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Hostserver
{
    public class HostServer
    {
        private HostListener listener;
        private ClientController clientController;

        public HostServer()
        {
            clientController = new ClientController();
            listener = new HostListener(clientController);
        }


        public void StartListening()
        {
            listener.StartNewWhiteboardListener();
        }

        public List<Client> getClientList()
        {
            return clientController.ClientList;
        }

        public void SendData(string data, Socket sendSocket)
        {
            PacketSender sender = new PacketSender(sendSocket);
            sender.Send(data);
        }

    }
}