using System.Net.Sockets;

namespace Hostserver
{
    public class Client
    {
        public Socket Socket {get; }
        public int Id {get; }
        private readonly PacketReceiver receiver;

        //add package receiver and package senders

        public Client(Socket socket, int id)
        {
            Socket = socket;
            Id = id;
            receiver = new PacketReceiver(socket, id);
            receiver.StartReceiving();
        }
    }
}