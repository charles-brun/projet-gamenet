using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace GameNetServer
{


    public class Game 
    {
        public static Character PlayerOne;
        public static Character PlayerTwo; 
        public static Character PlyrRound = null;

        public static byte PlyrOneTypeCharacter = 0;
        public static byte PlyrTwoTypeCharacter = 0;

        public static int round = 1;
        public static string[] shieldASCII = File.ReadAllLines("Game/shieldimg.txt");
        public static string[] mageASCII = File.ReadAllLines("Game/mageimg.txt");
        public static string[] paladinASCII = File.ReadAllLines("Game/paladinimg.txt");
        public static string plyrOneName = "";
        public static string plyrTwoName = "";

        public static Server ? server {get; set; }

        public static void GameBegin () {


            Console.Clear();

            server = new Server();
            server.SetupServer();

            while (Server.AllPlayers.Count != 2 || plyrOneName == "" || plyrTwoName == "" || PlayerOne == null || PlayerTwo == null ) {
                Thread.Sleep(250);
                //Console.WriteLine("AllPlayers Count : " + Server.AllPlayers.Count + " plyrOneName : " + plyrOneName + " plyrTwoName : " + plyrTwoName);
            }
            
            
            Console.WriteLine("Two players Connected GameBegin");
        

            byte[] P1NByte = Encoding.ASCII.GetBytes(((byte)0).ToString() + plyrOneName);
            byte[] P2NByte = Encoding.ASCII.GetBytes(((byte)0).ToString() + plyrTwoName);


            server.clientSockets[0].SendTo(P2NByte, 0, P2NByte.Length, SocketFlags.None, server.clientSockets[0].LocalEndPoint);
            server.clientSockets[1].SendTo(P1NByte, 0, P1NByte.Length, SocketFlags.None, server.clientSockets[1].LocalEndPoint);

            GameLoop();



        }

        public static byte[] SendToPlyrChoseMove = new byte[] {5};
        public static byte[] SendToPlyrCharInfo;
        //public static List<byte> SendToPlyrCharInfoList = new List<byte>();

        public async static void GameLoop() {
            while (PlayerOne.Health > 0 && PlayerTwo.Health > 0)
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine(PlayerOne.ToString());
                Console.WriteLine(PlayerTwo.ToString());

                if (round == 1) { 
                    PlayerOne.Special();
                    PlyrRound = PlayerOne;
                    DisplayPlyr(PlayerOne, plyrOneName);
                } 
                else if (round == 2) {
                    PlayerTwo.Special();
                    PlyrRound = PlayerTwo;
                    DisplayPlyr(PlayerTwo, plyrTwoName);
                }
                byte PlyrRoundType = 0;
                if (PlyrRound is Warrior) {
                    PlyrRoundType = 1;
                }
                if (PlyrRound is Cleric) {
                    PlyrRoundType = 2;
                }
                if (PlyrRound is Paladin) {
                    PlyrRoundType = 3;
                }

                SendToPlyrCharInfo = new byte[]{5, PlyrOneTypeCharacter, (byte)PlayerOne.MaxHealth, (byte)PlayerOne.Health, (byte)PlayerOne.GetUniqueValue() , PlyrTwoTypeCharacter, (byte)PlayerTwo.MaxHealth, (byte)PlayerTwo.Health, (byte)PlayerTwo.GetUniqueValue()};
                if (round == 1) {
                    server.clientSockets[0].SendTo(SendToPlyrCharInfo, 0, SendToPlyrCharInfo.Length, SocketFlags.None, server.clientSockets[0].LocalEndPoint);
                } else if (round == 2) {
                    server.clientSockets[1].SendTo(SendToPlyrCharInfo, 0, SendToPlyrCharInfo.Length, SocketFlags.None, server.clientSockets[1].LocalEndPoint);
                    
                }

                while (!PlyrHasChoseMove) {
                    
                }

                PlyrChoseMove();

                SendToPlyrCharInfo = new byte[]{6, PlyrOneTypeCharacter, (byte)PlayerOne.MaxHealth, (byte)PlayerOne.Health, (byte)PlayerOne.GetUniqueValue() , PlyrTwoTypeCharacter, (byte)PlayerTwo.MaxHealth, (byte)PlayerTwo.Health, (byte)PlayerTwo.GetUniqueValue()};
                if (round == 1) {
                    server.clientSockets[0].SendTo(SendToPlyrCharInfo, 0, SendToPlyrCharInfo.Length, SocketFlags.None, server.clientSockets[0].LocalEndPoint);
                } else if (round == 2) {
                    server.clientSockets[1].SendTo(SendToPlyrCharInfo, 0, SendToPlyrCharInfo.Length, SocketFlags.None, server.clientSockets[1].LocalEndPoint);
                    
                }

                if (round == 1) 
                { 
                    round = 2;
                }
                else if (round == 2) 
                {
                    round = 1;
                }

                Console.WriteLine(PlayerOne.Health+ " " + PlayerTwo.Health);
            }
            byte[] sendWinner = new byte[1];
            if (PlayerOne.Health == 0 && PlayerTwo.Health > 0) {
                Console.WriteLine(plyrTwoName + " WINS !");
                sendWinner[0] = 8;
            } else if(PlayerOne.Health > 0 && PlayerTwo.Health == 0) {
                Console.WriteLine(plyrOneName + " WINS !");
                sendWinner[0] = 7;
            } else if (PlayerOne.Health == 0 && PlayerTwo.Health == 0) {
                Console.WriteLine("DRAW !");
                sendWinner[0] = 9;
            }
            server.clientSockets[0].SendTo(sendWinner, 0, sendWinner.Length, SocketFlags.None, server.clientSockets[0].LocalEndPoint);
            server.clientSockets[1].SendTo(sendWinner, 0, sendWinner.Length, SocketFlags.None, server.clientSockets[1].LocalEndPoint);

            Console.WriteLine("Game ended !");
            server.CloseAllSockets();
        }

        public static int PlyrMoveInput = -1;
        public static int CibleMovePlyr = -1;
        public static bool PlyrHasChoseMove = false;

        public static void PlyrChoseMove() {

            if (round == 1) 
            {
                if (PlyrMoveInput == 1) {
                    if (CibleMovePlyr == 1) {
                        PlayerOne.CibledSpecial(PlayerOne);
                    }
                    else if(CibleMovePlyr == 2) {
                        PlayerOne.CibledSpecial(PlayerTwo);
                    }
                }
                else if(PlyrMoveInput == 2) {
                    if (CibleMovePlyr == 1) {
                        PlayerOne.AlternatifAtk(PlayerOne);
                    }
                    else if(CibleMovePlyr == 2) {
                        PlayerOne.AlternatifAtk(PlayerTwo);
                    }
                }

            } else if (round == 2) 
            {
                if (PlyrMoveInput == 1) {
                    if (CibleMovePlyr == 1) {
                        PlayerTwo.CibledSpecial(PlayerOne);
                    }
                    else if(CibleMovePlyr == 2) {
                        PlayerTwo.CibledSpecial(PlayerTwo);
                    }
                }
                else if(PlyrMoveInput == 2) {
                    if (CibleMovePlyr == 1) {
                        PlayerTwo.AlternatifAtk(PlayerOne);
                    }
                    else if(CibleMovePlyr == 2) {
                        PlayerTwo.AlternatifAtk(PlayerTwo);
                    }
                }
            }

            PlyrHasChoseMove = false;
            return;
        }

        public static int choiseOneOrTwo() {
            string PlyrInput = "";
            PlyrInput = Console.ReadLine();
            PlyrInput = PlyrInput != null ? PlyrInput : PlyrInput = "";
            if (PlyrInput == "1") {
                return 1;
            }
            else if(PlyrInput == "2") {
                return 2;
            } else {
                return choiseOneOrTwo();
            }
        }

        public static void PlyrChoice(byte PlyrChosen, byte plyrID) {
            string PlyrName = plyrID == 1 ? plyrOneName : plyrTwoName;
            if (PlyrChosen == 1) {
                if (plyrID == 1) {
                    PlayerOne = new Warrior(PlyrName, 200);
                    PlyrOneTypeCharacter = 1;
                }
                if (plyrID == 2) {
                    PlayerTwo = new Warrior(PlyrName, 200);
                    PlyrTwoTypeCharacter = 1;
                }
            }
            else if(PlyrChosen == 2) {
                if (plyrID == 1) {
                    PlayerOne = new Cleric(PlyrName, 200);
                    PlyrOneTypeCharacter = 2;
                }
                if (plyrID == 2) {
                    PlayerTwo = new Cleric(PlyrName, 200);
                    PlyrTwoTypeCharacter = 2;
                }
            }
            else if(PlyrChosen == 3) {                
                if (plyrID == 1) {
                    PlayerOne = new Paladin(PlyrName, 200);
                    PlyrOneTypeCharacter = 3;
                }
                if (plyrID == 2) {
                    PlayerTwo = new Paladin(PlyrName, 200);
                    PlyrTwoTypeCharacter = 3;
                }
            }

            Console.WriteLine(PlyrName + "  " + (PlayerOne != null ? PlayerOne.ToString() : "") + "  " + (PlayerTwo != null ? PlayerTwo.ToString() : ""));

        }

        public static void DisplayPlyr(Character plyrToDisplay, string PlyrName) {
            Console.WriteLine(PlyrName);
        }
    }
    
}