using System;
using System.Collections.Generic;
using System.Text;

namespace Love_Letter
{
    public abstract class Card
    {
        // This is an abstract method that will be carried and used into the different card types.
        public abstract void Play(Player[] playerArray, Player currentPlayer);
    }

}
