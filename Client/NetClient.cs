using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;


namespace GameNetClient {
    internal sealed class NetClient
    {
        public int myID;
        public const int Port = 7777;
        public readonly Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public bool hasLoggedin;
        //public int dataSent;

        public bool WaitingSomeData = false;

        public void ConnectToServer()
        {
            int attempts = 0;

            while (!clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    string ip = "127.0.0.1";
                    Console.WriteLine("Connection attempt to " + ip + ":"  + Port + "...");
                    var address = IPAddress.Parse(ip);
                    clientSocket.Connect(address, Port);
                }
                catch (SocketException e)
                {
                    Console.Clear();
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }
            SendLoginPacket();
            Console.WriteLine("Connected!");
            while (WaitingSomeData) {

            }
            return;
        }

        public void SendLoginPacket()
        {
            if (hasLoggedin == false)
            {
                SendString("NewUserConnected");
                hasLoggedin = true;
            }

            WaitingSomeData = true;
            RequestLoop();
        }

        public void RequestLoop()
        {
            
            new Thread(() => 
            {
                Thread.CurrentThread.IsBackground = true; 
                /* run your code here */ 
                while (true)
                {
                    ReceiveResponse();
                }
            }).Start();


            
        }

        public void Exit()
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        public void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            Console.WriteLine("Sent To Server : " + text);
            clientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
            //dataSent++;
            //Console.WriteLine(dataSent);
        }

        public void ReceiveResponse()
        {
            var buffer = new byte[2048];
            int received = clientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0)
            {
                return;
            }

            var data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.ASCII.GetString(data);

            if (text.Contains("Your Id :"))
            {
                string idtextf = text.Replace("Your Id : ", "");
                idtextf = idtextf.Replace(" what's your name ?", "");
                myID = int.Parse(idtextf);


                Console.WriteLine("what's your name ?");
                string userNameInput = "";
                while (userNameInput == "" || userNameInput == null)
                {
                    userNameInput = Console.ReadLine();
                }
                SendString("{" + myID + "} is [" + userNameInput + "]");
                WaitingSomeData = false;
            }
        }
    }
}