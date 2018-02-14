using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samus.FileManipulation
{
    public class CardTransform
    {
        private static int[] CardResult = new int[3];
        private static int[] SuitResult = new int[3];

        internal static string Flop(string[] cardNumbers)
        {
            int i = 0;
            foreach(var number in cardNumbers)
            {
                if (number.Length < 1)
                    continue;
                GetCardNumber(number, i);
                GetCardSuit(number, i);
                ++i;
            }
            return ExposeCards(CardResult, SuitResult);
        }

        private static string ExposeCards(int[] cardResult, int[] suitResult)
        {
            string result = null;
            foreach (var element in cardResult)
            {
                switch (element.ToString())
                {
                    case "0":
                        result += "2";
                        continue;

                    case "1":
                        result += "3";
                        continue;

                    case "2":
                        result += "4";
                        continue;

                    case "3":
                        result += "5";
                        continue;

                    case "4":
                        result += "6";
                        continue;

                    case "5":
                        result += "7";
                        continue;

                    case "6":
                        result += "8";
                        continue;

                    case "7":
                        result += "9";
                        continue;

                    case "8":
                        result += "T"; //TODO: check if its T or 10
                        continue;

                    case "9":
                        result += "J";
                        continue;

                    case "10":
                        result += "Q";
                        continue;

                    case "11":
                        result += "K";
                        continue;

                    case "12":
                        result += "A";
                        continue;

                    default:
                        throw new ArgumentOutOfRangeException("Card value does not exist. Re-check input.");
                }
            }

            foreach (var element in suitResult)
            {
                switch (element.ToString())
                {
                    case "0":
                        result += "s";
                        continue;
                    case "1":
                        result += "c";
                        continue;
                    case "2":
                        result += "h";
                        continue;
                    case "3":
                        result += "d";
                        continue;
                }
            }
            return result;
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

        internal static void GetCardNumber(string number, int position)
        {
                CardResult[position] = Convert.ToInt32(number) / 4;
        }
        internal static void GetCardSuit(string number, int position)
        {
             SuitResult[position] = Convert.ToInt32(number) % 4;
        }
    }
}
