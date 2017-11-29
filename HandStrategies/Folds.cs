using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samus.HandStrategies
{
    class Folds
    {
        public static bool CheckForFolds(Samus.Player[] players)
        {
            if (players[0].Fold == true) //check to see has either has folded
            {
                Program.OpponentsWinnings += Program.Pot / 2; // divided by two , measuring profit alone
                Program.MyWinnings -= Program.Pot / 2;
                return true;
            }
            else if (players[1].Fold == true)
            {
                Program.MyWinnings += Program.Pot / 2;
                Program.OpponentsWinnings -= Program.Pot / 2;
                return true;
            }
            return false;
        }
    }
}
