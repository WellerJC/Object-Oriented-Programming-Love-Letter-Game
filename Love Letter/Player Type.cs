using System;
using System.Collections.Generic;
using System.Text;

namespace Love_Letter
{
    class Human : Player
    {
        public Human(string name) : base(name)
        {          
        }
        public override void selectCard()
        {
            // The human selectCard() method shows the person both of their cards and  allows them to select one that will temporarily be 
            // assigned to the variable "Card selectedCard" in the parent class.
            while (true)
            {
                int i = 1;
                Console.WriteLine("Which card would you like to select?");
                foreach (Card card in playerDeck)  { Console.WriteLine(i + ". " + Utilities.GetTypeName(this.playerDeck[i-1])); i++; }
                int chosenCard;
                try
                {
                    chosenCard = int.Parse(Console.ReadLine());
                    // It is important to check whether a player is inputting a valid value and if they arent then we need to stop the wrong value 
                    // from breaking the game. Also we need to check if they picked the value assigned to a princess and stop them as a princess
                    // cannot be played.
                    if(chosenCard != 1 && chosenCard != 2) { Console.WriteLine("Please choose either card 1 or card 2"); continue; }
                    if(Utilities.GetTypeName(playerDeck[chosenCard - 1]).Equals("Princess")) { Console.WriteLine("You cannot use the princess"); continue; }
                    selectedCard = playerDeck[chosenCard - 1];
                    playerDeck.RemoveAt(chosenCard - 1);
                    return;
                }
                catch(Exception e){ continue; }
            }
        }
    }
    class Robot : Player
    {
        public Robot(string name) : base(name)
        {
        }
        public override void selectCard()
        {
            // The Robot selectCard() method is a simple random number generator that picks either the first or second card at random, it also
            // checks if the robot has a princess before hand and if so selects the other card.
            if (Utilities.GetTypeName(GetPlayerDeck()[0]).Equals("Princess")) { selectedCard = playerDeck[1]; playerDeck.RemoveAt(1); return; }
            if (Utilities.GetTypeName(GetPlayerDeck()[1]).Equals("Princess")) { selectedCard = playerDeck[0]; playerDeck.RemoveAt(0); return; }
            Random rng = new Random();
            int randomNumber = rng.Next(0, 2);
            selectedCard = playerDeck[randomNumber];
            playerDeck.RemoveAt(randomNumber);
        }
    }
}
