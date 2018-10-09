using System;

namespace Hostserver
{
    public static class PackageReceivedHandler
    {
        public static EventHandler<PackageReceivedEventArgs> OnReceivePackage;
    }
}