using System;
using System.Text;
using System.Net.Sockets;

namespace Network
{
    /// <summary>
    /// Packet receiver class, handles receiving data for the server and the client
    /// </summary>
    public class PacketReceiver
    {
        private byte[] _buffer;
        private readonly Socket _receiveSocket;
        private readonly int Id;
        private readonly bool IsServer;

        public PacketReceiver(Socket Socket, int Id)
        {
            _receiveSocket = Socket;
            this.Id = Id;
            IsServer = true;
        }

        public PacketReceiver(Socket Socket)
        {
            _receiveSocket = Socket;
            IsServer = false;
        }

        /// <summary>
        /// sets up the socket to receive information
        /// </summary>
        public void StartReceiving()
        {
            try
            {
                _buffer = new byte[4];
                _receiveSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        /// <summary>
        /// Callback triggered when the socket receives data
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (_receiveSocket.EndReceive(ar) > 1)
                {
                    _buffer = new byte[BitConverter.ToInt32(_buffer, 0)];
                    _receiveSocket.Receive(_buffer, _buffer.Length, SocketFlags.None);
                    //everything is received, now we convert the data:
                    string data = Encoding.Default.GetString(_buffer);
                    // raise the received package data with appropriate context information
                    PacketReceivedEventArgs eventArgs = new PacketReceivedEventArgs { Data = data, Id = Id, Socket = _receiveSocket };
                    if (IsServer)
                    {
                        PacketReceivedEventHandler.OnServerReceivePacket(this, eventArgs);
                    }
                    else
                    {
                        PacketReceivedEventHandler.OnClientReceivePacket(this, eventArgs);
                    }
                }
                else
                {
                    Disconnect();
                }
            }
            catch
            {
                if (!_receiveSocket.Connected)
                {
                    Disconnect();
                }
                else
                    StartReceiving();
            }
            StartReceiving();
        }

        private void Disconnect()
        {
            _receiveSocket.Disconnect(true);
        }
    }
}
