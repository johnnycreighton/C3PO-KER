using System;
using System.Linq;
using System.Reflection;
using BluffinMuffin.HandEvaluator;

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

            if (actionplayer.OpenEndedStraightDraw && actionplayer.BackDoorFlushDraw) // ~35% equity if behind
            {//8 outs + backdoor flush

                if (flop)
                    actionplayer.Tolerance = Program.Pot / 1.9;
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
                    actionplayer.Tolerance = Program.Pot / 4.75;
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
            else if (actionplayer.BackDoorFlushDraw) // hits 21.11% equity if behind
            {//4% chance
                if (flop)
                    actionplayer.Tolerance = Program.Pot / 10;
                else
                    actionplayer.Tolerance = Program.Pot / 15;
                return;
            }
        }

        internal static int GetRiverRankings(HandEvaluationResult bestFiveCarder)
        {
            int rank = 0;
            string bestHand = bestFiveCarder.ToString();
            switch (bestHand)
            {

                case ("Two Pairs"):
                    rank += 10;
                    break;
                case ("Three of a kind"):
                    rank += 10;
                    break;
                case ("Straight"):
                    rank += 10;
                    break;
                case ("Flush"):
                    rank += 21;
                    break;
                case ("Full House"):
                    rank += 21;
                    break;
                default:
                    break;
            }
            return rank;
        }

        internal static int GetEnhancedRankings(Player samus, HandEvaluationResult bestFiveCarder)
        {
            int rank = 0;
            string bestHand = bestFiveCarder.Hand.ToString();

            if(samus.FlushDraw)
            {
                rank += 20;
            }
            else if (samus.BackDoorFlushDraw)
            {
                rank += 5;
            }
            if (samus.OpenEndedStraightDraw)
            {
                rank += 15;
            }
            else if (samus.GutShotStraightDraw)
            {
                rank += 10;
            }
            else if (samus.BackDoorStraightDraw)
            {
                rank += 4;
            }

            switch (bestHand)
            {
                case ("HighCard"):
                        rank -= 5;
                        break;
                case ("OnePair"):
                    rank += 5;
                    break;
                case ("TwoPairs"):
                    rank += 10;
                    break;
                case ("ThreeOfAKind"):
                    rank += 15;
                    break;
                case ("Straight"):
                    rank += 20;
                    break;
                case ("Flush"):
                    rank += 25;
                    break;
                case ("FullHouse"):
                    rank += 40;
                    break;
                default:
                    rank += 1000;
                    break;
            }

            return rank;
        }
    }
}
