using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GameNetClient
{

        class Program
        {
            public static void Main(string[] args)
            {
                string serverIP ="";
                if (Environment.GetCommandLineArgs().Count()<=1){
                    string serverIPfile = "serverIP.txt";
                    if (File.Exists(serverIPfile)){
                        serverIP = File.ReadAllText(serverIPfile);
                    } else {
                        Console.WriteLine("Please specify your server IP");
                        Environment.Exit(0);
                    }
                } else {
                    serverIP = Environment.GetCommandLineArgs()[1];
                }
                int counter = 1;
                foreach (ActionCodes oneAction in Enum.GetValues(typeof(ActionCodes)) ) {
                    NetClient.Actions.Add(oneAction, (byte)counter);
                    counter++;
                }
                NetClient.Actions[ActionCodes.GetOtherName] = 48;
                try{
                    Game.GameBegin(serverIP);
                } catch {
                    Console.WriteLine("Invalid IP");
                    Environment.Exit(0);
                }
            }
        }
    
}

