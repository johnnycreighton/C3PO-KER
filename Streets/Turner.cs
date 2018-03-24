using BluffinMuffin.HandEvaluator;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Samus.Streets
{
    class Turner
    {
        private static string CasinoToBot = Program.CasinoToBot;
        private static string BotToCasino = Program.BotToCasino;
        private static int Rank;
        public static char action;

        public static int RiverCard;

        internal static void Start(string[] communityCards, int preFlopRank, int position)
        {
            //File.AppendAllText(debugBotPath, "\nEntered Turn." + System.Environment.NewLine);

            //  File.AppendAllText(debugBotPath, string.Format("Read Turn card: {0} {1} {2} {3}", Program.CommunityCards[0], Program.CommunityCards[1], Program.CommunityCards[2], Program.CommunityCards[3]) + System.Environment.NewLine);
            IStringCardsHolder[] players =
            {
                new Program.Player("Samus", Program.Samus.FirstCard.ToString(), Program.Samus.SecondCard.ToString(), Program.CommunityCards[0], Program.CommunityCards[1], Program.CommunityCards[2], Program.CommunityCards[3])
            };

            HandEvaluationResult bestFiveCarder = null;
            foreach (var p in HandEvaluators.Evaluate(players).SelectMany(x => x))
            {
                bestFiveCarder = HandEvaluators.Evaluate(p.CardsHolder.PlayerCards, p.CardsHolder.CommunityCards);
            }

            Program.Samus.Hand = bestFiveCarder.Hand.ToString();
            
            //  File.AppendAllText(debugBotPath, string.Format("Best five card hand post turn: {0}", bestFiveCarder + System.Environment.NewLine));

            HandStrategies.Draws.CheckForDraws(Program.Samus, Program.CommunityCards);
            
            //File.AppendAllText(debugBotPath, "Checked for Draws!" + System.Environment.NewLine);

            FileManipulation.Listeners.StartSummaryFileWatcher(Program.BotDirPath); //Starting summary file watcher as this will indicate the end of the hand and allow program to start a new one

            FileManipulation.Listeners.BotFileChanged = false; //reseting to catch river acrd

            Program.Samus.BackDoorStraightDraw = false;
            Program.Samus.BackDoorFlushDraw = false; // resetting these as there is no possibility of back door draws anymore as there is only one community card left 
            Rank = HandStrategies.PotOddsTolerance.GetEnhancedRankings(Program.Samus, bestFiveCarder);

            if (Rank == 5)
            {
                action = 'c';
                File.WriteAllText(BotToCasino, "c");
                // File.AppendAllText(debugBotPath, "Changed bot file to 'c'." + System.Environment.NewLine);
            }
            else if (Rank > 15)
            {
                action = 'r';
                File.WriteAllText(BotToCasino, "r");
                //  File.AppendAllText(debugBotPath, "Changed bot file to 'r'." + System.Environment.NewLine);
            }
            else
            {
                File.WriteAllText(BotToCasino, "f");
                //   File.AppendAllText(debugBotPath, "Changed bot file to 'f'. I missed the Turn COMPLETELY" + System.Environment.NewLine);
                Program.Folded = true;
                return;
            }

            while (true)
            {
                if (FileManipulation.Listeners.BotFileChanged)
                {
                    FileManipulation.Listeners.BotFileChanged = false;
                    if (RiverFound())
                    {
                        // File.AppendAllText(debugBotPath, "River Found" + System.Environment.NewLine);
                        break;
                    }
                }
            }
        }

        private static bool RiverFound()
        {
            string text = null;
            while (true)
            {
                if (FileManipulation.Extractions.IsFileReady(CasinoToBot))
                {
                    try
                    {
                        text = System.IO.File.ReadAllText(CasinoToBot);
                        break;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            int index = 0;
            if (text == null)
                return false;

            if (text.Contains("R"))
            {
                foreach (var digit in text)
                {
                    ++index;
                    if (digit == 'R')
                    {
                        // File.AppendAllText(Program.DebugBotPath, "River found here =  " + text + System.Environment.NewLine);
                        RiverCard = Convert.ToInt32(Regex.Match(text.Substring(index), @"\d+").Value);
                        FileManipulation.CardTransform.WriteCommunityCards(RiverCard, 4);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
