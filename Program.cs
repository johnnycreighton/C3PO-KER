using System;
using System.Collections.Generic;
using System.Linq;
using static System.String;
using BluffinMuffin.HandEvaluator;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using Samus.Streets;


//using System.ValueTuple;

namespace Samus
{
    public class Program
    {
        public static Samus.Player Samus = new Samus.Player("Samus");
        public static string DebugBotPath = @"D:\4th Year CSSE\Samus\MosersCasino\DebugBot.txt";
        public static string CasinoToBot = @"D:\4th Year CSSE\Samus\MosersCasino\botFiles\casinoToBot1"; //not needed so far

        public static string BotToCasino = @"D:\4th Year CSSE\Samus\MosersCasino\botFiles\botToCasino1";


        public static int[] FlopCards = new int[3];

        public static string PlayAreaPathEven = @"D:\4th Year CSSE\Samus\Cashino\PokerTesterGCC-master\simulationFiles\playAreaPathEven.txt";
        public static string PlayAreaPathOdd = @"D:\4th Year CSSE\Samus\Cashino\PokerTesterGCC-master\simulationFiles\playAreaPathOdd.txt";

        public static string BotDirPath = @"D:\4th Year CSSE\Samus\MosersCasino\botFiles";

        public static string PlayAreaDirPath = @"D:\4th Year CSSE\Samus\Cashino\PokerTesterGCC-master\simulationFiles";

        private static int RaisesFound = 0;
        private static bool HandFetched;
        private static bool HandFinished;

        private static string[] lines = new string[100];
        private static string playAreaLocation;

        public static bool MainAllIn;
        public static bool MainFold;
        public static int SmallBlind = 25;
        public static int BigBlind = 50;
        public static int Pot;
        public static string Path; // only for single hands run.

        public static string Hand;
        public static int i = -1;

        private static bool FirstAction = true;

        public static int Counter = 0;

        public static object[] Cards = new object[2];
        public static object[] Suits = new object[2];
        public static int DealerPosition;

        public static int rank;
        private static int handNumber = 1;

        public static int MyWinnings;
        public static int OpponentsWinnings;

        public static String[] CommunityCards = new String[5];

        public static Hashtable ranking = new Hashtable();
        private static bool firstRound = true;

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
            FileManipulation.Listeners.StartSummaryFileWatcher(BotDirPath); //working
            File.WriteAllText(DebugBotPath, ""); //Emptying Debugger for new game
            File.AppendAllText(DebugBotPath, "************************* Main Method Invoked *************************" + System.Environment.NewLine);
            TwoCardRanking.PopulateHashTable(ranking); //generates ranking of all pre-flop hands based on average money won per hand

            FileManipulation.Listeners.StartCasinoWatcher(BotDirPath);

            while (true)
            {
                HandFinished = false;
               // FileManipulation.Listeners.BotFileChanged = false;
                File.AppendAllText(DebugBotPath, "Bot watcher started." + System.Environment.NewLine);
                File.AppendAllText(DebugBotPath, "\nHand Number: " + ++Counter + "\nBotFileChanged = true." + System.Environment.NewLine);
                while (true)//first action pre-flop
                {
                    if (FileManipulation.Listeners.BotFileChanged)
                    {
                        FileManipulation.Listeners.BotFileChanged = false;
                        BotEventFired(); //gets hand + Position
                        if (HandFetched)
                        {
                            FstAction(); // first to act heads up pre-flop
                            File.AppendAllText(DebugBotPath, "BotFileChanged = false.\nFirst actions have been performed." + System.Environment.NewLine);
                            break;
                        }
                    }
                }
                while (true)
                {
                    if (FileManipulation.Listeners.BotFileChanged)
                    {
                        FileManipulation.Listeners.BotFileChanged = false;
                        if (FlopFound())
                        {
                            File.AppendAllText(DebugBotPath, "Flop Found" + System.Environment.NewLine);
                            break;
                        }
                    }
                }
                File.AppendAllText(DebugBotPath, string.Format("\nHeaded for the flop with cards {0} {1}", Samus.FirstCard, Samus.SecondCard) + System.Environment.NewLine);
                if (!HandFinished)
                {
                    Flopper.Start(FlopCards, rank, DealerPosition, DebugBotPath);
                }
                if (!HandFinished)
                {
                    Turner.Start(CommunityCards, rank, DealerPosition, DebugBotPath);
                }
                if (!HandFinished)
                {
                    River.Start(CommunityCards, rank, DealerPosition, DebugBotPath);
                }
                
                File.AppendAllText(DebugBotPath, string.Format(System.Environment.NewLine + "*************************** Hand Finished *************************** ") + System.Environment.NewLine);
            }
        }
        //private static string text1 = null;
        private static bool FlopFound()
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
            
