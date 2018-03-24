namespace Samus
{
    /// <summary>
    /// Card object needed for player strategy
    /// </summary>
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
