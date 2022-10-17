using System.Net.Sockets;
using System.Net;

namespace projet_gamenet
{
    public class Client
    {
        public static int dataBufferSize = 4096;
        public int id;
        public TCP tcp;

        public Client(int _clientId){
            id = _clientId;
            tcp = new TCP(id);
        }

        public class TCP
        {
            public TcpClient socket;
            private readonly int id;
            private NetworkStream stream;
            private byte[] receiveBuffer;

            public TCP(int _id){
                id = _id;
            }

            public void Connect(TcpClient _socket){
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();

                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                // TODO : send welcome packet
            }

            private void ReceiveCallback(IAsyncResult _result){
                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if (_byteLength <= 0){
                        // TODO : Disconnect
                        return;
                    }
                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);

                    // TODO : handle data
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception _ex)
                {
                    Console.WriteLine($"Error receiving TCP data : {_ex}");
                    // TODO : Disconnect
                }
            }
        }
    }
}