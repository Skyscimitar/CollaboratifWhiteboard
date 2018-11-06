using System;
using System.Text;
using System.Net.Sockets;

namespace Hostserver
{
    public class PacketReceiver
    {
        private byte[] _buffer;
        private readonly Socket _receiveSocket;
        private readonly int Id;

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
            try
            {
                if(_receiveSocket.EndReceive(ar)>1)
                {
                    _buffer = new byte[BitConverter.ToInt32(_buffer,0)];
                    _receiveSocket.Receive(_buffer, _buffer.Length, SocketFlags.None);
                    //everything is received, now we convert the data:
                    string data = Encoding.Default.GetString(_buffer);
                    // raise the received package data with appropriate context information
                    PackageReceivedEventArgs eventArgs = new PackageReceivedEventArgs{Data = data, Id = Id, Socket = _receiveSocket};
                    PackageReceivedHandler.OnReceivePackage(this, eventArgs);
                }
                else
                {
                    Disconnect();
                }
            }
            catch
            {
                if(!_receiveSocket.Connected)
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
            ClientController.RemoveClient(Id);
        }
    }
}