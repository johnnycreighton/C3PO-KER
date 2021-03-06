﻿using System;

namespace Samus
{
    public class Player
    {
        public int Rank;
        public string Name;
        public string Hand;

        public Card FirstCard;
        public Card SecondCard;

        
        public bool FlushDraw;
        public bool BackDoorFlushDraw;
        public bool BackDoorStraightDraw;
        public bool OpenEndedStraightDraw;
        public bool GutShotStraightDraw;
        
        /// <summary>
        /// Sorting cards into readable faces and suits for pre-flop evaluation
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="suits"></param>
        public void SetPreFlopCards(object[] cards, object[] suits)
        {
            string cardOne = null;
            string cardTwo = null;
            string cardSuitOne = null;
            string cardSuitTwo = null;

            switch (cards[0].ToString()) //switch statement for efficiency
            {
                case "0":
                    cardOne = "2";
                    break;

                case "1":
                    cardOne  = "3";
                    break;

                case "2":
                    cardOne = "4";
                    break;

                case "3":
                    cardOne = "5";
                    break;

                case "4":
                    cardOne = "6";
                    break;

                case "5":
                    cardOne = "7";
                    break;

                case "6":
                    cardOne = "8";
                    break;

                case "7":
                    cardOne = "9";
                    break;

                case "8":
                    cardOne = "10"; 
                    break;

                case "9":
                    cardOne = "J";
                    break;

                case "10":
                    cardOne = "Q";
                    break;

                case "11":
                    cardOne = "K";
                    break;

                case "12":
                    cardOne = "A";
                    break;

                default:
                    throw new ArgumentOutOfRangeException("Card value does not exist. Re-check input.");

            }

            switch (cards[1].ToString())
            {
                case "0":
                    cardTwo = "2";
                    break;

                case "1":
                    cardTwo = "3";
                    break;

                case "2":
                    cardTwo = "4";
                    break;

                case "3":
                    cardTwo = "5";
                    break;

                case "4":
                    cardTwo = "6";
                    break;

                case "5":
                    cardTwo = "7";
                    break;

                case "6":
                    cardTwo = "8";
                    break;

                case "7":
                    cardTwo = "9";
                    break;

                case "8":
                    cardTwo = "10"; 
                    break;

                case "9":
                    cardTwo = "J";
                    break;

                case "10":
                    cardTwo = "Q";
                    break;

                case "11":
                    cardTwo = "K";
                    break;

                case "12":
                    cardTwo = "A";
                    break;

                default:
                    throw new ArgumentOutOfRangeException("Card value does not exist. Re-check input.");

            }

            switch (suits[0].ToString())
            {
                case "0":
                    cardSuitOne = "h";
                    break;
                case "1":
                    cardSuitOne = "c";
                    break;
                case "2":
                    cardSuitOne = "s";
                    break;
                case "3":
                    cardSuitOne = "d";
                    break;
            }

            switch (suits[1].ToString())
            {
                case "0":
                    cardSuitTwo = "h";
                    break;
                case "1":
                    cardSuitTwo = "c";
                    break;
                case "2":
                    cardSuitTwo = "s";
                    break;
                case "3":
                    cardSuitTwo = "d";
                    break;
            }

            FirstCard = new Card(cardOne, cardSuitOne);
            SecondCard = new Card(cardTwo, cardSuitTwo);
        }

        public Player(String name)
        {
            Name = name; 
        }
    }
}
