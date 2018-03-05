using BluffinMuffin.HandEvaluator;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Samus
{
    class River
    {
        private static string CasinoToBot = Program.CasinoToBot;
        private static string BotToCasino = Program.BotToCasino;
        
        internal static void Start(string[] path, int rank, int position, string debugBotPath)
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

            while (true)
            {
                if (FileManipulation.Listeners.BotFileChanged)
                {
                    FileManipulation.Listeners.BotFileChanged = false;

                    if (rank < 54) //TODO sort his out to have some real strategy
                    {
                        File.WriteAllText(BotToCasino, "c"); // should be raising. TODO
                        File.AppendAllText(debugBotPath, "Changed bot file to 'r'." + System.Environment.NewLine);

                        break;
                    }
                    else if (rank < 93)
                    {
                        File.AppendAllText(debugBotPath, "Changed bot file to 'c'." + System.Environment.NewLine);
                        File.WriteAllText(BotToCasino, "c");
                        break;
                    }
                    else
                    {
                        File.AppendAllText(debugBotPath, "Changed bot file to 'c'.  this is for testing purposes only. this is actually a fold. " + System.Environment.NewLine);
                        File.WriteAllText(BotToCasino, "c");//change to fold after testing
                                                            //System.Environment.Exit(0);
                        break;
                    }
                }
            }
            while (true)
            {
                if (FileManipulation.Listeners.BotFileChanged)
                {
                    if (FileManipulation.Listeners.SummaryFileChanged)
                    {
                        File.AppendAllText(debugBotPath, "River Found" + System.Environment.NewLine);
                        break;
                    }
                }
            }
        }
    }
}
