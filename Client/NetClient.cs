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

        public static Dictionary<ActionCodes, byte> Actions = new Dictionary<ActionCodes, byte>();

        public bool WaitingSomeData = false;

        public byte[] actionSent = new byte[1];

        public List<byte> ActionsQueue = new List<byte>();

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
            
            
            WaitingForData();
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

        public void WaitingForData() {
            while (WaitingSomeData) {

            }
        }

        public void RequestLoop()
        {
            
            new Thread(() => 
            {
                Thread.CurrentThread.IsBackground = true; 
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

            foreach (var bytou in data)
            {
                Console.Write(bytou + " ");
            }
            Console.Write("\n");
            
            
                
            if (ActionsQueue.Count == 0) {
                foreach (var oneByte in data) {
                    ActionsQueue.Add(oneByte);
                } 
                try {
                    PlayForVariable();
                } catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            } else {
                foreach (var oneByte in data) {
                    ActionsQueue.Add(oneByte);
                }
            }
        }
        public void PlayForVariable() {
            while (true) {
                if (ActionsQueue.Count == 0) return;
                DoActionFromByte(ActionsQueue[0]);
                ActionsQueue.RemoveAt(0);
            }
        }
        public void DoActionFromByte(byte oneByte) {
            if ( oneByte == Actions[ActionCodes.SetIdToOne]) {
                myID = 1;
            }
            if ( oneByte == Actions[ActionCodes.SetIdToTwo]) {
                myID = 2;
            }
            if ( oneByte == Actions[ActionCodes.SetplyrName]) {
                Game.SetName(myID);
                WaitingSomeData = false;
            }
            if ( oneByte == Actions[ActionCodes.PlyrChoice]) {
                byte PlyrChosen = Game.PlyrChoice(myID == 1 ? Game.plyrOneName : Game.plyrTwoName);
                SendByte(new byte[]{PlyrChosen, (byte)myID});
            }
        }

        private void SendByte(byte byby) {
            actionSent[0] = byby;
            clientSocket.Send(actionSent);
            Console.WriteLine($"SENDED : {byby}   " + clientSocket.RemoteEndPoint);
        }

        private void SendByte(byte[] byby) {
            clientSocket.Send(byby);
            Console.WriteLine($"SENDED : {byby}   " + clientSocket.RemoteEndPoint);
        }
    }

    public enum ActionCodes {
        SetIdToOne = 1,
        SetIdToTwo = 2,
        SetplyrName = 3,
        PlyrChoice = 4,
    } 
}