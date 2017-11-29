using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samus.HandStrategies
{
    class HighCard
    {
        internal static void Action(Player actionplayer, bool flop)
        {
            //if drawing, try get to the turn cheap. if not bail
            PotOddsTolerance.CalculateTolerance(actionplayer, flop);




        }
    }
}
