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
                NetClient.Actions.Add(choseafaire.Name, 1);
                Game.GameBegin();
                
            }
        }
    
}

