using System;
using System.Net.Sockets;

namespace Hostserver
{
    public class PackageReceivedEventArgs
    {
        public string data {get; set; }
        public Socket socket {get; set; }
        public int id{get; set; }
    }
}