using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

internal sealed class Program
{
    public int myID;
    private const int Port = 7777;
    private readonly Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private bool hasLoggedin;
    private int dataSent;

    public static void Main()
    {
        var client = new Program();
        client.ConnectToServer();
        client.Exit();
    }

    private void ConnectToServer()
    {
        int attempts = 0;

        while (!clientSocket.Connected)
        {
            try
            {
                attempts++;
                string ip = "127.0.0.1";
                Console.WriteLine("Port[default]: " + Port);
                Console.WriteLine("Connection attempt to " + ip + ": " + attempts + " attempts");
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

        Console.WriteLine("Connected!");
        SendLoginPacket();
    }

    private void SendLoginPacket()
    {
        if (hasLoggedin == false)
        {
            SendString("NewUserConnected");
            hasLoggedin = true;
        }

        RequestLoop();
    }

    private void RequestLoop()
    {
        while (true)
        {
            ReceiveResponse();
        }
    }

    private void Exit()
    {
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
    }

    private void SendString(string text)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(text);
        Console.WriteLine("Sent: " + text);
        clientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        dataSent++;
        Console.WriteLine(dataSent);
    }

    private void ReceiveResponse()
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


            Console.WriteLine(text);
            string userNameInput = "";
            while (userNameInput == "" || userNameInput == null)
            {
                userNameInput = Console.ReadLine();
            }
            SendString("{" + myID + "} is [" + userNameInput + "]");
        }

        Console.WriteLine("Clients connected.");
    }
}