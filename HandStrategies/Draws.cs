using System;
using System.Linq;

namespace Samus.HandStrategies
{
    public class Draws
    {
        /// <summary>
        /// Method which will check for every possible draw and set a boolean value for each draw found into the player object
        /// </summary>
        /// <param name="actionplayer"></param>
        /// <param name="communityCards"></param>
        public static void CheckForDraws(Player actionplayer, string[] communityCards)
        {
            int i = 0;
            int[] myCards = new int[2];
            bool[] straightChecker = new bool[15]; //using a boolean array to check for straight draws

            string mySuits = "";
            string boardSuits = "";
            if (actionplayer.FirstCard.ToString().Contains("10") && actionplayer.SecondCard.ToString().Contains("10")) //checking for 10 madness
            {
                mySuits = actionplayer.FirstCard.ToString().ElementAt(2).ToString() + actionplayer.SecondCard.ToString().ElementAt(2).ToString();
                straightChecker[10] = true;
                straightChecker[10] = true;
            }
            else if (actionplayer.FirstCard.ToString().Contains("10"))
            {
                mySuits = actionplayer.FirstCard.ToString().ElementAt(2).ToString() + actionplayer.SecondCard.ToString().ElementAt(1).ToString();
                straightChecker[10] = true;
            }
            else if (actionplayer.SecondCard.ToString().Contains("10"))
            {
                mySuits = actionplayer.FirstCard.ToString().ElementAt(1).ToString() + actionplayer.SecondCard.ToString().ElementAt(2).ToString();
                straightChecker[10] = true;
            }
            else
            {
                mySuits = actionplayer.FirstCard.ToString().ElementAt(1).ToString() + actionplayer.SecondCard.ToString().ElementAt(1);
            }

            for (i = 0; i < 5; ++i)
            {
                if (communityCards[i] == null)
                    break;
                if (communityCards[i].Contains("10"))
                    boardSuits += communityCards[i].Substring(2, 1).ToString();
                else
                    boardSuits += communityCards[i].ElementAt(1).ToString();
            }


            for (i = 0; i < 2; ++i) // my cards, insert into straightChecker array
            {
                var face = "";
                if (actionplayer.FirstCard.ToString().Contains("10") || actionplayer.SecondCard.ToString().Contains("10")) //tens cause mayhem for parsing, so a check is done here
                {
                    straightChecker[10] = true;
                    continue;
                }
                if (i == 0)//to get the first card
                {
                    face = actionplayer.FirstCard.ToString().Substring(0, 1);
                }
                else //will grab the second card
                    face = actionplayer.SecondCard.ToString().Substring(0, 1);


                switch (face)
                {
                    case "J":
                        straightChecker[11] = true;
                        continue;

                    case "Q":
                        straightChecker[12] = true;
                        continue;

                    case "K":
                        straightChecker[13] = true;
                        continue;

                    case "A":
                        straightChecker[14] = true;
                        straightChecker[1] = true; // added in to check for the wheel straight
                        continue;

                    default:
                        straightChecker[int.Parse(face)] = true;// only when switch falls all the way through, eg card is a 3 ,insert 3 into element 3 in array
                        continue;
                }
            }


            for (i = 0; i < communityCards.Length; ++i) //board cards inserted into straightChecker array
            {
                if (communityCards[i] == null)
                    break;
                var face = ""; // redo varaiable
                if (communityCards[i].Contains("10")) //10 check here
                {
                    straightChecker[10] = true;
                    continue; //continue with loop
                }

                face = communityCards[i].Substring(0, 1);
                switch (face)
                {
                    case "J":
                        straightChecker[11] = true;
                        continue;

                    case "Q":
                        straightChecker[12] = true;
                        continue;

                    case "K":
                        straightChecker[13] = true;
                        continue;

                    case "A":
                        straightChecker[14] = true;
                        straightChecker[1] = true; // added in to check for the wheel straight
                        continue;

                    default:
                        straightChecker[int.Parse(communityCards[i].Substring(0, 1))] = true;
                        continue;
                }
            }

            ValueTuple<int, char> occ = new ValueTuple<int, char>(1, '.');
            ValueTuple<int, char> tempOcc = new ValueTuple<int, char>(1, '.');
            i = 0;
            foreach (char c in mySuits) //do two because i only want my cards checked against the board
            {
                tempOcc.Item1 = 1;
                for (i = 0; i < boardSuits.Length; ++i)
                {
                    if ((boardSuits[i].Equals((char.ToUpper(c)))))
                    {
                        ++tempOcc.Item1;
                        tempOcc.Item2 = boardSuits[i];
                    }
                }
                if(mySuits[0] == mySuits[1])
                {
                    ++tempOcc.Item1;
                }
                if (tempOcc.Item1 > occ.Item1)
                {
                    occ = tempOcc;
                }
            }

            if (occ.Item1 == 3)
            {
                actionplayer.BackDoorFlushDraw = true;
            }
            else if (occ.Item1 > 3)
            {
                actionplayer.FlushDraw = true;
            }

            short count = 0; //straight draw checker.

            for (i = 1; i <= straightChecker.Length; ++i) // checking in fives, so minus 5 for speed, start at 1 because 0 is not a card value
            {
                count = 0;
                if (i < 12 && straightChecker[i] == true ) //run inside loop if element found
                {
                    if (straightChecker[i + 1] == true && straightChecker[i + 2] == true && straightChecker[i + 3] == true)
                    {
                        actionplayer.OpenEndedStraightDraw = true;
                        goto here; //jump out of outer loop because open ended is found and the rest is not used in strategy from this point
                    }
                    for (int j = i + 1; j <= i + 4; ++j) //bundles of five from i && i + 1 because i know the first one is true 
                    {
                        if(j == 15)
                        {
                            goto here;
                        }
                        
                        if (j != 15 && straightChecker[j] == true)
                        {
                            count++;
                        }
                    }
                    if (count == 3)
                    {
                        actionplayer.GutShotStraightDraw = true;
                        break;
                    }
                    else if (count == 2)
                    {
                        actionplayer.BackDoorStraightDraw = true;
                    }
                }
            }
            here:;
        }
    }
}
