using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;

namespace Hostserver
{
    public class PacketReceiver
    {
        private byte[] _buffer;
        private Socket _receiveSocket;
        private int Id {get; set;}

        public PacketReceiver(Socket socket, int Id)
        {
            _receiveSocket = socket;
            this.Id = Id;
        }

        public void StartReceiving()
        {
            try
            {
                _buffer = new byte[4];
                _receiveSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {

        }

        private void Disconnect()
        {

        }
    }
}