using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samus.HandStrategies
{
    class PotOddsTolerance
    {
        internal static void CalculateTolerance(Player actionplayer, bool flop)
        {
        
            if(actionplayer.OpenEndedStraightDraw && actionplayer.FlushDraw) // ~35% equity if behind
            {//15 outs
                
                if(flop)
                    actionplayer.Tolerance = Program.Pot / 2.1;
                else
                    actionplayer.Tolerance = Program.Pot / 1.2;
                return;
            }

            else if(actionplayer.FlushDraw && actionplayer.GutShotStraightDraw) // ~30% equity if behind
            {//12 outs
                if (flop)
                    actionplayer.Tolerance = Program.Pot / 2;
                else
                    actionplayer.Tolerance = Program.Pot;
                return;
            }

            else if (actionplayer.FlushDraw) //~25% when marginally behind
            {//9 outs
                if (flop)
                    actionplayer.Tolerance = Program.Pot / 4.11;
                else
                    actionplayer.Tolerance = Program.Pot / 1.8;
                return;
            }

            else if (actionplayer.OpenEndedStraightDraw) //~25% equit when marginally behind
            {//8 outs
                if (flop)
                    actionplayer.Tolerance = Program.Pot * 4.75;
                else
                    actionplayer.Tolerance = Program.Pot / 2.17;
                return;
            }

            else if (actionplayer.GutShotStraightDraw) // hits 21.11% equity if behind
            {//4 outs
                if (flop)
                    actionplayer.Tolerance = Program.Pot / 9;
                else
                    actionplayer.Tolerance = Program.Pot / 15;
                return;
            }
        }
    }
}
