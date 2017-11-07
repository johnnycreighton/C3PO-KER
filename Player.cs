using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samus
{
    class Player
    {
        public static long Stack = 10000;
        public string Name;
        public Card FirstCard;
        public Card SecondCard;
        public Card[] WholeCards;
        public bool button = false;
        


        public Player(String name)
        {
            this.Name = name;
            WholeCards = new Card[2];
            
        }
        public void SetFirstCard(Card card)
        {
            this.WholeCards[0] = card;
        }
        public void SetSecondCard(Card card)
        {
            this.WholeCards[1] = card;
        }

        internal static void SetWholeCards(Player[] players, Deck deck)
        {
            players[0].FirstCard = deck.DealCard();
            players[1].FirstCard = deck.DealCard();
            players[0].SecondCard = deck.DealCard();
            players[1].SecondCard = deck.DealCard();
        }
    }
}
