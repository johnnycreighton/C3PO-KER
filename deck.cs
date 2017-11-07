using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samus
{
    public class Deck
    {
        private Card[] deck;
        private int currentCard;
        private const int numOfCards = 52;
        private Random rand;

        public Deck()
        {
            string[] faces = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
            string[] suit = { "h", "c", "d", "s" };
            deck = new Card[numOfCards];
            currentCard = 0;
            rand = new Random();
            for (int counter = 0; counter < deck.Length; ++counter)
            {
                deck[counter] = new Card(faces[counter % 13], suit[counter / 13]);
            }
        }

        public Card DealCard()
        {
            if (currentCard < deck.Length)
                return deck[currentCard++];
            else
                return null;
        }
        public void Shuffle()
        {
            currentCard = 0;
            for (int first = 0; first < deck.Length; ++first)
            {
                int second = rand.Next(numOfCards);
                Card temp = deck[first];
                deck[first] = deck[second];
                deck[second] = temp;
            }
        }
    }
}
