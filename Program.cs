using System;
using System.Collections.Generic;
using System.Linq;
using static System.String;
using BluffinMuffin.HandEvaluator;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using Samus.Streets;

namespace Samus
{
    public class Program : FileManipulation.Listeners
    {
        public static Samus.Player Samus = new Samus.Player("Samus"); //player creation

       // public static string DebugBotPath = @"Specify path to casino files";
        public static string CasinoToBot = @"Specify Path to casino files";
        public static string BotToCasino = @"Specify Path to casino files";
        public static string BotDirPath = @"Specify Path to casino files";
        public static string Hand;

        public static int[] FlopCards = new int[3]; //low level card numbers
        
        private static bool HandFetched;
        public static bool Folded;

        private static string[] lines = new string[100];
        public static string[] CommunityCards = new String[5];

        public static int Counter;
        public static int DealerPosition;
        public static int rank;

        public static object[] Cards = new object[2];
        public static object[] Suits = new object[2];
        
        public static Hashtable ranking = new Hashtable();

        public class Player : IStringCardsHolder
        {
            public string Name { get; }
            private string[] Cards { get; }

            public Player(string name, params string[] cards)
            {
                Name = name;
                Cards = cards;
            }

            public override string ToString()
            {
                return $"{Name}: [{Join(",", Cards)}]";
            }

            public IEnumerable<string> PlayerCards => Cards.Take(2);

            public IEnumerable<string> CommunityCards => Cards.Skip(2);
        }

        private static void Main()
        {   
            //Debugger files everywhere to track actions and possible bugs // all commented out for speed in tournament.
            // File.WriteAllText(DebugBotPath, ""); //Emptying Debugger for new game
            // File.AppendAllText(DebugBotPath, "************************* Main Method Invoked *************************" + System.Environment.NewLine);
            
            TwoCardRanking.PopulateHashTable(ranking); //generates ranking of all pre-flop hands based on average money won per hand
            StartCasinoWatcher(BotDirPath); //start watching file

            while (true) // loop forever to play infinite hands.
            {
                action = 'g'; //used so bot knows what decision it made prior
                Samus.BackDoorFlushDraw = false;
                Samus.BackDoorStraightDraw = false;
                Samus.FlushDraw = false;
                Samus.GutShotStraightDraw = false;
                Samus.OpenEndedStraightDraw = false; 

                if (CommunityCards[0] != null)
                {
                    for (int i = 0; i < CommunityCards.Length; i++)
                    {
                        CommunityCards[i] = null;
                    }
                }
                Folded = false;
                HandFetched = false; //resetting all variables

                //File.AppendAllText(DebugBotPath, "\n\n\n\nNew Hand, Number: " + Counter + "\n" +System.Environment.NewLine);

                while (true)//first action pre-flop, wait for cards
                {
                    if (BotFileChanged) //this changes when bot file has been written to
                    {
                        BotFileChanged = false;
                        HandFetched = false;
                        BotEventFired(Counter); //gets hand + Position
                        if (HandFetched)
                        {
                            FirstAction(); // first to act
                            // File.AppendAllText(DebugBotPath, "First actions have been performed." + System.Environment.NewLine);
                            break;
                        }
                    }
                }
                if (!Folded) //if not folded continue
                {
                    while (true)
                    {
                        if (BotFileChanged) //waiting for flop
                        {
                            BotFileChanged = false;
                            if (FlopFound())
                            {
                                //  File.AppendAllText(DebugBotPath, "Flop Found" + System.Environment.NewLine);
                                //  File.AppendAllText(DebugBotPath, Format("\nHeaded for the flop with cards {0} {1}", Samus.FirstCard, Samus.SecondCard) + System.Environment.NewLine);
                                Flopper.Start(FlopCards, rank, DealerPosition, action);
                                break;
                            }
                        }
                    }
                }
                if (!Folded)
                {
                    Turner.Start(CommunityCards, rank, DealerPosition);
                }
                if (!Folded)
                {
                    River.Start(CommunityCards, rank, DealerPosition, Turner.action);
                }
            }
        }

        private static bool FlopFound()
        {
            string text = null;
            here:
            if (FileManipulation.Extractions.IsFileReady(CasinoToBot)) // check if file is ready to read
            {
                try
                {
                    text = System.IO.File.ReadAllText(CasinoToBot);
                }
                catch
                { goto here; }

            }
            else
            {
                return false; // go back to loop and continue waiting
            }

            if (text.Contains("F")) //Flop found here
            {
                int index = 0;
                int position = 0;
                foreach (var digit in text)
                {
                    ++index;
                    if (digit == 'F')
                    {
                        var f = text.Substring(index);
                        FlopCards[position++] = Convert.ToInt32(Regex.Match(text.Substring(index), @"\d+").Value); //setting flop cards
                        if (position == 3)
                        {
                            break;
                        }
                    }
                }
                return true;
            }
            else
                return false;
        }

