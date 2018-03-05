using System;

namespace Samus.FileManipulation
{
    public class CardTransform
    {
        private static int[] CardResult = new int[3];
        private static int[] SuitResult = new int[3];

        private static string[] CardResultStr = new string[3];
        private static string[] SuitResultStr = new string[3];

        internal static void Flop(string[] cardNumbers, ref string[] communityCards)
        {
            int i = -1;
            foreach (var number in cardNumbers)
            {
                ++i;
                GetCardNumber(number, i); //gets card number and suits
                GetCardSuit(number, i);
            }
            ExposeCards(-1);

            for (i = 0; i < 3; ++i)
            {
                communityCards[i] = CardResultStr[i] + SuitResultStr[i]; //populates community cards using the ref keyword.
            }
        }
        /*
         *  int i = 0;
            foreach(var number in cardNumbers)
            {
                if (number.Length < 1)
                    continue;
                GetCardNumber(number, i); //gets card number and suits
                GetCardSuit(number, i);
                ++i;
            }

            ExposeCards(-1); //translates numbers into cards + suits

            for (i = 0; i < 3; ++i)
            {
                communityCards[i] = CardResultStr[i] + SuitResultStr[i]; //populates community cards using the ref keyword.
            }
         */

        internal static void Turn(int turnNumber, ref string[] communityCards)
        {
            GetCardNumber(turnNumber.ToString(), 0); //gets card number and suits
            GetCardSuit(turnNumber.ToString(), 0);

            ExposeCards(3); //translates numbers into cards + suits
            
            communityCards[3] = CardResultStr[0] + SuitResultStr[0]; //populates community cards using the ref keyword. // value in CardResultString will get overwritten here but we dont care. 
        }

        internal static void ExposeCards(int i)
        {
            foreach (var element in CardResult)
            {
                ++i;
                switch (element.ToString())
                {
                    case "0":
                        CardResultStr[i] = "2";
                        continue;

                    case "1":
                        CardResultStr[i] = "3";
                        continue;

                    case "2":
                        CardResultStr[i] = "4";
                        continue;

                    case "3":
                        CardResultStr[i] = "5";
                        continue;

                    case "4":
                        CardResultStr[i] = "6";
                        continue;

                    case "5":
                        CardResultStr[i] = "7";
                        continue;

                    case "6":
                        CardResultStr[i] = "8";
                        continue;

                    case "7":
                        CardResultStr[i] = "9";
                        continue;

                    case "8":
                        CardResultStr[i] = "10";
                        continue;

                    case "9":
                        CardResultStr[i] = "J";
                        continue;

                    case "10":
                        CardResultStr[i] = "Q";
                        continue;

                    case "11":
                        CardResultStr[i] = "K";
                        continue;

                    case "12":
                        CardResultStr[i] = "A";
                        continue;

                    default:
                        continue;
                        //throw new ArgumentOutOfRangeException("Card value does not exist. Re-check input.");
                }
            }
            i = -1;
            foreach (var element in SuitResult)
            {
                ++i;
                switch (element.ToString())
                {
                    case "0":
                        SuitResultStr[i] = "S";
                        continue;
                    case "1":
                        SuitResultStr[i] = "C";
                        continue;
                    case "2":
                        SuitResultStr[i] = "H";
                        continue;
                    case "3":
                        SuitResultStr[i] = "D";
                        continue;
                }
            }
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

        internal static void WriteCommunityCards(int card, int i) // extend to flop
        {
            int face = card / 4;
            int suit = card % 4;

            switch (face.ToString())
            {
                case "0":
                    Program.CommunityCards[i] = "2";
                    break;

                case "1":
                    Program.CommunityCards[i] = "3";
                    break;

                case "2":
                    Program.CommunityCards[i] = "4";
                    break;

                case "3":
                    Program.CommunityCards[i] = "5";
                    break;

                case "4":
                    Program.CommunityCards[i] = "6";
                    break;

                case "5":
                    Program.CommunityCards[i] = "7";
                    break;

                case "6":
                    Program.CommunityCards[i] = "8";
                    break;

                case "7":
                    Program.CommunityCards[i] = "9";
                    break;

                case "8":
                    Program.CommunityCards[i] = "10";
                    break;

                case "9":
                    Program.CommunityCards[i] = "J";
                    break;

                case "10":
                    Program.CommunityCards[i] = "Q";
                    break;

                case "11":
                    Program.CommunityCards[i] = "K";
                    break;

                case "12":
                    Program.CommunityCards[i] = "A";
                    break;

                default:
                    throw new ArgumentOutOfRangeException("Card value does not exist. Re-check code + input.");
                    
            }

            
            switch (suit.ToString())
            {
                case "0":
                    Program.CommunityCards[i] += "S";
                    break;
                case "1":
                    Program.CommunityCards[i] += "C";
                    break;
                case "2":
                    Program.CommunityCards[i] += "H";
                    break;
                case "3":
                    Program.CommunityCards[i] += "D";
                    break;
            }
        }

        internal static void GetCardNumber(string number, int position)
        {
                CardResult[position] = Convert.ToInt32(number) / 4; //eg 6 / 4 = 1, 1 = "card of face 3", 
        }
        internal static void GetCardSuit(string number, int position)
        {
             SuitResult[position] = Convert.ToInt32(number) % 4; //eg 10 mod 4 = 2, 2 = "hearts"
        }
    }
}
