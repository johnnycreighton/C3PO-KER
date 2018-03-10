using System;
using System.Linq;
using System.Threading;
using System.Collections;

namespace Samus
{
   /*class PreFlop
    {
        private static int Dealer;
        private static int BigBlind;
        public static int NoOfRaises;
        private static int Raise;
        public static bool RaiseCalled;
        public static bool HasRaise;
        public static bool Call;
        public static bool AllIn;
        public static bool AllInCall;

        /// <summary>
        /// Pre-flop Logic
        /// </summary>
        /// <param name="players"></param>
        /// <param name="posish"></param>
        /// <param name="ranking"></param>
        internal static void Play(Player[] players, int posish, Hashtable ranking)
        {

            Resets.ReuseableMethods.ResetFlags();
            AssignButtonAndBlinds(players, posish);

            while (HasRaise || !Call) //if there has been a raise and no call as of yet continue betting.
            {
                //Dealer acts first in a heads up game.
                if (AllInCall) return;
                Action(players[Dealer], ranking);

                //BB action
                if (HandStrategies.Folds.CheckForFolds(players) || RaiseCalled == true) return;
                if (AllInCall) return;

                Action(players[BigBlind], ranking);
                if (HandStrategies.Folds.CheckForFolds(players)) return;
            }
        }

        /// <summary>
        /// Assign blinds and button to adhere to universal rules.
        /// </summary>
        /// <param name="players"></param>
        /// <param name="posish"></param>
        private static void AssignButtonAndBlinds(Player[] players, int posish)
        {
            posish %= 2;
            posish = 1 - posish; 
            Dealer = 1 - posish;
            BigBlind = posish;
            players[Dealer].Button = true;

            players[Dealer].PayPot(Program.SmallBlind);
            players[BigBlind].PayPot(Program.BigBlind);
        }


        

        public static void Action(Player actionplayer, Hashtable ranking)
        {
            //actionplayer.FirstCard.face = "A";
            //actionplayer.SecondCard.face = "A";  //Test purposes
            //
            //actionplayer.FirstCard.suit = "d";
            //actionplayer.SecondCard.suit = "h";

            //if(actionplayer.FirstCard.ToString().Contains("T") || actionplayer.SecondCard.ToString().Contains("T"))
            //{

            //}
            if (actionplayer.Rank == 0) // check for hand ranking - preflop only
            {
                //var hand = actionplayer.FirstCard.ToString().TrimEnd('c', 'd', 's', 'h') + actionplayer.SecondCard.ToString().TrimEnd('c', 'd', 's', 'h');

                //To check suits
                int element1 = 1; 
                int element2 = 1;
                

                var suit = "";
                var cardOne = actionplayer.FirstCard.ToString().TrimEnd('c', 'd', 's', 'h');
                var cardTwo = actionplayer.SecondCard.ToString().TrimEnd('c', 'd', 's', 'h');

               if (actionplayer.FirstCard.ToString().Contains("10")) //tens have special attributes because they are more than one character length
               {
                    cardOne = "T";
                    element1 = 2;
               }
               if (actionplayer.SecondCard.ToString().Contains("10"))
               {
                    cardTwo = "T";
                    element2 = 2;
               }
                
                if (actionplayer.FirstCard.ToString().ElementAt(element1).Equals(actionplayer.SecondCard.ToString().ElementAt(element2))) 
                {
                    suit = "s";
                    //hand = hand + "s";
                }
                else
                {
                   // hand = hand + "o";
                    suit = "o";
                }

                foreach (DictionaryEntry elem in ranking)
                {
                    if (cardOne == cardTwo) //different algo for pairs
                    {
                        if (elem.Value.ToString().Substring(0, 1).Contains(cardOne) && elem.Value.ToString().Substring(1, 1).Contains(cardTwo) && elem.Value.ToString().Contains(suit))
                        {
                            actionplayer.Rank = Int32.Parse(elem.Key.ToString());
                            break;
                        }
                    }
                    else
                    {
                        if (elem.Value.ToString().Contains(cardOne) && elem.Value.ToString().Contains(cardTwo) && elem.Value.ToString().Contains(suit))
                        {
                            actionplayer.Rank = Int32.Parse(elem.Key.ToString());
                            break;
                        }
                    }

                }
            }
            Thread.Sleep(30); //TODO machine is too fast         ----------------------------------- call someones all in
            Random rand = new Random();
            
            if (AllIn)
            {
                if (NoOfRaises <= 2 && actionplayer.Rank < 30 || NoOfRaises > 2 && actionplayer.Rank <= 5)
                {
                    actionplayer.PayPot(Raise);
                    Call = true;
                    AllInCall = true;
                    //Program.MainAllIn = true; dont need this its already set 
                    return;
                }
                else 
                {
                    actionplayer.Fold = true;
                    Program.MainFold = true;
                }
            }
            if (HasRaise) //strategy is based on amount of raises and ranking of hand.       
            {
                if (actionplayer.Rank <= 2 && NoOfRaises > 3)
                {
                    Raise = actionplayer.Stack;
                    actionplayer.PayPot(Raise);
                    AllIn = true;
                    Program.MainAllIn = true;
                    return;
                }
                else if (actionplayer.Rank < 6 && NoOfRaises <= 3)
                {
                    Raise *= rand.Next(2, 5); //pot bet here instead of a raise bet, (think more on this)
                    actionplayer.PayPot(Raise);
                    HasRaise = true;
                    NoOfRaises++;
                    return;
                }
                else if(actionplayer.Rank < 42 && NoOfRaises < 3)
                {
                    actionplayer.PayPot(Raise);
                    HasRaise = false;
                    RaiseCalled = true;
                    return;
                }
                
                else { actionplayer.Fold = true; return; }
            }
           
            else if (actionplayer.Rank > 35 && Call && actionplayer.Button == false) //for a BB check
            {
                //Call = false;
                return;
            }
            if (actionplayer.Rank <= 34)
            {
                Raise = Program.BigBlind * rand.Next(2, 5);
                actionplayer.PayPot(Raise);
                HasRaise = true;
                NoOfRaises++;
            }
            else if (actionplayer.Rank < 72)
            {
                actionplayer.PayPot(Program.SmallBlind);
                Call = true;
                return;
            }
            else
            {
                actionplayer.Fold = true;
                Program.MainFold = true;
            }
        }
    }*/
}
