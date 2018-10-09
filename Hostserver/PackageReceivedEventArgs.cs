using System;
using System.Linq;
using System.Net.Sockets;

namespace Hostserver
{
    public class PackageReceivedEventArgs : EventArgs
    {
        public string data {get; set; }
        public Socket socket {get; set; }
        public int id {get; set; }
    }
}