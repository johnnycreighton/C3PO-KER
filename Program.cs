using System;
using System.Collections.Generic;
using System.Linq;
using static System.String;
using BluffinMuffin.HandEvaluator;
using System.Threading;
using System.Collections;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
//using System.ValueTuple;

namespace Samus
{
    public class Program
    {
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
        public static bool BotFileChanged;

        private static bool FirstAction = true;

        public static bool PlayAreaEvenFileChanged;
        public static bool PlayAreaOddFileChanged;

        public static int Counter = 0;

        public static object[] Cards = new object[2];
        public static object[] Suits = new object[2];
        public static int Position;

        public static int rank;
        private static int handNumber = 1;

        public static int MyWinnings;
        public static int OpponentsWinnings;

        public static FileSystemWatcher BotWatcher = new FileSystemWatcher();
        public static FileSystemWatcher PlayAreaEvenWatcher = new FileSystemWatcher();
        public static FileSystemWatcher PlayAreaOddWatcher = new FileSystemWatcher();



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
        public static String[] CommunityCards = new String[5];
        public static void SetCommunityCards(Deck deck, int numberOfCards)
        {
            short x = 0;
            do
            {
                var index = Array.FindIndex(CommunityCards, i => i == null || i.Length == 0);
                
                CommunityCards[index] = deck.DealCard().ToString();//flop three cards
                x++;
            } while (x < numberOfCards); // minus one for -> starts at zero
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private static void StartBotWatcher(string path)
        {
            BotWatcher.Path = path;
            BotWatcher.NotifyFilter = NotifyFilters.LastWrite;
            BotWatcher.Filter = "bot1.txt";
            
            BotWatcher.Changed += new FileSystemEventHandler(BotFileChange);
            // Begin watching.
            BotWatcher.EnableRaisingEvents = true;
        }

        private static void StartPlayAreaEvenWatcher(string path)
        {
            PlayAreaEvenWatcher.Path = path;
            PlayAreaEvenWatcher.NotifyFilter = NotifyFilters.LastWrite;
            PlayAreaEvenWatcher.Filter = "*Even.txt";

            PlayAreaEvenWatcher.Changed += new FileSystemEventHandler(PlayAreaEvenFileChange);

            // Begin watching.
            PlayAreaEvenWatcher.EnableRaisingEvents = true;
        }
        private static void StartPlayAreaOddWatcher(string path)
        {
            PlayAreaOddWatcher.Path = path;
            PlayAreaOddWatcher.NotifyFilter = NotifyFilters.LastWrite;
            PlayAreaOddWatcher.Filter = "*Odd.txt";

            PlayAreaOddWatcher.Changed += new FileSystemEventHandler(PlayAreaOddFileChange);

            // Begin watching.
            PlayAreaOddWatcher.EnableRaisingEvents = true;
        }
        private static void PlayAreaOddFileChange(object sender, FileSystemEventArgs e)
        {
            PlayAreaOddFileChanged = true;
        }
        private static void PlayAreaEvenFileChange(object sender, FileSystemEventArgs e)
        {
            PlayAreaEvenFileChanged = true;
        }

        private static void BotFileChange(object source, FileSystemEventArgs e)
        {
            BotFileChanged = true;
        }

        private static bool FileIsReady(string sFileName)
        {
            FileStream stream = null;
            FileInfo file = new FileInfo(sFileName);
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return false;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return true;






            //FileStream fs = new FileStream(sFileName, FileMode.Open, FileAccess.Write);
            //if (fs.CanWrite)
            //{
            //    fs.Close();
            //    return true;
            //}








            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            //try
            //{
            //    using (FileStream inputStream = File.Open(sFileName, FileMode.Open, FileAccess.Read, FileShare.None))
            //    {
            //        if (inputStream.Length > 0)
            //        {
            //            return true;
            //        }
            //        else
            //        {
            //            return false;
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
        }
        


        private static void StopWatcher(string path)
        {
            if (path.Contains("Odd"))
            {
                PlayAreaOddWatcher.EnableRaisingEvents = false;
                PlayAreaOddWatcher.Changed -= new FileSystemEventHandler(PlayAreaOddFileChange);
            }
            else if(path.Contains("Even"))
            {
                PlayAreaEvenWatcher.EnableRaisingEvents = false;
                PlayAreaEvenWatcher.Changed -= new FileSystemEventHandler(PlayAreaEvenFileChange);
            }
            else
            {
                BotWatcher.EnableRaisingEvents = false;
                BotWatcher.Changed -= new FileSystemEventHandler(BotFileChange);
            }
            BotFileChanged = false;
        }


        private static void Main()
        {
            StopWatcher(PlayAreaPathOdd);
            
            File.WriteAllText(DebugBotPath, ""); //Emptying Debugger for new game
            File.AppendAllText(DebugBotPath, "Main Method Invoked." + System.Environment.NewLine);
            Samus.Player johnny = new Samus.Player("johnny");
            TwoCardRanking.PopulateHashTable(ranking); //generates ranking of all pre-flop hands based on average money won per hand
            
            StartBotWatcher(BotDirPath); 
            File.AppendAllText(DebugBotPath, "Bot watcher started." + System.Environment.NewLine);
            File.AppendAllText(DebugBotPath, "Starting both play area bot watchers." + System.Environment.NewLine);
            StartPlayAreaEvenWatcher(PlayAreaDirPath); //starting play area watchers to aquire correct position.
            StartPlayAreaOddWatcher(PlayAreaDirPath);
            File.AppendAllText(DebugBotPath, "Started." + System.Environment.NewLine);

            while (true)//first action pre-flop
            {
                if (BotFileChanged)
                {
                    File.AppendAllText(DebugBotPath, "\nHand Number: " + ++Counter + "\nBotFileChanged = true." + System.Environment.NewLine);
                    File.AppendAllText(DebugBotPath, "HandFetched = true. " + System.Environment.NewLine);
                    BotEventFired();

                    switch (GetPosition())
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
                    //if i call on the button and he raises from the big blind

                    BotFileChanged = false;
                    StartBotWatcher(BotDirPath);
                    //Position = 0;// reset for multiple hands at once
                    File.AppendAllText(DebugBotPath, "BotFileChanged = false.\nFirst actions have been performed." + System.Environment.NewLine);
                    break;
                }
            }

            // if(RaiseFound(GetPath())) //TODO when two bots compete, check for a raise to your call.
            // {
            //     
            // }
            
            if(IsHandFinished(Path))
            {
                File.AppendAllText(DebugBotPath, "Hand Finished sort this line out- what to do." + System.Environment.NewLine);
            }




            Flopper.Start(Path, rank, Position, Hand,);



            while(true)
            {
                if (PlayAreaEvenFileChanged && !PlayAreaOddFileChanged)
                {
                    File.AppendAllText(DebugBotPath, "PlayAreaEvenFileChanged = true." + System.Environment.NewLine);

                    PlayAreaEventFired(PlayAreaPathEven);

                    File.AppendAllText(DebugBotPath, "PlayAreaEvenFileChanged = false." + System.Environment.NewLine);
                    PlayAreaEvenFileChanged = false;


                    StartPlayAreaEvenWatcher(PlayAreaDirPath);///////////////////////
                }
                else if (PlayAreaOddFileChanged && !PlayAreaEvenFileChanged)
                {
                    File.AppendAllText(DebugBotPath, "PlayAreaOddFileChanged = true." + System.Environment.NewLine);

                    PlayAreaOddEventFired(PlayAreaPathOdd);

                    File.AppendAllText(DebugBotPath, "PlayAreaOddFileChanged = false." + System.Environment.NewLine);
                    PlayAreaOddFileChanged = false;
                    StartPlayAreaOddWatcher(PlayAreaDirPath);
                }
                if (HandFinished)
                {
                    File.AppendAllText(DebugBotPath, "Hand finished, resetting variables. \n\n\n" + System.Environment.NewLine);
                    StartBotWatcher(BotDirPath);
                    StartPlayAreaEvenWatcher(PlayAreaDirPath);
                    StartPlayAreaOddWatcher(PlayAreaDirPath);
                    HandFinished = false;
                    HandFetched = false;
                    FirstAction = true;
                    Position = 0;
                    rank = 0;
                }
            }
            
        }

        private static int GetPosition()
        {
            if(Position > 0)
            {
                return Position;
            }

            Path = GetPath();

            while (true)//check if file is ready to read.
            {
                File.AppendAllText(DebugBotPath, "Checking if file is ready." + System.Environment.NewLine);
                if (FileIsReady(Path))
                {
                  lines = System.IO.File.ReadAllLines(Path);
                  break;
                }
            }

            foreach (string line in lines) //try catch this
            {
                if (line.Contains("bot1"))
                {
                    char[] bot1 = { 'b', 'o', 't', '1' }; //remove the first int to allow for position int to be found.
                    string newLine = line.TrimStart(bot1);
                    //System.Text.RegularExpressions.Regex
                    //  \d+ is the first integer occurrence in Regex.  
                    Position = Convert.ToInt32(Regex.Match(newLine, @"\d+").Value);
                    File.AppendAllText(DebugBotPath, "Position Found: " + Position + System.Environment.NewLine);
                    break;
                }
            }
            

            return Position;
        }

        private static string GetPath()
        {
            while (true)
            {
                if (PlayAreaOddFileChanged)
                {
                    playAreaLocation = "Odd";
                    return PlayAreaPathOdd;
                }
                else if (PlayAreaEvenFileChanged)
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
            
            StopWatcher(PlayAreaPathOdd);

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
            StopWatcher(path);

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

        private static void BigBlindFirstAction()
        {
            string path = GetPath();
            
            File.AppendAllText(DebugBotPath, "Big blind first action." + System.Environment.NewLine);
            while (true)//check if file is ready to read.
            {
                if (FileIsReady(BotPath))
                {
                    File.AppendAllText(DebugBotPath, "File ready." + System.Environment.NewLine);
                    
                    if (rank < 35) //careful of number of raises -> check 
                    {
                        if(RaisesFound < 4)
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
                    else if(rank > 71 && RaiseFound(path))
                    {
                        File.AppendAllText(DebugBotPath, "Changing bot file to 'f' because rank is bad and raise found." + System.Environment.NewLine);
                        File.WriteAllText(BotPath, "f");
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

        private static bool RaiseFound(string path)
        {
            if(path == null)
            {
                return false;
            }
            while (true)
            {
                if (FileIsReady(path))
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
                    StopWatcher(path); // should be both.
                }
                
            }
            return raise;
        }

        private static void ButtonFirstAction()
        {
            File.AppendAllText(DebugBotPath, "First action on the button" + System.Environment.NewLine);

            string path = GetPath();

            while (true)
            {
                if (FileIsReady(BotPath))
                {
                    if (rank < 35)
                    {
                        File.WriteAllText(BotPath, "r");

                        File.AppendAllText(DebugBotPath, "\n" + File.ReadAllLines(BotPath) +"\n"+ System.Environment.NewLine);
                        File.AppendAllText(DebugBotPath, "Changed bot file to 'r'." + System.Environment.NewLine);
                        ++RaisesFound;

                        break;
                    }
                    else if (rank < 72)
                    {
                        File.AppendAllText(DebugBotPath, "Changed bot file to 'c'." + System.Environment.NewLine);
                        File.WriteAllText(BotPath, "c");
                        break;
                    }
                    else
                    {
                        File.AppendAllText(DebugBotPath, "Changed bot file to 'f'." + System.Environment.NewLine);
                        File.WriteAllText(BotPath, "f");
                        HandFinished = true;
                        break;
                    }
                }
                //FirstAction = true;
            }
        }

        private static bool IsHandFinished(string path)
        {
            File.AppendAllText(DebugBotPath, "Checking if hand has finished...." + path.Substring(67) + "\n");
            if (handNumber == 1)
            {
                ++handNumber;
                HandFinished = false;
                File.AppendAllText(DebugBotPath, "Hand NOT finished...." + path.Substring(67) + "\n");
                return false;
            }
            
            //read appropiate file.
            //check for finished.
            while (true)
            {
                if (FileIsReady(path))
                {
                    lines = System.IO.File.ReadAllLines(path);
                    break;
                }
            }
            
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
            File.AppendAllText(DebugBotPath, "Bot Event has fired so - Bot watcher stopped. " + System.Environment.NewLine);
            StopWatcher(BotPath);
            string text = "";

            while (true)//check if file is ready to read.
            {
                if (FileIsReady(BotPath))
                {
                    text = System.IO.File.ReadAllText(BotPath);
                    break;
                }
            }
            Hand = GetWholeCards(text);

            File.AppendAllText(DebugBotPath, "Bot File ready. Cards = \t" + text + "\nHole cards fetched and converted: \t" + Hand + System.Environment.NewLine);
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
            
            

            string hand = "";
            
            foreach (var element in Cards)
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

            /*
             * suit:
             * 0 = spades
             * 1 = clubs
             * 2 = hearts
             * 3 = diamonds
             * 
             * rank 0 = deuce, 12 = ace
             */

           
        }















































        /*
        var count = 0;
        var johnnyWin = 0;
        var coralieWin = 0;

        while (count < 1000000)
        {

            Deck deck1 = new Deck();
            deck1.Shuffle();


            string p1Cards = "";
            string p2Cards = "";

            for (int i = 0; i < 2; ++i)
            {
                p1Cards += deck1.DealCard().ToString();
                p2Cards += deck1.DealCard().ToString();
            }
            p1Cards = p1Cards.Substring(0, 2) + "," + p1Cards.Substring(2) + ",";

            SetFlopCards(deck1);
            var communeCardsString = string.Join(",", CommunityCards);
            // string[] player1Cards = new { deck.DealCard().ToString()};
            IStringCardsHolder[] players1 =
            {
                new Program.Player("Johnny", deck1.DealCard().ToString(), deck1.DealCard().ToString(), CommunityCards[0], CommunityCards[1], CommunityCards[2], CommunityCards[3], CommunityCards[4]),
                new Player("Coralie", deck1.DealCard().ToString(), deck1.DealCard().ToString(), CommunityCards[0], CommunityCards[1], CommunityCards[2], CommunityCards[3], CommunityCards[4] ),


            };

            // foreach (var p in HandEvaluators.Evaluate(players1))
            // {
            //    // if()
            //     //Console.WriteLine("{0}: {1} -> {2}" + "    " + count, p.Rank == int.MaxValue ? "  " : p.Rank.ToString(), ((Player)p.CardsHolder).Name, p.Evaluation);
            // }

            foreach (var p in HandEvaluators.Evaluate(players1).SelectMany(x => x))
            {
                if (p.Rank == 1)
                {
                    string best = p.Evaluation.ToString();
                    if (best.Contains("Royal") || best.Contains("Straight Flush"))
                    {
                        Console.WriteLine("ROYAL FLUSH  at the " + count + " try.");
                        Console.ReadKey();
                    }
                    if (((Player)p.CardsHolder).Name.Contains("C"))
                    {
                        ++coralieWin;
                    }
                    if (((Player)p.CardsHolder).Name.Contains("J"))
                    {
                        ++johnnyWin;
                    }
                    var winner = HandEvaluators.Evaluate(p.CardsHolder.PlayerCards, p.CardsHolder.CommunityCards);
                    Console.WriteLine(((Player)p.CardsHolder).Name + " is the winner with: " + p.Evaluation + "\t " + count);
                    ++count;
                    Thread.Sleep(30);
                    break;
                }
            }
        }
        Console.ReadKey();*/
    }
 //   }
}
