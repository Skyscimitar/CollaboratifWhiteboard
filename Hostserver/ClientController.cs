using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace Hostserver
{
    public class ClientController
    {
        public List<Client> ClientList = new List<Client>();

        public void AddClient(Socket socket)
        {
            ClientList.Add(new Client(socket, ClientList.Count));
        }

        public void RemoveClient(int id)
        {
            ClientList.RemoveAt(ClientList.FindIndex(x => x.Id == id));
        }
    }
}