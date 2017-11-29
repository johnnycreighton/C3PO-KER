using System;
using System.Linq;
using System.Threading;
using System.Collections;


namespace Samus
{
    class PreFlop
    {
        private static int Dealer;
        private static int BigBlind;
        private static int NoOfRaises;
        private static int Raise;
        private static bool RaiseCalled;
        private static bool HasRaise;
        private static bool Call;
        private static bool AllIn;
        private static bool AllInCall;

        //TODO count raises (done) act accordingly. alwasy 3 bet with a beast. 
        internal static void Play(Player[] players, int posish, Hashtable ranking)
        {
            NoOfRaises = 0;
            HasRaise = false;
            Call = false;
            AllIn = false;
            AllInCall = false;
            RaiseCalled = false;

            posish %= 2;
            posish = 1 - posish; //organise
            Dealer = 1 - posish;
            BigBlind = posish;
            players[Dealer].Button = true;

            players[Dealer].PayPot(Program.SmallBlind); //pay blinds
            players[BigBlind].PayPot(Program.BigBlind);

            while (HasRaise || !Call)
            {
                //Dealer acts first in a heads up game.
                if (AllInCall) return;
                Action(players[Dealer], ranking);
                
                if (HandStrategies.Folds.CheckForFolds(players) || RaiseCalled == true) return;
                //BB action
                if (AllInCall) return;

                Action(players[BigBlind], ranking);
                if (HandStrategies.Folds.CheckForFolds(players)) return;
            }
        }

        

        public static void Action(Player actionplayer, Hashtable ranking)
        {
            //if(actionplayer.FirstCard.ToString().Contains("T") || actionplayer.SecondCard.ToString().Contains("T"))
            //{

            //}
            if (actionplayer.Rank == 0) // check for hand ranking - preflop only
            {
                var hand = actionplayer.FirstCard.ToString().TrimEnd('c', 'd', 's', 'h') + actionplayer.SecondCard.ToString().TrimEnd('c', 'd', 's', 'h');
                int element1 = 1;
                int element2 = 1;

                var suit = "";

               //if (actionplayer.FirstCard.ToString().Contains("10")) //tens have special attributes becaus
               //{
               //    //cardOne = hand.Substring(0, 2);
               //    hand = hand.Replace("10", "T");
               //    element1 = 2;
               //}
               //if (actionplayer.SecondCard.ToString().Contains("10"))
               //{
               //    hand = hand.Replace("10", "T");
               //    element2 = 2;
               //}
                var cardOne = hand.Substring(0, 1);
                var cardTwo = hand.Substring(1, 1);
                if (actionplayer.FirstCard.ToString().ElementAt(element1).Equals(actionplayer.SecondCard.ToString().ElementAt(element2)))
                {
                    suit = "s";
                    hand = hand + "s";
                }
                else
                {
                    hand = hand + "o";
                    suit = "o";
                }

                foreach (DictionaryEntry elem in ranking)
                {
                    if (cardOne == cardTwo) //different algo for pairs
                    {
                        if (elem.Value.ToString().Substring(0, 1).Contains(cardOne) && elem.Value.ToString().Substring(1, 1).Contains(cardTwo) && elem.Value.ToString().Contains(suit))
                        {
                            actionplayer.Rank = Int32.Parse(elem.Key.ToString());
                        }
                    }
                    else
                    {
                        if (elem.Value.ToString().Contains(cardOne) && elem.Value.ToString().Contains(cardTwo) && elem.Value.ToString().Contains(suit))
                        {
                            actionplayer.Rank = Int32.Parse(elem.Key.ToString());
                        }
                    }

                }
            }
            Thread.Sleep(30); //TODO machine is too fast         ----------------------------------- call someones all in
            Random rand = new Random();
            if(actionplayer.Rank < 3)
            {

            }
            if (AllIn)
            {
                if (NoOfRaises <= 2 && actionplayer.Rank < 30 || NoOfRaises > 2 && actionplayer.Rank <= 5)
                {
                    actionplayer.PayPot(Raise);
                    Call = true;
                    AllInCall = true;
                    Program.MainAllIn = true;
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
                    Raise *= rand.Next(2, 3); //pot bed here in stead of raise bet, think
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
            }
            else
            {
                actionplayer.Fold = true;
                Program.MainFold = true;
            }
        }
    }
}
