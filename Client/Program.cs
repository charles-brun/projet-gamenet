using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GameNetClient
{

        class Program
        {
            public static void Main()
            {
                int counter = 1;
                foreach (ActionCodes oneAction in Enum.GetValues(typeof(ActionCodes)) ) {
                    NetClient.Actions.Add(oneAction, (byte)counter);
                    counter++;
                }
                Game.GameBegin();
                
            }
        }
    
}

