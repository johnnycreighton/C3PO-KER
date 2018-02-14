using BluffinMuffin.HandEvaluator;
using MoreLinq;
using System;
using System.Linq;

namespace Samus
{
    class GetWinner
    {
        internal static string Calculate(Player[] tablePlayers, string[] communityCards)
        {
            IStringCardsHolder[] players =
                {
                   new Program.Player(tablePlayers[1].Name, tablePlayers[1].FirstCard.ToString(),tablePlayers[1].SecondCard.ToString(), communityCards[0], communityCards[1], communityCards[2], communityCards[3],communityCards[4]),
                   new Program.Player(tablePlayers[0].Name, tablePlayers[0].FirstCard.ToString(),tablePlayers[0].SecondCard.ToString(), communityCards[0], communityCards[1], communityCards[2], communityCards[3],communityCards[4]),
                };

            IStringCardsHolder winner;
            string stringWinner = null;
            string s = "";
           foreach (var p in HandEvaluators.Evaluate(players).SelectMany(x => x))
           {
               Console.WriteLine("{0}: {1} -> {2}", p.Rank == int.MaxValue ? "  " : p.Rank.ToString(), ((Program.Player)p.CardsHolder).Name, p.Evaluation); //prints out all players and ranks
               winner = players.MaxBy(x => p.Rank);
               stringWinner = winner.ToString();
            }
            foreach (var category in stringWinner)
            {
                s += category;
                Console.WriteLine(s);
            }
            int index = s.IndexOf(':');

            if (index >= 0)
            {
                return s.Substring(0, index);
            }
            else
                return null; //TODO exception handle this
               
        }
    }
}
