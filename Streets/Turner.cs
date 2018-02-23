using BluffinMuffin.HandEvaluator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samus.Streets
{
    class Turner
    {
        private static string BotPathFile = Program.BotPath;
        private static string[] FileText;

        private static bool isHandFinished;


        internal static void Start(string path, int rank, int position, string debugBotPath)
        {

            File.AppendAllText(debugBotPath, "\nEntered Turn." + System.Environment.NewLine);
            //position 2 goes first
            //read flop
            //sort hand
            //get rank
            //check draws
            //check or bet
            //go turning*

            System.Threading.Thread.Sleep(200);

            FileText = FileManipulation.Extractions.GetFileInfo(path);
            int turnCardNumber = FileManipulation.Extractions.GetTurnCardNumber(FileText); //trim start and finish
            FileManipulation.CardTransform.Turn(turnCardNumber, ref Program.CommunityCards);  //community cards being set inside.

            File.AppendAllText(debugBotPath, string.Format("Read Turn card: {0}", Program.CommunityCards[3]) + System.Environment.NewLine);
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
                    else
                    {
                        File.AppendAllText(debugBotPath, "Changed bot file to 'c' only for testing purposes this is async ctually a fold." + System.Environment.NewLine);
                        File.WriteAllText(BotPathFile, "c"); // change to fold after testing.
                        //System.Environment.Exit(0);
                        //isHandFinished = true;
                        break;
                    }
                }   //TODO
            }
        }
    }
}
