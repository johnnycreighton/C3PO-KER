using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samus
{
    public class Player
    {
        public int Stack = 10000;
        public int Rank;
       
        public double Tolerance;

        public string Name;
        public string Hand;

        public Card FirstCard;
        public Card SecondCard;

        public bool AllIn;
        public bool Button;
        public bool Fold;
        public bool FlushDraw;
        public bool BackDoorFlushDraw;
        public bool StraightDraw;
        public bool BackDoorStraightDraw;
        public bool OpenEndedStraightDraw;
        public bool GutShotStraightDraw;



        /*
         * options for speed
         * add card ranks reduces the need to check for straights/flushes 
         * 
         */


        public Player(String name)
        {
            this.Name = name; 
        }
        public void PayPot(int amount)
        {
            this.Stack = this.Stack - amount;
            Program.Pot += amount;
        }
        //public void SetFirstCard(Card card)
        //{
        //    this.WholeCards[0] = card;
        //}
        //public void SetSecondCard(Card card)
        //{
        //    this.WholeCards[1] = card;
        //}

        internal static void SetWholeCards(Player[] players, Deck deck)
        {
            players[0].FirstCard = deck.DealCard();
            players[1].FirstCard = deck.DealCard();
            players[0].SecondCard = deck.DealCard();
            players[1].SecondCard = deck.DealCard();
        }
    }
}
