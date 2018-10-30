using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteboardClient
{
    class DiscoverHostException : Exception
    {
        public DiscoverHostException() { }
        public DiscoverHostException(string message):base(message){}
    }
}
