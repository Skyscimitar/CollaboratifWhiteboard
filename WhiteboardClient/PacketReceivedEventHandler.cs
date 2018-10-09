using System;

namespace WhiteboardClient
{
    public static class PacketReceivedEventHandler
    {
        public static EventHandler<PacketReceivedEventArgs> OnReceivePackage;
    }
}