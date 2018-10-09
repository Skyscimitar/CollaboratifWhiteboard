using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace WhiteboardClient
{
    public class PacketReceiver
    {
        private byte[] _buffer;
        private Socket _receiveSocket;

        public PacketReceiver(Socket socket)
        {
            _receiveSocket = socket;
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
                Console.WriteLine("Exception starting to receive: {0}", e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if(_receiveSocket.EndReceive(ar)>1)
                {
                    _buffer = new byte[BitConverter.ToInt32(_buffer, 0)];
                    _receiveSocket.Receive(_buffer, _buffer.Length, SocketFlags.None);
                    string data = Encoding.Default.GetString(_buffer);
                    //the following line is for debugging only, can be removed after testing
                    Console.WriteLine(data);
                    //raise an event when a package is received
                    PacketReceivedEventArgs eventArgs = new PacketReceivedEventArgs{data = data, socket = _receiveSocket};
                    PacketReceivedEventHandler.OnReceivePackage(this, eventArgs);
                }
                else
                {
                    Disconnect();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                if (!_receiveSocket.Connected)
                    Disconnect();
                else
                    StartReceiving();
            }
        }

        public void Disconnect()
        {
            _receiveSocket.Disconnect(true);
        }
    }
}