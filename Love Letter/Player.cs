using System;
using System.Collections.Generic;
using System.Text;

namespace Love_Letter
{
    public abstract class Player
    {
        private int tokenCount;
        private string playerName;
        protected List<Card> playerDeck;
        private bool shielded;
        private bool inPlay;
        protected Card selectedCard;

        public Player(string name)
        {
            //This declares starting values for a players data types.
            tokenCount = 0;
            this.playerName = name;
            playerDeck = new List<Card>();
            this.shielded = false;
            this.inPlay = true;
        }

        // An abstract method that will be used differently between the player and the robot.
        public abstract void selectCard();

        // Allows the program to present a players name with ease.
        public string viewName() { return playerName; }

        // A commonly used method that allows a player to pick a new card at the start of a new round or when attacked with certain
        // card types.
        public void DrawCard(string currentPlayer)
        {
            //This method automatically draws whatever is next in the shuffled deck.
            Card drawnCard = MainGame.gameDeck[0];
            if(Utilities.GetTypeName(this) == "Human")
            {
                Console.WriteLine(currentPlayer + " drew a card from the deck");
                Console.WriteLine();
            }
            MainGame.gameDeck.RemoveAt(0);
            playerDeck.Add(drawnCard);
        }

        // Adds a token to the players overall score if they win the round.
        public void addToken() { tokenCount++; }

        // Allows the program to see the players current score, is used for end of round updates and to check if a player has reached
        // the required tokens to win the game.
        public int showToken() { return tokenCount; }

        // Switches a players shield either on or off dependent on if their turn is over or if they used a Handmaid card.
        public void triggerShield(bool shielded) { this.shielded = shielded; }

        // Allows us to see the status of a persons shield so that we can see if the player can target them.
        public bool checkShield() { return shielded; }

        // Allows us to see whether a player is in or out of the round so that we can see if the player can target them.
        public bool checkIfPlayerIsOut() { return inPlay; }

        // Changes the players status, used when they have been defeated or a new round begins.
        public void changePlayerStatus(bool play) { this.inPlay = play; }

        // A method that sends the selected card through the Card.Play method to be processed to the right card type.
        public void chosenCard() { selectedCard.Play(MainGame.playerArray, this); }

        // Allows us to see a certain players deck.
        public List<Card> GetPlayerDeck() { return playerDeck; }  

        // Used to make switching the 2 players deck in the king card class easier.
        public void SetPlayerDeck(List<Card> newPlayerDeck) { playerDeck = newPlayerDeck; }

        // Forces a player to play the countess if they have it in their hand along with a king or prince, essentially a self
        // inflicted miss a turn card.
        public void PlayCountess()
        {
            int countess = 1;
            if(Utilities.GetTypeName(this.GetPlayerDeck()[0]) == "Countess") { countess = 0; }
            selectedCard = playerDeck[countess];
            playerDeck.RemoveAt(countess);
        }

        // Clears a players full hand, used at the end of the round or when a player plays a prince card
        public void ClearHand() { playerDeck.Clear(); }

        // Gives a list of players and whether they can be targeted, if they aren't the player is sent a message to choose a different player.
        public static Player choosePlayer(Player[] playerArray, bool selfSelect, string currentPlayer)
        {
            while (true)
            {           
                Console.WriteLine("Which player would you like to select?");
                int chosenPlayer;
                int targetablePlayers = 0;
                int i = 1;
                foreach(Player player in playerArray)
                {                      
                    Console.Write(i + ". " + player.viewName());
                    if (player.checkShield() == true) { Console.Write(" - SHIELDED"); }
                    else if (player.checkIfPlayerIsOut() == false) { Console.Write(" - PLAYER IS OUT"); }
                    else if (player.viewName().Equals(currentPlayer) && selfSelect == false) { Console.Write(" - YOU CANT PICK YOURSELF"); }
                    else { targetablePlayers++; }
                    Console.WriteLine();
                    i++;
                }
                if (targetablePlayers == 0)
                {
                    Console.WriteLine("There are currently no available players to target");
                    Console.WriteLine("Your turn is over.");
                    return null;
                }
                try
                {
                    chosenPlayer = int.Parse(Console.ReadLine());                  
                    if (playerArray[chosenPlayer - 1].checkShield() == true) { Console.WriteLine("This player is shielded with a handmaid"); throw new Exception(); }
                    else if (playerArray[chosenPlayer - 1].checkIfPlayerIsOut() == false) { Console.WriteLine("This person is out of the current round"); throw new Exception(); }
                    else if(playerArray[chosenPlayer - 1].viewName().Equals(currentPlayer) && selfSelect == false) { Console.WriteLine("You cant pick yourself with this card"); throw new Exception(); }
                    return playerArray[chosenPlayer - 1];
                }
                catch (Exception e) { Console.WriteLine("Please enter a valid number"); }
            }    
        }
    }  
}
