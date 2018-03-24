using BluffinMuffin.HandEvaluator;

namespace Samus.HandStrategies
{
    class PotOddsTolerance
    {
        /// <summary>
        /// Logic for determining how strong the bots final hand is.
        /// </summary>
        /// <param name="bestFiveCarder"></param>
        /// <returns></returns>
        internal static int GetRiverRankings(HandEvaluationResult bestFiveCarder)
        {
            int rank = 0;
            string bestHand = bestFiveCarder.ToString();

            switch (bestHand)
            {
                case ("HighCard"):
                    rank = 1;
                    break;
                case ("OnePair"):
                    rank = 1;
                    break;
                case ("TwoPairs"):
                    rank = 5;
                    break;
                default:
                    rank = 10;
                    break;
            }
            return rank;
        }
        /// <summary>
        /// Method which will determine if a hand is worth raising, calling or folding
        /// </summary>
        /// <param name="samus"></param>
        /// <param name="bestFiveCarder"></param>
        /// <returns></returns>
        internal static int GetEnhancedRankings(Player samus, HandEvaluationResult bestFiveCarder)
        {
            int rank = 0;
            string bestHand = bestFiveCarder.Hand.ToString();

            if (samus.FlushDraw && samus.OpenEndedStraightDraw)
            {
                rank = 105;
            }
            if ((samus.BackDoorFlushDraw && samus.BackDoorStraightDraw) || samus.OpenEndedStraightDraw || samus.FlushDraw)
            {
                rank += 15;
            }
            if (samus.GutShotStraightDraw || samus.BackDoorStraightDraw)
            {
                rank += 15;
            }
            switch (bestHand)
            {
                case ("HighCard"):
                    rank -= 5;
                    break;
                case ("OnePair"):
                    rank += 5;
                    break;
                default:
                    rank += 100;//anything else is worth a raise.
                    break;
            }
            return rank;
        }
    }
}