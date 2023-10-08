using System;

namespace Love_Letter
{
    class Program
    {
        public static bool Menu = false;
        //bool SP stands for singleplayer and is used to add bots to a game if a player enters 1 into the player count.
        public static bool SP = false;
        public static int playerCount;
        static void Main(string[] args)
        {

            Console.WriteLine("Welcome to James Wellers Love Letter game!\r\nWould you like to continue from your previous game or start a new one?\r\nPress 'P' to continue from your save or 'N to start a new game");
            string game;
            while (true)
            {
                game = Console.ReadLine();

                if (game == "N" || game == "n")
                {
                    while (Menu == false)
                    {
                        Console.WriteLine("Enter how many people you would like to play with (limit is 4)");
                        try
                        {
                            playerCount = int.Parse(Console.ReadLine());
                            // As the limit of people that can play this game is a maximum of 4 its important to make sure that we
                            // catch anything that is incorrect so that the applications doesn't crash, this also work with words as
                            // well.
                            if (playerCount > 4) { throw new Exception(); }
                            else if (playerCount == 1) { playerCount = 4; SP = true; }
                            Menu = true;
                        }
                        catch (Exception e) { Console.WriteLine("Invalid number of players!"); }
                    }

                    MainGame.Setup(playerCount, SP);
                    MainGame.Run();
                    break;
                }
                else if (game == "P" || game == "p")
                {
                    MainGame.SetUpXML();
                    MainGame.Run();
                    break;
                }
                else
                {
                    Console.WriteLine("That isnt a valid option");
                    continue;
                }
            }
        }


    }
}
