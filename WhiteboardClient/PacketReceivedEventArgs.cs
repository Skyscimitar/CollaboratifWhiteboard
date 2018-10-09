using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace WhiteboardClient
{
    public class PacketReceivedEventArgs : EventArgs
    {
        public string data {get; set; }
        public Socket socket {get; set; }
        public int id {get; set; }
    }
}