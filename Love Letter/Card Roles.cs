using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Love_Letter
{
    class Princess : Card
    {
        // You may not discard the princess. If you are forced to discord her via the prince then you are out.
        public override void Play(Player[] playerArray, Player currentPlayer)
        {
           // No method needed as the princess card cannot be played.
        }
    }

    class Countess : Card
    {
        // If you have the countess in your hand with either the king or the prince then you must discard it. Otherwise, no effect.
        public override void Play(Player[] players, Player currentPlayer)
        {
            // Just a simple message suffices as this card does nothing.
                Console.WriteLine(currentPlayer.viewName() + " has played the countess card, it is inaffective");  
        }
    }

    class King : Card
    {
        // You must choose another player to swap cards with you.
        public override void Play(Player[] playerArray, Player currentPlayer)
        {
            if (Utilities.GetTypeName(currentPlayer).Equals("Human"))
            {
                Console.WriteLine("You played your king card this means you can swap your deck with another players");
                Player selectedPlayer = Player.choosePlayer(playerArray, false, currentPlayer.viewName());
                if (selectedPlayer == null) { return; }
                Console.WriteLine("You have selected " + selectedPlayer.viewName());
                // This bit puts the current player and the selected players card decks into a temporary array and then assigns the temporary
                // decks to the other player.
                Card[] playerDeck1 = new Card[currentPlayer.GetPlayerDeck().Count];
                Card[] playerDeck2 = new Card[selectedPlayer.GetPlayerDeck().Count];
                currentPlayer.GetPlayerDeck().CopyTo(playerDeck1);
                selectedPlayer.GetPlayerDeck().CopyTo(playerDeck2);
                currentPlayer.SetPlayerDeck(playerDeck2.ToList());
                selectedPlayer.SetPlayerDeck(playerDeck1.ToList());
                Console.WriteLine("You have swapped your " + Utilities.GetTypeName(playerDeck1[0]) + " with a " + Utilities.GetTypeName(playerDeck2[0]));
            }
            else
            {
                Console.WriteLine(currentPlayer.viewName() + " has played a king card");
                Random rng = new Random();
                int selectedPlayer;
                int targetablePlayer = 0;
                foreach (Player player in playerArray)
                {
                    // Checks whether there are targetable people, if not the robots turn ends and the card is used with no effect.
                    if (player.checkShield() == false && player.checkIfPlayerIsOut() == true && player.Equals(currentPlayer) == false) { targetablePlayer++; }
                    if (targetablePlayer == 0) { Console.WriteLine("None of the players are targetable.\r\n" + currentPlayer.viewName() + " turn is over"); return; }
                }
                while (true)
                {
                    // Checks if the player that was selected is targetable if not then the random number generator picks a new number.
                    selectedPlayer = rng.Next(0, playerArray.Length);
                    if (playerArray[selectedPlayer].checkShield() == true) { continue; }
                    else if (playerArray[selectedPlayer].checkIfPlayerIsOut() == false) { continue; }
                    else if (playerArray[selectedPlayer].Equals(currentPlayer) == true) { continue; }
                    break;
                }
                Console.WriteLine(currentPlayer.viewName() + " played The King they must choose a player to swap cards with.");
                Console.WriteLine("They have selected " + playerArray[selectedPlayer].viewName());
                //Is the same as the human version.
                Card[] playerDeck1 = new Card[currentPlayer.GetPlayerDeck().Count];
                Card[] playerDeck2 = new Card[playerArray[selectedPlayer].GetPlayerDeck().Count];
                currentPlayer.GetPlayerDeck().CopyTo(playerDeck1);
                playerArray[selectedPlayer].GetPlayerDeck().CopyTo(playerDeck2);
                currentPlayer.SetPlayerDeck(playerDeck2.ToList());
                playerArray[selectedPlayer].SetPlayerDeck(playerDeck1.ToList());
            }
            }
    }

    class Prince : Card
    {
        // You must choose any player. The player must discard one of their cards and draw another.
        public override void Play(Player[] playerArray, Player currentPlayer)
        {
            if (Utilities.GetTypeName(currentPlayer).Equals("Human"))
            {
                Player selectedPlayer = Player.choosePlayer(playerArray, true, currentPlayer.viewName());
                Console.WriteLine("You played your prince card this means you can choose a player and they have to discard a card");
                // Checks if there are cards in the game deck left, if not it cancels the effect as proceeding would crash the game as there
                // arent any cards to draw at the end of this method.
                if (MainGame.gameDeck.Count <= 0) { Console.WriteLine("This is ineffective as there are no cards left in the game deck"); return; }
                if (selectedPlayer == null) { return; }
                Console.WriteLine("You have chosen " + selectedPlayer.viewName());
                // If the player has a princess then they are instantly out of the round.
                if (Utilities.GetTypeName(selectedPlayer.GetPlayerDeck()[0]).Equals("Princess")) 
                {
                    Console.WriteLine(selectedPlayer.viewName() + " had to discard their princess and is now out of the round");
                    selectedPlayer.changePlayerStatus(false);
                    return;
                }
                selectedPlayer.ClearHand();
                Console.WriteLine(selectedPlayer.viewName() + " has got rid of their card");
                selectedPlayer.DrawCard(selectedPlayer.viewName());
            }
            else
            {
                Console.WriteLine(currentPlayer.viewName() + " has played a prince card");
                if (MainGame.gameDeck.Count <= 0) { Console.WriteLine("This is ineffective as there are no cards left in the game deck"); return; }
                Random rng = new Random();
                int selectedPlayer;
                int targetablePlayer = 0;
                // Checks whether there are targetable people, if not the robots turn ends and the card is used with no effect.
                foreach (Player player in playerArray)
                {
                    if (player.checkShield() == false && player.checkIfPlayerIsOut() == true && player.Equals(currentPlayer) == false) { targetablePlayer++; }
                }
                    if (targetablePlayer == 0) { Console.WriteLine("There are currently no targetable players.\r\nThis players turn is over."); return; }
                while (true)
                {
                    // Checks if the player that was selected is targetable if not then the random number generator picks a new number.
                    selectedPlayer = rng.Next(0, playerArray.Length);
                        if (playerArray[selectedPlayer].checkShield() == true || playerArray[selectedPlayer].checkIfPlayerIsOut() == false || playerArray[selectedPlayer].Equals(currentPlayer) == true)
                        { continue; }
                    break;
                }
                Console.WriteLine(currentPlayer.viewName() + " has chosen " + playerArray[selectedPlayer].viewName());
                playerArray[selectedPlayer].ClearHand();
                Console.WriteLine(playerArray[selectedPlayer].viewName() + " has got rid of their card");
                playerArray[selectedPlayer].DrawCard(playerArray[selectedPlayer].viewName());
            }
        }
    }

    class Handmaid : Card
    {
        // The handmaid protects you until your next turn.
        public override void Play(Player[] players, Player currentPlayer)
        {
            // Simply changes the shield bool to true so when it is checked by another player the person cant be targeted.
            currentPlayer.triggerShield(true);
            Console.WriteLine(currentPlayer.viewName() + " has played the handmaid card and is now shielded.");
        }
    }

    class Baron : Card
    {
        // You must choose another player, whoevers card is further from the princess in role is out.
        public override void Play(Player[] playerArray, Player currentPlayer)
        {
            if (Utilities.GetTypeName(currentPlayer).Equals("Human"))
            {
                Console.WriteLine("You played your baron card, you must pick another players card to compete for the princess");
                Player selectedPlayer = Player.choosePlayer(playerArray, false, currentPlayer.viewName());
                if (selectedPlayer == null) { return; }
                int spc = 0;
                int currentwinner = 0;
                int selectedwinner = 0;
                // This loop makes sure that both cards in the selected players deck are taken into account and compared to the remaining
                // card in the current players deck.
                foreach (Card card in selectedPlayer.GetPlayerDeck())
                {
                    int currentPlayerScore = Utilities.cardTypes[Utilities.GetTypeName(currentPlayer.GetPlayerDeck()[0])];
                    int selectedPlayerScore = Utilities.cardTypes[Utilities.GetTypeName(selectedPlayer.GetPlayerDeck()[spc])];
                    // If the current players card has a lower assigned value then they earn a point and vice versa, if they both happen to
                    // have the same card they both get a point.
                    if(currentPlayerScore < selectedPlayerScore) { currentwinner++; }
                    else if(currentPlayerScore > selectedPlayerScore) { selectedwinner++; }
                    else { currentwinner++; selectedwinner++; }
                    spc++;
                }
                // If the current player has more points in the end the selected player is taken out of the game and vice versa, if they 
                // end up having the same amount its a draw and neither are defeated.
                if(currentwinner > selectedwinner)
                {
                    Console.WriteLine("You are closer to the princess, " + selectedPlayer.viewName() + " is out of the round");
                    selectedPlayer.changePlayerStatus(false);
                }
                else if(currentwinner < selectedwinner)
                {
                    Console.WriteLine("You are further away from the princess, you are out of the round");
                    currentPlayer.changePlayerStatus(false);
                }
                else { Console.WriteLine("You are both the same distance away so nothing happens;"); }
            }
            else
            {
                Console.WriteLine(currentPlayer.viewName() + " has played a baron card");
                Random rng = new Random();
                int selectedPlayer;
                int targetablePlayer = 0;
                // Checks whether there are targetable people, if not the robots turn ends and the card is used with no effect.
                foreach (Player player in playerArray)
                {
                    if (player.checkShield() == false && player.checkIfPlayerIsOut() == true && player.Equals(currentPlayer) == false) { targetablePlayer++; }
                }
                if (targetablePlayer < 1) { Console.WriteLine("None of the other players are targetable.\r\n" + currentPlayer.viewName() + "turn is over."); return; }
                while (true)
                {
                    // Checks if the player that was selected is targetable if not then the random number generator picks a new number.
                    selectedPlayer = rng.Next(0, playerArray.Length);
                    if (playerArray[selectedPlayer].checkShield() == true || playerArray[selectedPlayer].checkIfPlayerIsOut() == false || playerArray[selectedPlayer].Equals(currentPlayer)) { continue; }
                    break;
                }
                int spc = 0;
                int currentwinner = 0;
                int selectedwinner = 0;
                Console.WriteLine(currentPlayer.viewName() + " has picked to compete with" + playerArray[selectedPlayer].viewName());
                // The rest of this method is essentially the same as the humans version.
                foreach (Card card in playerArray[selectedPlayer].GetPlayerDeck())
                {
                    int currentPlayerScore = Utilities.cardTypes[Utilities.GetTypeName(currentPlayer.GetPlayerDeck()[0])];
                    int selectedPlayerScore = Utilities.cardTypes[Utilities.GetTypeName(playerArray[selectedPlayer].GetPlayerDeck()[spc])];
                    if (currentPlayerScore > selectedPlayerScore) { currentwinner++; }
                    else if (currentPlayerScore < selectedPlayerScore) { selectedwinner++; }
                    else { currentwinner++; selectedwinner++; }
                    spc++;
                }
                if (currentwinner > selectedwinner)
                {
                    Console.WriteLine(currentPlayer.viewName() + " was closer to the princess, " + playerArray[selectedPlayer].viewName() + " is out of the round");
                    playerArray[selectedPlayer].changePlayerStatus(false);
                }
                else if (currentwinner < selectedwinner)
                {
                    Console.WriteLine(currentPlayer.viewName() + " was further away from the princess, " + currentPlayer.viewName() + " is out of the round");
                    currentPlayer.changePlayerStatus(false);
                }
                else { Console.WriteLine("Both players were the same distance away so nothing happens;"); }
            }
        }
    }

    class Priest : Card
    {
        // You can choose and look at another players cards.
        public override void Play(Player[] playerArray, Player currentPlayer)
        {
            int i = 1;
            if (Utilities.GetTypeName(currentPlayer).Equals("Human"))
            {
                Console.WriteLine("You play a priest that means you get to see another players card");
                Player selectedPlayer = Player.choosePlayer(playerArray, false, currentPlayer.viewName());
                if (selectedPlayer == null) { return; }
                // Shows you both of the players cards.
                foreach (Card card in selectedPlayer.GetPlayerDeck()) { Console.WriteLine(i + ". " + Utilities.GetTypeName(card)); i++; }
                return;
            }
            else
            {
                Console.WriteLine(currentPlayer.viewName() + " played a priest");
                Random rng = new Random();
                int selectedPlayer;
                while (true)
                {
                    // Checks if the player that was selected is targetable if not then the random number generator picks a new number.
                    selectedPlayer = rng.Next(0, playerArray.Length);
                    if (playerArray[selectedPlayer].checkShield() == true || playerArray[selectedPlayer].checkIfPlayerIsOut() == false || playerArray[selectedPlayer].Equals(currentPlayer)) { continue; }
                    break;
                }
               // Notifies the player on whether the robot saw the players card or another robots.
                if (selectedPlayer == 0 && playerArray[0].checkShield()) { Console.WriteLine(currentPlayer.viewName() + " saw your cards"); }
                else { Console.WriteLine(currentPlayer.viewName() + " saw " + playerArray[selectedPlayer].viewName() + "'s card"); }
            }
        }
    }

    class Guard : Card
    {
        // You can pick another player and try to guess their card, if you are correct they are out. Otherwise no effect.
        public override void Play(Player[] playerArray, Player currentPlayer)
        {
            if (Utilities.GetTypeName(currentPlayer).Equals("Human"))
            {
                while (true)
                {
                    Console.WriteLine("You played your guard card you can pick and guess another players card, if you are correct they are out.");
                    Player selectedPlayer = Player.choosePlayer(playerArray, false, currentPlayer.viewName());
                    Console.WriteLine("Please select one of these cards:\r\n1. Princess\r\n2. Countess\r\n3. King\r\n4. Prince\r\n5. Handmaid\r\n6. Baron\r\n7.Priest");
                    int selectedCard;
                    bool guessedCorrectly = false;
                    while (true)
                    {
                        try { selectedCard = int.Parse(Console.ReadLine()); if (selectedCard < 1 || selectedCard > 7){ throw new Exception(); } break; }
                        catch (Exception e) { Console.WriteLine("Please enter a valid number"); }
                    }
                    // Checks if either of their cards are equal to the option you picked.
                    for (int i = 0; i < selectedPlayer.GetPlayerDeck().Count(); i++)
                    {
                        // A big if loop filter, if the current player is correct the guessedCorrectly bool is changed to true otherwise it
                        // stays false.
                        if ((Utilities.GetTypeName(selectedPlayer.GetPlayerDeck()[i]).Equals("Princess") && selectedCard == 1) || (Utilities.GetTypeName(selectedPlayer.GetPlayerDeck()[i]).Equals("Countess") && selectedCard == 2)
                            || (Utilities.GetTypeName(selectedPlayer.GetPlayerDeck()[i]).Equals("King") && selectedCard == 3) || (Utilities.GetTypeName(selectedPlayer.GetPlayerDeck()[i]).Equals("Prince") && selectedCard == 4)
                            || (Utilities.GetTypeName(selectedPlayer.GetPlayerDeck()[i]).Equals("Handmaid") && selectedCard == 5) || (Utilities.GetTypeName(selectedPlayer.GetPlayerDeck()[i]).Equals("Baron") && selectedCard == 6)
                            || (Utilities.GetTypeName(selectedPlayer.GetPlayerDeck()[i]).Equals("Priest") && selectedCard == 7)) { guessedCorrectly = true; }
                    }
                    // If the current player was correct the guessedCorrectly bool would be true and therefore defeat the selected player
                    // otherwise its ineffective.
                    if (guessedCorrectly == true)
                    {
                        Console.WriteLine("Congratulations, you guessed correctly!\r\n" + selectedPlayer.viewName() + " is out of the round");
                        selectedPlayer.changePlayerStatus(false);
                    }
                    else if (guessedCorrectly == false) { Console.WriteLine("You guessed incorrectly"); }
                    break;
                }
            }  
            else
            {
                Console.WriteLine(currentPlayer.viewName() + " has drawn a guard card");
                Random rng = new Random();                
                int targetablePlayer = 0;
                // Checks whether there are targetable people, if not the robots turn ends and the card is used with no effect.
                foreach (Player player in playerArray)
                {
                    if(player.checkShield() == false && player.checkIfPlayerIsOut() == true && player.Equals(currentPlayer) == false) { targetablePlayer++; }
                }
                if (targetablePlayer < 1) { Console.WriteLine("None of the other players are targetable.\r\n" + currentPlayer.viewName() + "turn is over."); return; }
                Double guessNumber = rng.NextDouble();
                // This is decided on whether the robot guesses a value under a certain percentage, since there is 7 different choices the
                // needed value is rougly the first seventh of one (0% - 15%).
                if(guessNumber < 0.15)
                {
                    int chosenPlayer = rng.Next(0, playerArray.Length);
                    Player selectedPlayer = playerArray[chosenPlayer];
                    while(selectedPlayer.Equals(currentPlayer) || currentPlayer.checkShield() == true)
                    {
                        chosenPlayer = rng.Next(0, playerArray.Length);
                        selectedPlayer = playerArray[chosenPlayer];
                    }
                    Console.WriteLine(currentPlayer.viewName() + " guessed correctly!\r\n" + selectedPlayer.viewName() + " is out of the round\r\n");
                    selectedPlayer.changePlayerStatus(false);
                }
                else { Console.WriteLine(currentPlayer.viewName() + "guessed incorrectly."); }
            }
        }
    }
}
