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
                SetUp(players[Dealer], communityCards);

                if (HandStrategies.Folds.CheckForFolds(players) || RaiseCalled == true) return;
                //BB action
                if (AllInCall) return;

                SetUp(players[BigBlind], communityCards);
                if (HandStrategies.Folds.CheckForFolds(players)) return;
            }
        }


        public static void SetUp(Player actionplayer, string[] communityCards)
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

            switch (hand.Hand.ToString()) //work out a way to decide on flop turn and river more fluidly
            {
                case "HighCard":
                    HandStrategies.HighCard.Action(actionplayer, true);
                    break;
                case "OnePair":
                    HandStrategies.OnePair.Action(actionplayer, true);
                    break;
                case "TwoPairs":
                case "ThreeOfAKind":
                case "Straight":
                case "Flush":


                default: //anything else flopped is whopper go nuts //careful of the flop coming trips and a pair in your hand, spells disaster - build a check class

                    break;

            }


            while (HasRaise || !Call) //if there has been a raise and no call as of yet continue betting.
            {

              //  Play(actionplayer);

               
            }
            

            

            //Thread.Sleep(30); //TODO machine is too fast         ----------------------------------- call someones all in

        }

      /*  private static void Play(Player actionplayer)
        {
            if (!HasRaise && actionplayer.check) return; // check and no raise = continue on with game
            else actionplayer.Fold = true; // check and raise = fold


            //Dealer acts first in a heads up game.
            if (AllInCall) return;
            Action(players[Dealer], ranking);

            //BB action
            if (HandStrategies.Folds.CheckForFolds(players) || RaiseCalled == true) return;
            if (AllInCall) return;

            Action(players[BigBlind], ranking);
            if (HandStrategies.Folds.CheckForFolds(players)) return;



        }*/
    }
}
