using System.Net.Sockets;
using System.Net;

namespace projet_gamenet
{
    public class Client
    {
        public static int dataBufferSize = 4096;
        public int id;
        public static string ip = "127.0.0.1";
        public static int ServerPort = 26950;
        public TCP tcp;

        public Client(int _clientId){
            id = _clientId;
            tcp = new TCP(id);
        }

        public void ConnectToServer() {
            tcp.Connect(tcp.socket);
        }

        public class TCP
        {
            public TcpClient socket = new TcpClient();
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

                socket.BeginConnect(Client.ip, Client.ServerPort, ConnectCallback, socket);
                
            }

            private void ConnectCallback(IAsyncResult _result)
            {
                socket.EndConnect(_result);
                if (!socket.Connected)
                {
                    return;
                }

                stream = socket.GetStream();

                receiveBuffer = new byte[dataBufferSize];

                // TODO : send welcome packet

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
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