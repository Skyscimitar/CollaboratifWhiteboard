using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
namespace WhiteboardClient
{
    public class PacketSender
    {
        private Socket _sendSocket;

        public PacketSender(Socket socket)
        {
            _sendSocket = socket;
        }

        public void Send(string data)
        {
            Debug.WriteLine("Sending to server:");
            Debug.WriteLine(data);
            try
            {
                var fullPacket = new List<byte>();
                fullPacket.AddRange(BitConverter.GetBytes(data.Length));
                fullPacket.AddRange(Encoding.Default.GetBytes(data));
                _sendSocket.Send(fullPacket.ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine("PacketSendException: {0}", e);
            }
        }
    }
}