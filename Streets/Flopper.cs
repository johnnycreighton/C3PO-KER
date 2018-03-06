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

        public static int RaisesFound { get; private set; }

        public static void Start(int[] cardNumbers, int rank, int position, string debugBotPath) // path = play area path 
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


            while (true)
            {
                 if (FileManipulation.Listeners.BotFileChanged)
                {
                    FileManipulation.Listeners.BotFileChanged = false;
                    
                    if (rank < 54) //TODO sort his out to have some real strategy
                    {
                        File.WriteAllText(BotToCasino, "r");
                        File.AppendAllText(debugBotPath, "Changed bot file to 'r'." + System.Environment.NewLine);
                        ++RaisesFound;
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
                        isHandFinished = true;                                //System.Environment.Exit(0);
                        break;
                    }
                }
            }
            while(true)
            {
                if (FileManipulation.Listeners.BotFileChanged)
                {
                    FileManipulation.Listeners.BotFileChanged = false;
                    if (TurnFound())
                    {
                        File.AppendAllText(DebugBotPath, "Turn Found" + System.Environment.NewLine);
                        break;
                    }
                }
            }
        }

        private static bool TurnFound()
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
            if (text.Contains("T"))
            {
                
                foreach (var digit in text)
                {
                    ++index;
                    if (digit == 'T')
                    {
                        
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
