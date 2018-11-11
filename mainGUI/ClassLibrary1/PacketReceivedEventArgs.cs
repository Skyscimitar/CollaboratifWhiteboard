using System;
using System.Net.Sockets;

namespace Network
{
    public class PacketReceivedEventArgs : EventArgs
    {
        public string Data { get; set; }
        public Socket Socket { get; set; }
        public int Id { get; set; }
    }
}
