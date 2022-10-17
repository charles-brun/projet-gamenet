using System;

namespace projet_gamenet{
    class Program {
        static void Main(string[] args){
            Console.Title = "Game Server";

            Client client = new Client(1);
            client.ConnectToServer();

            Console.ReadKey();
        }
    }
}
