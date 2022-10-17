using System;

namespace projet_gamenet{
    class Program {
        static void Main(string[] args){
            Console.Title = "Game Server";

            Server.Start(2, 26950);

            Console.ReadKey();
        }
    }
}
