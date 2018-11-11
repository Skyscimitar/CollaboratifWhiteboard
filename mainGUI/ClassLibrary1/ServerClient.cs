using System.Net.Sockets;

namespace Network
{
    public class ServerClient
    {
        public Socket Socket { get; }
        public int Id { get; }
        private readonly PacketReceiver receiver;

        public ServerClient(Socket socket, int id)
        {
            Socket = socket;
            Id = id;
            receiver = new PacketReceiver(socket, id);
            receiver.StartReceiving();
        }
    }
}
