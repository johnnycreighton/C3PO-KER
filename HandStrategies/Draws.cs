using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samus.HandStrategies
{
    class Draws
    {
        internal static void CheckForDraws(Samus.Player actionplayer, string[] communityCards)
        {
            int i = 0;
            int[] myCards = new int[2];
            bool[] straightChecker = new bool[15]; //use a boolean array

            string mySuits = "";
            string boardSuits = "";
            if (actionplayer.FirstCard.ToString().Contains("10") && actionplayer.SecondCard.ToString().Contains("10"))
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
                if (actionplayer.FirstCard.ToString().Contains("10") || actionplayer.SecondCard.ToString().Contains("10"))
                {
                    straightChecker[10] = true;
                    continue;
                }
                if (i == 0)
                {
                    face = actionplayer.FirstCard.ToString().Substring(0, 1);
                }
                else
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


            for (i = 0; i < communityCards.Length; ++i) //board cards inserted into straightChecker array    TODO: possibly extract this method and make it callable to use on board and whole cards
            {
                if (communityCards[i] == null)
                    break;
                var face = ""; // redo varaiable
                if (communityCards[i].Contains("10"))
                {
                    straightChecker[10] = true;
                    continue;
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

            var occ = (1, '.');
            var tempOcc = (1, '.');
            i = 0;
            foreach (char c in mySuits) //do two because i only want my cards checked against the board
            {
                tempOcc.Item1 = 1;
                for (i = 0; i < boardSuits.Length; ++i)
                {

                    if (boardSuits[i].Equals(c))
                    {
                        tempOcc.Item1++;
                        tempOcc.Item2 = boardSuits[i];
                    }
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
            for (i = 1; i <= straightChecker.Length - 5; ++i) // checking in fives, so minus 5 for speed
            {
                count = 0;
                if (straightChecker[i] == true) //run inside loop if element found
                {
                    for (int j = i + 1; j < i + 5; ++j) //bundles of five from i && i + 1 because i know the first one is true 
                    {

                        if (straightChecker[j] == true && straightChecker[j + 1] == true && straightChecker[j + 1] == true && straightChecker[j + 1] == true)
                        {
                            actionplayer.OpenEndedStraightDraw = true;
                            break;
                        }
                        if (straightChecker[j] == true)
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
        }


    }
}
