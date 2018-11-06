using System;
using System.Net.Sockets;

namespace Hostserver
{
    public class PackageReceivedEventArgs : EventArgs
    {
        public string Data {get; set; }
        public Socket Socket {get; set; }
        public int Id {get; set; }
    }
}