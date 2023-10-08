using System;
using System.Collections.Generic;
using System.Text;

namespace Love_Letter
{
    public static class MainGame
    {
        private static bool SP;
        public static Player[] playerArray { get; set; }
        public static int playerCount { get; set; }
        public static int tokensToWin;
        public static List<Card> gameDeck { get; set; }
        public static int roundNumber = 1;

        public static void Setup(int playerCount, bool singlePlayer)
        {
            MainGame.SP = singlePlayer;
            MainGame.playerCount = playerCount;
            playerArray = new Player[playerCount];
            for(int i = 0; i < playerCount; i++)
            {
                //SP is used as a filter to check if a person is playing on their own, if they are then they will be put
                //in a game with virtual players (robots).
                if(SP == true)
                {
                    Console.WriteLine("Player please enter your name");
                    playerArray[0] = new Human(Console.ReadLine());
                    for (i = 1;  i < playerCount; i++) { playerArray[i] = new Robot("Bot " + i); }
                }
                else if(SP == false)
                {
                    int currentP = i + 1;
                    Console.WriteLine("Player " + currentP + " please enter your name");
                    playerArray[i] = new Human(Console.ReadLine());
                }
                Console.WriteLine();
            }
            //This sets the token limit dependent on the numbers of players.
            if (playerArray.Length == 2) { tokensToWin = 7; }
            else if (playerArray.Length == 3) { tokensToWin = 5; }
            else if (playerArray.Length == 4) { tokensToWin = 4; }
        }

        public static void SetUpXML()
        {
            XML.LoadGame();
            SP = XML.SinglePlayerCheck();
            //This sets the token limit dependent on the numbers of players.
            if (playerArray.Length == 2) { tokensToWin = 7; }
            else if (playerArray.Length == 3) { tokensToWin = 5; }
            else if (playerArray.Length == 4) { tokensToWin = 4; }

            
        }
        public static void Run()
        {
            if (SP == true) { Console.WriteLine("Starting game with " + playerArray[0].viewName() + " and 3 bots\r\n"); }
            else if (SP == false)
            {
                Console.WriteLine("Starting game with the following players:");
                //Although not necessary I felt like I should note everyones names down before the game starts so they
                //can make sure everything is correct.
                for (int i = 0; i < playerArray.Length; i++)
                {
                    int currentP = i + 1;
                    Console.WriteLine(currentP + ". " + playerArray[i].viewName());
                }
            }
            foreach (Player player in playerArray) { Console.WriteLine(player.viewName() + " has " + player.showToken() + " tokens."); }
            while (true)
            {
                // The players are placed into a while loop where the rounds will keep going into someone has enough tokens to win.
                Console.WriteLine("We are starting round " + roundNumber + "\r\nThe amount of tokens needed to win are " + tokensToWin);
                XML.SaveGame(playerArray, roundNumber, SP);
                currentRound();
                roundNumber++;
                if(CheckWinner() != null)
                {
                    Console.WriteLine("Congratulations to " + CheckWinner().viewName() + "you have beat the competition and have earned the love of the princess.");
                    return;
                }
            }
        }

