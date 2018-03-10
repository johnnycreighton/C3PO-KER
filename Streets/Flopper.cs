using BluffinMuffin.HandEvaluator;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Samus
{
    public class Flopper
    {
        private static string CasinoToBot = Program.CasinoToBot;
        private static string BotToCasino = Program.BotToCasino;
        private static string DebugBotPath = Program.DebugBotPath;


        private static bool isHandFinished;
        private static int TurnCard;
        private static int Rank;

        public static void Start(int[] cardNumbers, int preFlopRank, int position, string debugBotPath) // path = play area path 
        {
            File.AppendAllText(debugBotPath, "\nEntered flop." + System.Environment.NewLine);
            //position 2 goes first
            //read flop
            //sort hand
            //get rank
            //check draws
            //check or bet
            //go turning*

            string[] cards = cardNumbers.Select(x => x.ToString()).ToArray();

            FileManipulation.CardTransform.Flop(cards, ref Program.CommunityCards); //format = KJQ123 -> now K1J2Q3 -> community cards being set inside.

            File.AppendAllText(debugBotPath, string.Format("Read Flop cards: {0} {1} {2}", Program.CommunityCards[0], Program.CommunityCards[1], Program.CommunityCards[2]) + System.Environment.NewLine);
            IStringCardsHolder[] players =
                {
                    new Program.Player("Samus", Program.Samus.FirstCard.ToString(), Program.Samus.SecondCard.ToString(), Program.CommunityCards[0], Program.CommunityCards[1], Program.CommunityCards[2])
                };

            HandEvaluationResult bestFiveCarder = null;
            foreach (var p in HandEvaluators.Evaluate(players).SelectMany(x => x))
            {
                bestFiveCarder = HandEvaluators.Evaluate(p.CardsHolder.PlayerCards, p.CardsHolder.CommunityCards);
            }
            Program.Samus.Hand = bestFiveCarder.Hand.ToString();
            File.AppendAllText(debugBotPath, string.Format("Best five card hand post flop: {0}", bestFiveCarder + System.Environment.NewLine));
            HandStrategies.Draws.CheckForDraws(Program.Samus, Program.CommunityCards);
            File.AppendAllText(debugBotPath, "Checked for Draws!" + System.Environment.NewLine);

            Rank = HandStrategies.PotOddsTolerance.GetEnhancedRankings(Program.Samus, bestFiveCarder); 

            if (Rank <= 30 && Rank > 5) 
            {
                File.WriteAllText(BotToCasino, "c");
                File.AppendAllText(debugBotPath, "Changed bot file to 'c'." + System.Environment.NewLine);
            }
            else if (Rank >= 30)
            {
                File.WriteAllText(BotToCasino, "r");
                File.AppendAllText(debugBotPath, "Changed bot file to 'r'." + System.Environment.NewLine);
            }
            else if (preFlopRank < 5 ) 
            {
                File.WriteAllText(BotToCasino, "c");
                File.AppendAllText(debugBotPath, "Changed bot file to 'c'." + System.Environment.NewLine);
            }
            else
            {
                File.WriteAllText(BotToCasino, "f");
                File.AppendAllText(debugBotPath, "Changed bot file to 'f'. I missed the flop" + System.Environment.NewLine);
                Program.HandFinished = true;
                return;
            }

            while (true)
            {
                if (FileManipulation.Listeners.BotFileChanged)
                {
                    FileManipulation.Listeners.BotFileChanged = false;
                    if (TurnFound())
                    {
                        File.AppendAllText(DebugBotPath, "Turn Found" + System.Environment.NewLine);
                        return;
                    }
                }
            }
        }

        private static bool TurnFound()
        {
            string text = null;

            if (FileManipulation.Extractions.IsFileReady(CasinoToBot))
            {
                text = System.IO.File.ReadAllText(CasinoToBot);
            }
            else
            {
                return false;
            }
            

            int index = 0;
            if (text.Contains("T"))
            {
                
                foreach (var digit in text)
                {
                    ++index;
                    if (digit == 'T')
                    {
                        File.AppendAllText(DebugBotPath, "Turn found here =  " + text + System.Environment.NewLine);
                        TurnCard = Convert.ToInt32(Regex.Match(text.Substring(index), @"\d+").Value);
                        FileManipulation.CardTransform.WriteCommunityCards(TurnCard, 3);
                        break;
                    }
                }
                return true;
            }
            else
                return false;
        }
    }
}
