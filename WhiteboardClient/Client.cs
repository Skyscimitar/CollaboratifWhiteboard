using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Sockets;

namespace WhiteboardClient
{
    public class Client
    {
        public Socket socket {get; set; }
        public PacketReceiver receiver;

        public Client(Socket socket)
        {
            this.socket = socket;
            receiver = new PacketReceiver(socket);
            receiver.StartReceiving();
        }

        public void Disconnect()
        {
            //ends the connection (basically kills the client)
            receiver.Disconnect();
        }
    }
}