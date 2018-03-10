using BluffinMuffin.HandEvaluator;
using System.IO;
using System.Linq;

namespace Samus
{
    class River
    {
        private static string CasinoToBot = Program.CasinoToBot;
        private static string BotToCasino = Program.BotToCasino;
        private static int Rank;

        internal static void Start(string[] path, int preFlopRank, int position, string debugBotPath)
        {
            File.AppendAllText(debugBotPath, "\nEntered River." + System.Environment.NewLine);

            File.AppendAllText(debugBotPath, string.Format("Read River card: {0} {1} {2} {3} {4}", Program.CommunityCards[0], Program.CommunityCards[1], Program.CommunityCards[2], Program.CommunityCards[3], Program.CommunityCards[4]) + System.Environment.NewLine);
            IStringCardsHolder[] players =
                {
                    new Program.Player("Samus", Program.Samus.FirstCard.ToString(), Program.Samus.SecondCard.ToString(), Program.CommunityCards[0], Program.CommunityCards[1], Program.CommunityCards[2], Program.CommunityCards[3], Program.CommunityCards[4])
                };

            HandEvaluationResult bestFiveCarder = null;
            foreach (var p in HandEvaluators.Evaluate(players).SelectMany(x => x))
            {
                bestFiveCarder = HandEvaluators.Evaluate(p.CardsHolder.PlayerCards, p.CardsHolder.CommunityCards);
            }
            Program.Samus.Hand = bestFiveCarder.Hand.ToString();
            File.AppendAllText(debugBotPath, string.Format("Best five card hand post River: {0}", bestFiveCarder + System.Environment.NewLine));
            Rank = HandStrategies.PotOddsTolerance.GetRiverRankings(bestFiveCarder);
            while (true) //not needed i think
            {
                if (FileManipulation.Listeners.BotFileChanged)
                {
                    if (Rank > 20)
                    {
                        File.WriteAllText(BotToCasino, "r");
                        File.AppendAllText(debugBotPath, "Changed bot file to 'r'." + System.Environment.NewLine);
                        break;
                    }
                    else if (Rank <= 0)
                    {
                        File.WriteAllText(BotToCasino, "f");
                        File.AppendAllText(debugBotPath, "Changed bot file to 'f'." + System.Environment.NewLine);
                        Program.HandFinished = true;
                        return;
                    }
                    else
                    {
                        File.WriteAllText(BotToCasino, "c");
                        File.AppendAllText(debugBotPath, "Changed bot file to 'c'." + System.Environment.NewLine);
                        break;
                    }
                }
            }
        
            while (true)
            {
                if (FileManipulation.Listeners.SummaryFileChanged)
                {
                   if (FileManipulation.Extractions.IsFileReady(CasinoToBot))
                   {
                       string text = System.IO.File.ReadAllText(CasinoToBot);
                       File.AppendAllText(debugBotPath, "text when the summary file has changed, should be finished  = " +  text + System.Environment.NewLine); //testing
                       break;
                   }

                    FileManipulation.Listeners.BotFileChanged = false;
                    FileManipulation.Listeners.SummaryFileChanged = false;
                    File.AppendAllText(debugBotPath, "*************************** Hand finished ***************************" + System.Environment.NewLine);
                    break;
                }
            }
        }
    }
}
