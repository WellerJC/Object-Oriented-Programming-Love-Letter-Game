using System;
using System.Collections.Generic;
using System.Text;

namespace Love_Letter
{
    class Utilities
    {
        // This is used for the baron card and it assigns a value to each card type so that we can compare them,
        // the player with the highest value loses. Also used to determine winners at the end of a round if the
        // game deck is empty.
        public static Dictionary<string, int> cardTypes = new Dictionary<string, int>
        {
            {"Princess", 0}, {"Countess", 1}, {"King", 2}, {"Prince", 3},
            {"Handmaid", 4}, {"Baron", 5}, {"Priest", 6}, {"Guard", 7}
        };

        // This allows us to easily get the name of an object type, it is especially useful when using filters.
        public static string GetTypeName(Object obj)
        {
            return obj.GetType().ToString().Split(".")[1];
        }

       
    }
}
