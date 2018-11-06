using System.Collections.Generic;
using System.Net.Sockets;


namespace Hostserver
{
    public static class ClientController
    {
        public static List<Client> ClientList { get; } = new List<Client>();

        public static void AddClient(Socket socket)
        {
            ClientList.Add(new Client(socket, ClientList.Count));
        }

        public static void RemoveClient(int id)
        {
            ClientList.RemoveAt(ClientList.FindIndex(x => x.Id == id));
        }
    }
}