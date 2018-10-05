using System;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Hostserver
{
    public class Client
    {
        public Socket Socket {get; set;}
        public int Id {get; private set;}

        //add package receiver and package senders

        public Client(Socket socket, int id)
        {
            Socket = socket;
            Id = id;
        }
    }
}