using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace Hostserver
{
    public static class ClientController
    {
        public static List<Client> ClientList = new List<Client>();

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