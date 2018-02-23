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

        public static string DebugBotPath = @"D:\4th Year CSSE\Samus\DebugBot.txt";
        public static string BotPath = @"D:\4th Year CSSE\Samus\Cashino\PokerTesterGCC-master\simulationFiles\bots\bot1.txt";
         
        public static string PlayAreaPathEven = @"D:\4th Year CSSE\Samus\Cashino\PokerTesterGCC-master\simulationFiles\playAreaPathEven.txt";
        public static string PlayAreaPathOdd = @"D:\4th Year CSSE\Samus\Cashino\PokerTesterGCC-master\simulationFiles\playAreaPathOdd.txt";
        
        public static string BotDirPath = @"D:\4th Year CSSE\Samus\Cashino\PokerTesterGCC-master\simulationFiles\bots";
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
        public static int Position;

        public static int rank;
        private static int handNumber = 1;

        public static int MyWinnings;
        public static int OpponentsWinnings;

        public static String[] CommunityCards = new String[5];

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
            FileManipulation.Listeners.StopWatcher(PlayAreaPathOdd);
            FileManipulation.Listeners.StopWatcher(PlayAreaPathEven);

            FileManipulation.Listeners.StartPlayAreaEvenWatcher(PlayAreaDirPath);
            FileManipulation.Listeners.StartPlayAreaOddWatcher(PlayAreaDirPath);
            FileManipulation.Listeners.StartBotWatcher(BotDirPath);
            File.AppendAllText(DebugBotPath, "Bot watcher started And both play areas watchers started." + System.Environment.NewLine);
            File.AppendAllText(DebugBotPath, "Starting both play area bot watchers." + System.Environment.NewLine);

            File.WriteAllText(DebugBotPath, ""); //Emptying Debugger for new game
            File.AppendAllText(DebugBotPath, "Main Method Invoked." + System.Environment.NewLine);
            
            TwoCardRanking.PopulateHashTable(ranking); //generates ranking of all pre-flop hands based on average money won per hand


            //File.AppendAllText(DebugBotPath, "Started." + System.Environment.NewLine);

            Path = GetPath();
            while (true)//Get position
            {
                lines = GetText();
                if (WaitForText("bot1", lines)) //will ensure position is found
                {
                    FileManipulation.Listeners.PlayAreaOddFileChanged = false;
                    FileManipulation.Listeners.PlayAreaEvenFileChanged = false;
                    Samus.position = GetPosition(); // players position found and play area path found for later use
                    break;
                }
            }
            while(true)
            { 
                if (FileManipulation.Listeners.BotFileChanged)
                {
                    FileManipulation.Listeners.StopWatcher(BotDirPath); //stopping bot watcher as we do not need to listen anymore.
                    File.AppendAllText(DebugBotPath, "\nHand Number: " + ++Counter + "\nBotFileChanged = true." + System.Environment.NewLine);
                    BotEventFired(); // gets hand + rank
                    break;
                }
            }
            switch (Samus.position)
            {
                case 1:
                    ButtonFirstAction(); // first to act heads up pre-flop
                    break;

                case 2:
                    BigBlindFirstAction();
                    break;

                default:
                    throw new NotImplementedException(); // act accordingly.
            }

            FileManipulation.Listeners.StopWatcher(PlayAreaPathOdd);
            FileManipulation.Listeners.StopWatcher(PlayAreaPathEven);
            File.AppendAllText(DebugBotPath, "BotFileChanged = false.\nFirst actions have been performed." + System.Environment.NewLine);
            
           // FileManipulation.Listeners.StartPlayAreaEvenWatcher(PlayAreaDirPath); 
           // FileManipulation.Listeners.StartPlayAreaOddWatcher(PlayAreaDirPath);


            // if(RaiseFound(GetPath())) //TODO when two bots compete, check for a raise to your call.
            // {
            //     
            // }

            if (IsHandFinished(Path))
            {
                File.AppendAllText(DebugBotPath, "Hand Finished sort this line out- what to do." + System.Environment.NewLine);
                return;
            }
            
            while (true)
            {
                
                    while (true)
                    {
                        if (FileManipulation.Listeners.PlayAreaEvenFileChanged)
                        {
                            //File.AppendAllText(DebugBotPath, "PlayAreaEvenFileChanged = true." + System.Environment.NewLine);
                            File.AppendAllText(DebugBotPath, string.Format("\nHeaded for the flop with cards {0} {1}", Samus.FirstCard, Samus.SecondCard) + System.Environment.NewLine);
                            Flopper.Start(Path, rank, Position, DebugBotPath);
                           

                            // File.AppendAllText(DebugBotPath, "PlayAreaEvenFileChanged = false." + System.Environment.NewLine);
                            FileManipulation.Listeners.PlayAreaEvenFileChanged = false;
                            FileManipulation.Listeners.StartPlayAreaEvenWatcher(PlayAreaDirPath);

                            break;
                        }
                        else if (FileManipulation.Listeners.PlayAreaOddFileChanged)
                        {
                            
                            File.AppendAllText(DebugBotPath, string.Format("\nHeaded for the flop with cards {0} {1}", Samus.FirstCard, Samus.SecondCard) + System.Environment.NewLine);
                            Flopper.Start(Path, rank, Position, DebugBotPath);
                           

                            // File.AppendAllText(DebugBotPath, "PlayAreaOddFileChanged = false." + System.Environment.NewLine);
                            FileManipulation.Listeners.PlayAreaOddFileChanged = false;
                            FileManipulation.Listeners.StartPlayAreaOddWatcher(PlayAreaDirPath);
                            break;
                        }
                    }
                    
                
                //******************************************************************************************************************
            }
            while (true)
            {
                //******************************************************************************************************************
                if (FileManipulation.Listeners.PlayAreaEvenFileChanged && !FileManipulation.Listeners.PlayAreaOddFileChanged)
                {
                    //File.AppendAllText(DebugBotPath, "PlayAreaEvenFileChanged = true." + System.Environment.NewLine);
                    File.AppendAllText(DebugBotPath, string.Format("\nHeaded for the turn with best hand {0}", Samus.Hand) + Environment.NewLine);
                    Turner.Start(Path, rank, Position, DebugBotPath);
                    // PlayAreaEventFired(PlayAreaPathEven);

                   // File.AppendAllText(DebugBotPath, "PlayAreaEvenFileChanged = false." + System.Environment.NewLine);
                    FileManipulation.Listeners.PlayAreaEvenFileChanged = false;
                    FileManipulation.Listeners.StartPlayAreaEvenWatcher(PlayAreaDirPath);

                    break;
                }
                else if (FileManipulation.Listeners.PlayAreaOddFileChanged && !FileManipulation.Listeners.PlayAreaEvenFileChanged)
                {
                   // File.AppendAllText(DebugBotPath, "PlayAreaOddFileChanged = true." + System.Environment.NewLine);
                    File.AppendAllText(DebugBotPath, string.Format("\nHeaded for the turn with best hand {0}", Samus.Hand) + Environment.NewLine);
                    Turner.Start(Path, rank, Position, DebugBotPath);
                    //PlayAreaOddEventFired(PlayAreaPathOdd);

                   // File.AppendAllText(DebugBotPath, "PlayAreaOddFileChanged = false." + System.Environment.NewLine);
                    FileManipulation.Listeners.PlayAreaOddFileChanged = false;
                    FileManipulation.Listeners.StartPlayAreaOddWatcher(PlayAreaDirPath);
                    break;
                }
                //******************************************************************************************************************
            }

            if (IsHandFinished(Path))
            {
                File.AppendAllText(DebugBotPath, "Hand Finished sort this line out- what to do." + System.Environment.NewLine);
                return;
            }
            while (true)
            {
                //******************************************************************************************************************
                if (FileManipulation.Listeners.PlayAreaEvenFileChanged && !FileManipulation.Listeners.PlayAreaOddFileChanged)
                {
                    //File.AppendAllText(DebugBotPath, "PlayAreaEvenFileChanged = true." + System.Environment.NewLine);
                    File.AppendAllText(DebugBotPath, string.Format("\nHeaded for the river with best hand {0}", Samus.Hand) + Environment.NewLine);
                    River.Start(Path, rank, Position, DebugBotPath);
                    // PlayAreaEventFired(PlayAreaPathEven);

                    //File.AppendAllText(DebugBotPath, "PlayAreaEvenFileChanged = false." + System.Environment.NewLine);
                    FileManipulation.Listeners.PlayAreaEvenFileChanged = false;
                    FileManipulation.Listeners.StartPlayAreaEvenWatcher(PlayAreaDirPath);

                    break;
                }
                else if (FileManipulation.Listeners.PlayAreaOddFileChanged && !FileManipulation.Listeners.PlayAreaEvenFileChanged)
                {
                    //File.AppendAllText(DebugBotPath, "PlayAreaOddFileChanged = true." + System.Environment.NewLine);
                    File.AppendAllText(DebugBotPath, string.Format("\nHeaded for the river with best hand {0}", Samus.Hand) + Environment.NewLine);
                    River.Start(Path, rank, Position, DebugBotPath);
                    //PlayAreaOddEventFired(PlayAreaPathOdd);

                    //File.AppendAllText(DebugBotPath, "PlayAreaOddFileChanged = false." + System.Environment.NewLine);
                    FileManipulation.Listeners.PlayAreaOddFileChanged = false;
                    FileManipulation.Listeners.StartPlayAreaOddWatcher(PlayAreaDirPath);
                    break;
                }
                //******************************************************************************************************************
            }


            


            while (true)
            {
                if (FileManipulation.Listeners.PlayAreaEvenFileChanged && !FileManipulation.Listeners.PlayAreaOddFileChanged)
                {
                    File.AppendAllText(DebugBotPath, "PlayAreaEvenFileChanged = true." + System.Environment.NewLine);

                    PlayAreaEventFired(PlayAreaPathEven);

                    File.AppendAllText(DebugBotPath, "PlayAreaEvenFileChanged = false." + System.Environment.NewLine);
                    FileManipulation.Listeners.PlayAreaEvenFileChanged = false;


                    FileManipulation.Listeners.StartPlayAreaEvenWatcher(PlayAreaDirPath);///////////////////////
                }
                else if (FileManipulation.Listeners.PlayAreaOddFileChanged && !FileManipulation.Listeners.PlayAreaEvenFileChanged)
                {
                    File.AppendAllText(DebugBotPath, "PlayAreaOddFileChanged = true." + System.Environment.NewLine);

                    PlayAreaOddEventFired(PlayAreaPathOdd);

                    File.AppendAllText(DebugBotPath, "PlayAreaOddFileChanged = false." + System.Environment.NewLine);
                    FileManipulation.Listeners.PlayAreaOddFileChanged = false;
                    FileManipulation.Listeners.StartPlayAreaOddWatcher(PlayAreaDirPath);
                }
                if (HandFinished)
                {
                    File.AppendAllText(DebugBotPath, "Hand finished, resetting variables. \n\n\n" + System.Environment.NewLine);
                    FileManipulation.Listeners.StartBotWatcher(BotDirPath);
                    FileManipulation.Listeners.StartPlayAreaEvenWatcher(PlayAreaDirPath);
                    FileManipulation.Listeners.StartPlayAreaOddWatcher(PlayAreaDirPath);
                    HandFinished = false;
                    HandFetched = false;
                    FirstAction = true;
                    Position = 0;
                    rank = 0;
                }
            }
            
        }

        private static bool WaitForText(string txt, string[] linesX)
        {
            foreach (string line in linesX)
            {
                if (line.Contains(txt))
                {
                    return true;
                }
            }
            return false;
        }

        private static string[] GetText()
        {
            int ddwe = 0;
            while (true)//check if file is ready to read.
            {
                if (FileManipulation.Extractions.FileIsReady(Path))
                {
                    return System.IO.File.ReadAllLines(Path);
                }
                ++ddwe;
            }
        }

        private static int GetPosition()
        {
            if(Position > 0)
            {
                return Position;
            }

            //Path = GetPath();
            lines = GetText();

            foreach (string line in lines) //try catch this
            {
                if (line.Contains("bot1"))
                {
                    char[] bot1 = { 'b', 'o', 't', '1' }; //remove the first int to allow for position int to be found.
                    string newLine = line.TrimStart(bot1);
                    //System.Text.RegularExpressions.Regex
                    //  \d+ is the first integer occurrence in Regex.  
                    Position = Convert.ToInt32(Regex.Match(newLine, @"\d+").Value);
                    File.AppendAllText(DebugBotPath, "Position Found: " + Position +"\t"+ line+System.Environment.NewLine);
                    break;
                }
            }
            return Position;
        }

        private static string GetPath()
        {
            while (true)
            {
                if (FileManipulation.Listeners.PlayAreaOddFileChanged)
                {
                    playAreaLocation = "Odd";
                    return PlayAreaPathOdd;
                }
                else if (FileManipulation.Listeners.PlayAreaEvenFileChanged)
                {
                    playAreaLocation = "Even";
                    return PlayAreaPathEven;
                }
            }
        }

        private static void PlayAreaOddEventFired(string path)
        {
            IsHandFinished(path);
            File.AppendAllText(DebugBotPath, "Play Area Odd Event fired." + System.Environment.NewLine);

            File.AppendAllText(DebugBotPath, "Stopping play area bot odd watcher." + System.Environment.NewLine);

            FileManipulation.Listeners.StopWatcher(PlayAreaPathOdd);

            int resultString = GetPosition();


            //switch (resultString)
            //{
            //    case 1:
            //        ButtonAction(); // first to act heads up pre-flop
            //        break;
            //
            //    case 2:
            //        BigBlindAction(path);
            //        break;
            //
            //    default:
            //        throw new NotImplementedException(); // act accordingly.
            //}
        }

        private static void PlayAreaEventFired(string path)
        {

            File.AppendAllText(DebugBotPath, "Play Area Even Event fired." + System.Environment.NewLine);

            File.AppendAllText(DebugBotPath, "Stopping play area bot even watcher." + System.Environment.NewLine);
            FileManipulation.Listeners.StopWatcher(path);

            int resultString = GetPosition();

            
            //switch (resultString)
            //{
            //    case 1:
            //        ButtonAction(); // first to act heads up pre-flop
            //        break;
            //
            //    case 2:
            //        BigBlindAction(path);
            //        break;
            //
            //    default: 
            //        throw new NotImplementedException(); // act accordingly.
            //}
        }

        

        private static bool RaiseFound(string path)
        {
            if(path == null)
            {
                return false;
            }
            while (true)
            {
                if (FileManipulation.Extractions.FileIsReady(Path))
                {
                    lines = System.IO.File.ReadAllLines(path);
                    break;
                }
            }
            bool raise = false;

            foreach (string line in lines) //try catch this
            {
                if (line.Contains("bot2") && line.Contains("raised"))
                {
                    raise = true;
                    ++RaisesFound;
                    File.AppendAllText(DebugBotPath, "Raise found.\nStopping play area Even watcher." + System.Environment.NewLine);
                    FileManipulation.Listeners.StopWatcher(path); // should be both.
                }
                
            }
            return raise;
        }

        private static void BigBlindFirstAction()
        {
            //string path = GetPath();

            File.AppendAllText(DebugBotPath, "Big blind first action." + System.Environment.NewLine);
            //*RaiseFound
            while (true)//check if file is ready to read.
            {
                if (FileManipulation.Extractions.FileIsReady(BotPath))
                {
                    if (rank < 35) //careful of number of raises -> check 
                    {
                        if (RaisesFound < 4)
                        {
                            File.AppendAllText(DebugBotPath, "Changing bot file to 'r'." + System.Environment.NewLine);
                            File.WriteAllText(BotPath, "r");
                            ++RaisesFound;
                            break;
                        }
                        else
                        {
                            File.AppendAllText(DebugBotPath, "Changing bot file to 'c'." + System.Environment.NewLine);
                            File.WriteAllText(BotPath, "c");
                            break;
                        }

                    }
                    else if (rank > 120 && RaiseFound(Path))
                    {
                        File.AppendAllText(DebugBotPath, "Changing bot file to 'f' because rank is bad and raise found." + System.Environment.NewLine);
                        File.WriteAllText(BotPath, "f");
                       // System.Environment.Exit(0);
                        HandFinished = true;
                        break;
                    }
                    else
                    {
                        File.AppendAllText(DebugBotPath, "Changing bot file to 'c'." + System.Environment.NewLine);
                        File.WriteAllText(BotPath, "c");//working
                        break;
                    }
                }
            }
            // File.AppendAllText(DebugBotPath, "\n" + File.ReadAllLines(BotPath) + "\n" + System.Environment.NewLine);

        }

        private static void ButtonFirstAction()
        {
            File.AppendAllText(DebugBotPath, "First action on the button" + System.Environment.NewLine);

            while (true)
            {
                if (FileManipulation.Extractions.FileIsReady(BotPath))
                {
                    if (rank < 54)
                    {
                        File.AppendAllText(DebugBotPath, "Changed bot file to 'r'." + System.Environment.NewLine);
                        File.WriteAllText(BotPath, "r");
                        ++RaisesFound;

                        break;
                    }
                    else if (rank < 93)
                    {
                        File.AppendAllText(DebugBotPath, "Changed bot file to 'c'." + System.Environment.NewLine);
                        File.WriteAllText(BotPath, "c");
                        break;
                    }
                    else
                    {
                        File.AppendAllText(DebugBotPath, "Changed bot file to 'f'. This is calling but for testing only, this will be a fold" + System.Environment.NewLine);
                        File.WriteAllText(BotPath, "c");
                        //System.Environment.Exit(0); //for single hand termination
                        HandFinished = true;
                        break;
                    }
                }
                //FirstAction = true;
            }
        }

        private static bool IsHandFinished(string path)
        {
            File.AppendAllText(DebugBotPath, "Checking if hand has finished...." + path.Substring(69) + "\n");
            lines = GetText();
            
            foreach (string line in lines) //try catch this problem here
            {
                if (line.Contains("won") && path.Substring(69).Contains(playAreaLocation))
                {
                    File.AppendAllText(DebugBotPath, "Hand has finished." + System.Environment.NewLine);
                    HandFinished = true;
                    return true;
                }
            }
            HandFinished = false;
            File.AppendAllText(DebugBotPath, "Hand has NOT finished." + System.Environment.NewLine);
            return false;
        }

        private static void BotEventFired() //clean
        {
            string text = "";

            while (true)//check if file is ready to read.
            {
                if (FileManipulation.Extractions.FileIsReady(BotPath))
                {
                    text = System.IO.File.ReadAllText(BotPath); //reading bot file
                    break;
                }
            }
            Hand = GetWholeCards(text);

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

        private static string GetWholeCards(string text)
        {
            if (text.Length > 4)
            {
                Cards[0] = Convert.ToInt32(text.Substring(0, 2)) / 4;
                Cards[1] = Convert.ToInt32(text.Substring(3)) / 4;

                Suits[0] = Convert.ToInt32(text.Substring(0, 2)) % 4;
                Suits[1] = Convert.ToInt32(text.Substring(3)) % 4;
            }
            else if(text.Length == 3)
            {

                Cards[0] = Convert.ToInt32(text.Substring(0, 1)) / 4;
                Cards[1] = Convert.ToInt32(text.Substring(2)) / 4;

                Suits[0] = Convert.ToInt32(text.Substring(0, 1)) % 4;
                Suits[1] = Convert.ToInt32(text.Substring(2)) % 4;
            }
            else if (text[1].Equals(' '))
            {
                Cards[0] = Convert.ToInt32(text.Substring(0, 1)) / 4;
                Cards[1] = Convert.ToInt32(text.Substring(2)) / 4;

                Suits[0] = Convert.ToInt32(text.Substring(0, 1)) % 4;
                Suits[1] = Convert.ToInt32(text.Substring(2)) % 4;
            }
            else
            {
                Cards[0] = Convert.ToInt32(text.Substring(0, 2)) / 4;
                Cards[1] = Convert.ToInt32(text.Substring(3)) / 4;

                Suits[0] = Convert.ToInt32(text.Substring(0, 2)) % 4;
                Suits[1] = Convert.ToInt32(text.Substring(3)) % 4;
            }

            Samus.SetPreFlopCards(Cards, Suits);

            string hand = "";
            
            foreach (var element in Cards) //may not need this
            {
               
                switch (element.ToString())
                {
                    case "0":
                        hand += "2";
                        continue;

                    case "1":
                        hand += "3";
                        continue;

                    case "2":
                        hand += "4";
                        continue;

                    case "3":
                        hand += "5";
                        continue;

                    case "4":
                        hand += "6";
                        continue;

                    case "5":
                        hand += "7";
                        continue;

                    case "6":
                        hand += "8";
                        continue;

                    case "7":
                        hand += "9";
                        continue;

                    case "8":
                        hand += "T"; //TODO: check if its T or 10
                        continue;

                    case "9":
                        hand += "J";
                        continue;

                    case "10":
                        hand += "Q";
                        continue;

                    case "11":
                        hand += "K";
                        continue;

                    case "12":
                        hand += "A";
                        continue;

                    default:
                        throw new ArgumentOutOfRangeException("Card value does not exist. Re-check input.");

                }
            }

            if (Suits[0].Equals(Suits[1]))
            {
                hand += "s";
            }
            else
                hand += "o";

            HandFetched = true;
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
