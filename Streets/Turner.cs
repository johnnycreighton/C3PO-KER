using BluffinMuffin.HandEvaluator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Samus.Streets
{
    class Turner
    {
        private static string CasinoToBot = Program.CasinoToBot;
        private static string BotToCasino = Program.BotToCasino;
        

        public static int RiverCard;

        internal static void Start(string[] path, int rank, int position, string debugBotPath)
        {
            File.AppendAllText(debugBotPath, "\nEntered Turn." + System.Environment.NewLine);

            File.AppendAllText(debugBotPath, string.Format("Read Turn card: {0} {1} {2} {3}", Program.CommunityCards[0], Program.CommunityCards[1], Program.CommunityCards[2], Program.CommunityCards[3]) + System.Environment.NewLine);
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
            File.AppendAllText(debugBotPath, string.Format("Best five card hand post turn: {0}", bestFiveCarder + System.Environment.NewLine));
            HandStrategies.Draws.CheckForDraws(Program.Samus, Program.CommunityCards);
            File.AppendAllText(debugBotPath, "Checked for Draws!" + System.Environment.NewLine);

            while (true)
            {
                if (FileManipulation.Listeners.BotFileChanged)
                {
                    FileManipulation.Listeners.BotFileChanged = false;

                    if (rank < 54) //TODO sort his out to have some real strategy
                    {
                        File.WriteAllText(BotToCasino, "r");
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
                        File.AppendAllText(debugBotPath, "Changed bot file to 'f'" + System.Environment.NewLine);
                        File.WriteAllText(BotToCasino, "f");//change to fold after testing
                                                      //System.Environment.Exit(0);
                        break;
                    }
                }
            }
            while (true)
            {
                if (FileManipulation.Listeners.BotFileChanged)
                {
                    FileManipulation.Listeners.BotFileChanged = false;
                    if (RiverFound())
                    {
                        File.AppendAllText(debugBotPath, "River Found" + System.Environment.NewLine);
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
                    text = System.IO.File.ReadAllText(CasinoToBot);
                    
                    break;
                }
            }

            int index = 0;
            if (text.Contains("R"))
            {

                foreach (var digit in text)
                {
                    ++index;
                    if (digit == 'R')
                    {
                        File.AppendAllText(Program.DebugBotPath, "River found here =  " + text + System.Environment.NewLine);
                        RiverCard = Convert.ToInt32(Regex.Match(text.Substring(index), @"\d+").Value);
                        FileManipulation.CardTransform.WriteCommunityCards(RiverCard, 4);
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
