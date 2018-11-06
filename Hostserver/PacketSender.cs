using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Hostserver
{
    public class PacketSender
    {
        private readonly Socket _sendSocket;

        public PacketSender(Socket socket)
        {
            _sendSocket = socket;
        }

        public void Send(string data)
        {
            try
            {
                var fullPacket = new List<byte>();
                fullPacket.AddRange(BitConverter.GetBytes(data.Length));
                fullPacket.AddRange(Encoding.Default.GetBytes(data));
                _sendSocket.Send(fullPacket.ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine("Packet Send exception: {0}", e);
            }
        }

    }
}