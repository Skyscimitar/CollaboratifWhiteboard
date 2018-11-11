using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public static class PacketReceivedEventHandler
    {
        public static EventHandler<PacketReceivedEventArgs> OnServerReceivePacket;
        public static EventHandler<PacketReceivedEventArgs> OnClientReceivePacket;
    }
}
