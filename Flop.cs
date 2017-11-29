using BluffinMuffin.HandEvaluator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samus
{
    class Flop
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

    internal static void Play(Player[] players, string[] communityCards, int posish)
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

            while (HasRaise || !Call)
            {
                if (AllInCall) return;
                Action(players[Dealer], communityCards);

                if (HandStrategies.Folds.CheckForFolds(players) || RaiseCalled == true) return;
                //BB action
                if (AllInCall) return;

                Action(players[BigBlind], communityCards);
                if (HandStrategies.Folds.CheckForFolds(players)) return;
            }
        }
              

        public static void Action(Player actionplayer, string[] communityCards)
        {
            IStringCardsHolder[] players =
                {
                    new Program.Player(actionplayer.Name, actionplayer.FirstCard.ToString(),actionplayer.SecondCard.ToString(), communityCards[0], communityCards[1], communityCards[2])
                };

            HandEvaluationResult hand = null;
            foreach (var p in HandEvaluators.Evaluate(players).SelectMany(x => x))
            {
                hand = HandEvaluators.Evaluate(p.CardsHolder.PlayerCards, p.CardsHolder.CommunityCards);
            }
            actionplayer.Hand = hand.Hand.ToString();

            HandStrategies.Draws.CheckForDraws(actionplayer, communityCards);

            switch (hand.Hand.ToString())
            {
                case "HighCard":
                    HandStrategies.HighCard.Action(actionplayer, true);
                    break;
                case "OnePair":
                case "TwoPairs":
                case "ThreeOfAKind":
                case "Straight":
                case "Flush":
                   
                
                default: //any else flopped is whopper go nuts //careful off the flop coming trips and a pair in your hand, spells disaster

                    break;

                    /*
                     * HighCard = 0;
                     * OnePair = 1;
                     *  = 2;
                     *  = 3;
                     *  = 4;
                     *  = 5;
                     * FullHouse = 6;
                     * FourOfAKind = 7;
                     * StraightFlush = 8;
                    
                     */
            }


            //Thread.Sleep(30); //TODO machine is too fast         ----------------------------------- call someones all in
            Random rand = new Random();









            if (actionplayer.Rank < 3)
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
                else if (actionplayer.Rank < 42 && NoOfRaises < 3)
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