        public static char action;

        private static void FirstAction()
        {
            while (true)
            {
                if (rank < 48) //previous == 54
                {
                    action = 'r';
                    // File.AppendAllText(DebugBotPath, "Changed bot file to 'r'." + System.Environment.NewLine);
                    File.WriteAllText(BotToCasino, "r");
                    break;
                }
                else if (rank < 111)//previous == 93
                {
                    action = 'c';
                    // File.AppendAllText(DebugBotPath, "Changed bot file to 'c'." + System.Environment.NewLine);
                    File.WriteAllText(BotToCasino, "c");
                    break;
                }
                else
                {
                    //  File.AppendAllText(DebugBotPath, "Changed bot file to 'f' Exiting hand." + System.Environment.NewLine);
                    File.WriteAllText(BotToCasino, "f");
                    Folded = true;
                    System.Threading.Thread.Sleep(50); // waiting for hand to be played
                    break;
                }
            }
        }

        private static void BotEventFired(int counter) 
        {
            string text = null;
            while (true)
            {
                if (FileManipulation.Extractions.IsFileReady(CasinoToBot))
                {
                    try
                    {
                        text = System.IO.File.ReadAllText(CasinoToBot); //reading bot file
                        break;
                    }
                    catch
                    {
                        continue; //if error, even after checking if ready, continue on the loop and try again
                    }
                }
            }
            
            if (!text.Contains("A")) // If A is not in the text cards are available to read, as per casino protocol
            {
                // File.AppendAllText(DebugBotPath, "Hand not available " + text + System.Environment.NewLine);
                HandFetched = false;
                return; //returning because cards are not here
            }

            Hand = GetWholeCardsAndPosition(text);
            HandFetched = true;
            ++Counter;
            //File.AppendAllText(DebugBotPath, "Bot File ready.\nCards = \t" + text + "\nHole cards converted: " + Hand + System.Environment.NewLine);
            foreach (DictionaryEntry elem in ranking) //Gets Rank
            {
                if (Hand[0] == Hand[1])//different algorithm for pairs.
                {
                    if (elem.Value.ToString().Substring(0, 1).Contains(Hand[0]) && elem.Value.ToString().Substring(1, 1).Contains(Hand[1]))
                    {
                        rank = Convert.ToInt32(elem.Key);
                        break;
                    }
                }
                else
                {
                    if (elem.Value.ToString().Contains(Hand[0]) && elem.Value.ToString().Contains(Hand[1]) && elem.Value.ToString().Contains(Hand[2]))
                    {
                        rank = Convert.ToInt32(elem.Key);
                        break;
                    }
                }
            }
            if (rank > 0)
            {
                // File.AppendAllText(DebugBotPath, "Rank Found:\t" + rank + System.Environment.NewLine); // degging only
            }
            else
                throw new Exception("No rank found, Further testing needed on hash table elements");
        }
        
        private static string GetWholeCardsAndPosition(string text)
        {
            int firstCard = 0;
            int secondCard = 0;
            var index = 0;

            foreach (var digit in text) //extract needed information
            {
                ++index;
                if (digit == 'D')
                {
                    DealerPosition = Convert.ToInt32(Regex.Match(text.Substring(index), @"\d+").Value);
                }
                if (digit == 'A')
                {
                    firstCard = Convert.ToInt32(Regex.Match(text.Substring(index), @"\d+").Value); //grabs first integar from the start of the substring beginning A
                }
                else if (digit == 'B')
                {
                    secondCard = Convert.ToInt32(Regex.Match(text.Substring(index), @"\d+").Value);
                    break; //only break here not at A.
                }
            }

            Cards[0] = firstCard / 4; //card exposition as per casino protocol
            Cards[1] = secondCard / 4;

            Suits[0] = firstCard % 4;
            Suits[1] = secondCard % 4;

            Samus.SetPreFlopCards(Cards, Suits);

            string hand = null;
            if (Samus.FirstCard.face == "10" && Samus.SecondCard.face == "10") //some madness with tens in build so checks will be made regularly
            {
                hand = "TT";
            }
            else if (Samus.FirstCard.face == "10")
            {
                hand = "T" + Samus.SecondCard.face;
            }
            else if (Samus.SecondCard.face == "10")
            {
                hand = Samus.FirstCard.face + "T";
            }
            else
            {
                hand = Samus.FirstCard.face + Samus.SecondCard.face;
            }

            if (Samus.FirstCard.suit == Samus.SecondCard.suit)
            {
                hand += "s";
            }
            else
                hand += "o";

            return hand;
        }
    }
}