            if (text.Contains("F"))
            {
                int index = 0;
                int position = 0;
                foreach (var digit in text)
                {
                    ++index;
                    if (digit == 'F')
                    {
                        var f = text.Substring(index);
                        FlopCards[position++] = Convert.ToInt32(Regex.Match(text.Substring(index), @"\d+").Value);
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

        private static void FstAction()
        {
            File.AppendAllText(DebugBotPath, "First action" + System.Environment.NewLine);

            while (true)
            {
                if (rank < 54 && RaisesFound < 4)
                {
                    File.AppendAllText(DebugBotPath, "Changed bot file to 'r'." + System.Environment.NewLine);
                    File.WriteAllText(BotToCasino, "r");
                    ++RaisesFound;

                    break;
                }
                else if (rank < 93 || RaisesFound == 3)
                {
                    File.AppendAllText(DebugBotPath, "Changed bot file to 'c'." + System.Environment.NewLine);
                    File.WriteAllText(BotToCasino, "c");
                    break;
                }
                else
                {
                    File.AppendAllText(DebugBotPath, "Changed bot file to 'f'." + System.Environment.NewLine);
                    File.WriteAllText(BotToCasino, "f");
                    //System.Environment.Exit(0); //for single hand termination
                    HandFinished = true;
                    break;
                }
            }
        }

        private static void BotEventFired() //clean
        {
            string text = null;
            while (true)
            {
                if (FileManipulation.Extractions.IsFileReady(CasinoToBot))
                {
                    text = System.IO.File.ReadAllText(CasinoToBot); //reading bot file
                    break;
                }
            }
            if(!text.Contains("A")) // FIX
            {
                HandFetched = false;
                return;
            }
            Hand = GetWholeCardsAndPosition(text);

            HandFetched = true;
            File.AppendAllText(DebugBotPath, "Bot File ready.\nCards = \t" + text + "\nHole cards converted: " + Hand + System.Environment.NewLine);
            foreach (DictionaryEntry elem in ranking) //Gets Rank
            {
                if (Hand[0] == Hand[1])//different algo for pairs.
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
                HandFetched = true;
                File.AppendAllText(DebugBotPath, "Rank Found:\t" + rank + System.Environment.NewLine);
            }
            else
                throw new Exception("No rank found, sort out");
        }

        /*
        private static void PopulatePlayer(Samus.Player johnny)
        {
            i = -1;

            foreach (var element in Cards)
            {
                ++i;
                switch (element.ToString())
                {
                    case "0":
                        if (i == 0)
                        {
                            johnny.FirstCardTest += "2";
                            continue;
                        }
                        else
                        {
                            johnny.SecondCardTest += "2";
                            continue;
                        }

                    case "1":
                        if (i == 0)
                        {
                            johnny.FirstCardTest += "3";
                            continue;
                        }
                        else
                        {
                            johnny.SecondCardTest += "3";
                            continue;
                        }

                    case "2":
                        if (i == 0)
                        {
                            johnny.FirstCardTest += "4";
                            continue;
                        }
                        else
                        {
                            johnny.SecondCardTest += "4";
                            continue;
                        }

                    case "3":
                        if (i == 0)
                        {
                            johnny.FirstCardTest += "5";
                            continue;
                        }
                        else
                        {
                            johnny.SecondCardTest += "5";
                            continue;
                        }

                    case "4":
                        if (i == 0)
                        {
                            johnny.FirstCardTest += "6";
                            continue;
                        }
                        else
                        {
                            johnny.SecondCardTest += "6";
                            continue;
                        }

                    case "5":
                        if (i == 0)
                        {
                            johnny.FirstCardTest += "7";
                            continue;
                        }
                        else
                        {
                            johnny.SecondCardTest += "7";
                            continue;
                        }

                    case "6":
                        if (i == 0)
                        {
                            johnny.FirstCardTest += "8";
                            continue;
                        }
                        else
                        {
                            johnny.SecondCardTest += "8";
                            continue;
                        }

                    case "7":
                        if (i == 0)
                        {
                            johnny.FirstCardTest += "9";
                            continue;
                        }
                        else
                        {
                            johnny.SecondCardTest += "9";
                            continue;
                        }

                    case "8":
                        if (i == 0)
                        {
                            johnny.FirstCardTest += "10";
                            continue;
                        }
                        else
                        {
                            johnny.SecondCardTest += "10";
                            continue;
                        }

                    case "9":
                        if (i == 0)
                        {
                            johnny.FirstCardTest += "J";
                            continue;
                        }
                        else
                        {
                            johnny.SecondCardTest += "J";
                            continue;
                        }

                    case "10":
                        if (i == 0)
                        {
                            johnny.FirstCardTest += "Q";
                            continue;
                        }
                        else
                        {
                            johnny.SecondCardTest += "Q";
                            continue;
                        }

                    case "11":
                        if (i == 0)
                        {
                            johnny.FirstCardTest += "K";
                            continue;
                        }
                        else
                        {
                            johnny.SecondCardTest += "K";
                            continue;
                        }

                    case "12":
                        if (i == 0)
                        {
                            johnny.FirstCardTest += "A";
                            continue;
                        }
                        else
                        {
                            johnny.SecondCardTest += "A";
                            continue;
                        }

                    default:
                        throw new ArgumentOutOfRangeException("Card value does not exist. Re-check input.");

                }
            }

            i = -1;
            foreach (var element in Suits)
            {
                ++i;
                switch (element.ToString())
                {
                    case "0":
                        if (i == 0)
                        {
                            johnny.FirstCardTest += "s";
                            continue;
                        }
                        else
                        {
                            johnny.SecondCardTest += "s";
                            continue;
                        }

                    case "1":
                        if (i == 0)
                        {
                            johnny.FirstCardTest += "c";
                            continue;
                        }
                        else
                        {
                            johnny.SecondCardTest += "c";
                            continue;
                        }

                    case "2":
                        if (i == 0)
                        {
                            johnny.FirstCardTest += "h";
                            continue;
                        }
                        else
                        {
                            johnny.SecondCardTest += "h";
                            continue;
                        }

                    case "3":
                        if (i == 0)
                        {
                            johnny.FirstCardTest += "d";
                            continue;
                        }
                        else
                        {
                            johnny.SecondCardTest += "d";
                            continue;
                        }

                    default:
                        throw new ArgumentOutOfRangeException("Suit value does not exist. Re-check input.");
                }

            }
            i = -1;
        }

        */

        private static string GetWholeCardsAndPosition(string text)
        {
            int firstCard = 0;
            int secondCard = 0;
            var index = 0;

            foreach (var digit in text)
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


            Cards[0] = firstCard / 4;
            Cards[1] = secondCard / 4;

            Suits[0] = firstCard % 4;
            Suits[1] = secondCard % 4;


            Samus.SetPreFlopCards(Cards, Suits);

            string hand = null;
            if (Samus.FirstCard.face == "10" && Samus.SecondCard.face == "10")
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

            HandFetched = true; // MAY NOT NEED THIS 
            return hand;
        }

        /*
        
            IStringCardsHolder[] players1 =
            {
                new Program.Player("Johnny", deck1.DealCard().ToString(), deck1.DealCard().ToString(), CommunityCards[0], CommunityCards[1], CommunityCards[2], CommunityCards[3], CommunityCards[4]),
                new Player("Coralie", deck1.DealCard().ToString(), deck1.DealCard().ToString(), CommunityCards[0], CommunityCards[1], CommunityCards[2], CommunityCards[3], CommunityCards[4] ),
            };

            // foreach (var p in HandEvaluators.Evaluate(players1))
            // {
            //   
            //     //Console.WriteLine("{0}: {1} -> {2}" + "    " + count, p.Rank == int.MaxValue ? "  " : p.Rank.ToString(), ((Player)p.CardsHolder).Name, p.Evaluation);
            // }

            foreach (var p in HandEvaluators.Evaluate(players1).SelectMany(x => x))
            {
               
                    break;
                }
            }*/
    }
}
