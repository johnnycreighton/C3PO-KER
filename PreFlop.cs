using BluffinMuffin.HandEvaluator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.String;
using BluffinMuffin.HandEvaluator;
using System.Threading;

namespace Samus
{
    class PreFlop
    {

        internal static void Play(Player[] players, int count)
        {
            if (count % 2 == 0)
                players[0].button = true;
            else
                players[1].button = true;
            
            foreach(var p in players)
            {
                if (p.button == true)
                    Action(p); 
            }




        }

        public static void Action(Player actionplayer)
        {
            //check for raise ---- never a raise pre flop cause its first action.
            //get stack here
            //check how strong cards are, stats, probability, rank

            IStringCardsHolder[] player =
                {
                    new Program.Player("Johnny", actionplayer.FirstCard.ToString(), actionplayer.SecondCard.ToString()),
                };

            BluffinMuffin.HandEvaluator.EvaluatedCardHolder<BluffinMuffin.HandEvaluator.IStringCardsHolder> strength;
            // IStringCardsHolder sguih = actionplayer.FirstCard.ToString() +","+ actionplayer.SecondCard.ToString();
            foreach (var p in HandEvaluators.Evaluate(player).SelectMany(x => x))
            {
                strength = p;
            }
           
            
            
            
            
            // var res = HandEvaluators.Evaluate(new[] { "Ks", "5s" }, new[] { "8d", "As" });
            //var biggest = HandEvaluators.Evaluate(new[] { "Ks", "Ac" }, new[] { "8d", "3c", "7s", "10h", "2s" });
            //actAccordingly
        }


    }
}
