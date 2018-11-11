using System.Collections.Generic;
using System.Net.Sockets;

namespace Network
{
    public static class ServerClientController
    {
        public static List<ServerClient> ClientList { get; } = new List<ServerClient>();

        public static void AddClient(Socket socket)
        {
            ClientList.Add(new ServerClient(socket, ClientList.Count));
        }

        public static void RemoveClient(int id)
        {
            ClientList.RemoveAt(ClientList.FindIndex(x => x.Id == id));
        }
    }
}
