using BluffinMuffin.HandEvaluator;
using System;
using System.IO;
using System.Linq;

namespace Samus
{
    public class Flopper
    {
        private static string BotPathFile = Program.BotPath;
        private static string[] FileText;
        
        

        private static bool isHandFinished;

        public static void Start(string path, int rank, int position, string debugBotPath) // path = play area path 
        {
            File.AppendAllText(debugBotPath, "\nEntered flop." + System.Environment.NewLine);
            //position 2 goes first
            //read flop
            //sort hand
            //get rank
            //check draws
            //check or bet
            //go turning*
            
            FileText = FileManipulation.Extractions.GetFileInfo(path);
            string[] cardNumbers = FileManipulation.Extractions.GetFlopCardNumbers(FileText); //trim start and finish
            FileManipulation.CardTransform.Flop(cardNumbers, ref Program.CommunityCards); //format = KJQ123 -> now K1J2Q3 -> community cards being set inside.

            File.AppendAllText(debugBotPath, string.Format("Read Flop cards: {0} {1} {2}", Program.CommunityCards[0], Program.CommunityCards[1], Program.CommunityCards[2]) +System.Environment.NewLine);
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
                if (FileManipulation.Extractions.FileIsReady(BotPathFile))
                {
                    if (rank < 54)
                    {
                        File.WriteAllText(BotPathFile, "r");
                        File.AppendAllText(debugBotPath, "Changed bot file to 'r'." + System.Environment.NewLine);
                        //++RaisesFound;

                        break;
                    }
                    else if (rank < 93)
                    {
                        File.AppendAllText(debugBotPath, "Changed bot file to 'c'." + System.Environment.NewLine);
                        File.WriteAllText(BotPathFile, "c");
                        break;
                    }
                    
                    {
                        File.AppendAllText(debugBotPath, "Changed bot file to 'c'.  this is for testing purposes only. this is actually a fold. " + System.Environment.NewLine);
                        File.WriteAllText(BotPathFile, "c");//change to fold after testing
                        //System.Environment.Exit(0);
                        isHandFinished = true;
                        break;
                    }
                }   //TODO
            }
            
        }
    }
}
