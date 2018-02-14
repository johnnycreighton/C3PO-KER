namespace Samus
{
    public class Card
    {
        public string face;
        public string suit;

        public Card(string cardFace, string cardSuit)
        {
            face = cardFace;
            suit = cardSuit;
        }
        public override string ToString()
        {
            return face + "" + suit;
        }
    }
}