        private static void currentRound()
        {
            // Before the round starts its important to fill and shuffle the deck and that every player has 2 cards at the beginning so certain card types
            // dont crash the game.
            fillDeck();
            shuffleDeck(gameDeck);
            foreach (Player player in playerArray) { player.DrawCard(player.viewName()); player.DrawCard(player.viewName());  }
            while (gameDeck.Count > 0 && onePlayerLeft(playerArray) == true)
            {
                foreach (Player player in playerArray)
                {
                    if(SP == false) { Console.WriteLine("It is " + player.viewName() + " turn, press the enter key when your ready to begin."); Console.ReadKey(); }
                    if(SP == true) { Console.WriteLine("It is your turn " + player.viewName() + "."); }
                    // It is important to check that a player has no less or no more than two cards, if they are missing one then they need to be given it before their turn.
                    if (player.GetPlayerDeck().Count < 2) { player.DrawCard(player.viewName()); }                   
                    if (gameDeck.Count <= 0) { Console.WriteLine("The draw pile is empty the current round is over!"); break; }
                    if (onePlayerLeft(playerArray) == false) { Console.WriteLine("There is only one player left"); break; }
                    if (player.checkIfPlayerIsOut() == true)
                    {
                        {
                            if(CheckCard("Countess", player.GetPlayerDeck()) && (CheckCard("King", player.GetPlayerDeck()) || CheckCard("Prince", player.GetPlayerDeck())))
                            {
                                player.PlayCountess();
                                if (Utilities.GetTypeName(player).Equals("Human")) { Console.WriteLine("You had both a Countess and a King or a Prince so you were forced to play the Countess"); }
                            }
                            else { player.selectCard(); }
                            player.triggerShield(false);
                            player.chosenCard();         
                        }
                    }
                    else if(player.checkIfPlayerIsOut() == false) { Console.WriteLine(player.viewName() + " is out their turn will be skipped\r\n"); }              
                    // I decided to set up some stuff for multiplayer to try and keep each playes cards secret to one another, the singleplayer version doesn't
                    // need it as the player cant see the robots cards.
                    if(SP == false) { Console.WriteLine("It is the end of " + player.viewName() + " turn press the enter key and pass to the next person"); Console.ReadLine(); Console.Clear(); }
                }
            }
            int lowest = 10000000;
            List<Player> winners = new List<Player>();
            // This is used to see whos card is closer to the princess at the end of the round in case there are multiple people left if the deck is empty.
            foreach (Player player in playerArray)
            {
                if (player.checkIfPlayerIsOut())
                {
                    string card = Utilities.GetTypeName(player.GetPlayerDeck()[0]);
                    int score = Utilities.cardTypes[card];
                    if (score == lowest) { winners.Add(player); }
                    if (score < lowest) { lowest = score; winners.Clear(); winners.Add(player); }
                }
            }
            // Lets players know who won the round.
            foreach (Player winner in winners) { winner.addToken(); Console.WriteLine(winner.viewName() + " Won the round!"); Console.WriteLine(); Console.WriteLine("They gain one token!\n"); }
            // Lets players know the current progress in tokens.
            foreach (Player player in playerArray) { Console.WriteLine(player.viewName() + " has " + player.showToken() + " tokens."); }
            // Factory resets a player so that there arent any mishaps in the next round.
            foreach (Player player in playerArray) { player.ClearHand(); player.changePlayerStatus(true); player.triggerShield(false); }
        }

        private static List<Card> shuffleDeck(List<Card> gameDeck)
        {
            //This shuffles our deck so that when a player picks up the next card it will be random and not repetitive.  
            Random rng = new Random();
            int game_deck_size = gameDeck.Count;
            while (game_deck_size > 1)
            {
                game_deck_size--;
                int randomNumber = rng.Next(game_deck_size + 1);
                Card number = gameDeck[randomNumber];
                gameDeck[randomNumber] = gameDeck[game_deck_size];
                gameDeck[game_deck_size] = number;
            }
            return gameDeck;
        }

        private static List<Card> fillDeck()
        {
            gameDeck = new List<Card> { };
            // These for loops make sure that the right amount of each card type is put into the deck that was suggested
            // in the specification.
            for (int i = 0; i < 1; i++)
            {
                gameDeck.Add(new Princess());
                gameDeck.Add(new King());
                gameDeck.Add(new Countess());
            }
            for (int i = 0; i < 2; i++)
            {
                gameDeck.Add(new Prince());
                gameDeck.Add(new Handmaid());
                gameDeck.Add(new Baron());
                gameDeck.Add(new Priest());
            }
            for (int i = 0; i < 5; i++)
            {
                gameDeck.Add(new Guard());
            }
            return gameDeck;
        }

        private static Player CheckWinner()
        {
            // This allows the program to check if a person has reached the necessary amount of tokens to win, if not it returns a value of
            // null so that the game will continue.
            foreach(Player player in playerArray)
            {
                if(player.showToken()  == tokensToWin) { return player; }
            }
            return null;
        }

        private static bool CheckCard(string checkCard, List<Card> playerDeck)
        {
            // This is to check if a player has a certain card in their deck.
            foreach(Card card in playerDeck) { if (Utilities.GetTypeName(card).Equals(checkCard)) { return true; } }
            return false;
        }

        private static bool onePlayerLeft(Player[] playerArray)
        {
            // This checks if there is only one player left, if there are it returns a false bool so that the current round ends.
            int playersLeft = 0;
            foreach(Player player in playerArray)
            {
                if (player.checkIfPlayerIsOut() == true) { playersLeft++; }
                else if(player.checkIfPlayerIsOut() == false) { }
            }
            if (playersLeft <= 1) { return false; }
            else { return true; }
        }
    }
}
