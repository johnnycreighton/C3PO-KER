﻿using System;
using System.Collections.Generic;
using System.Linq;
using static System.String;
using BluffinMuffin.HandEvaluator;
using System.Threading;

namespace Samus
{
    public class Program
    {
        public class Player : IStringCardsHolder
        {
            public string Name { get; }
            private string[] Cards { get; }

            public Player(string name, params string[] cards)
            {
                Name = name;
                Cards = cards;
            }

            public override string ToString()
            {
                return $"{Name}: [{Join(",", Cards)}]";
            }

            public IEnumerable<string> PlayerCards => Cards.Take(2);

            public IEnumerable<string> CommunityCards => Cards.Skip(2);
        }
        public static String[] CommunityCards = new String[5];
        public static void SetCommunityCards(Deck deck)
        {
            var x = 0;
            do
            {
                CommunityCards[x] = deck.DealCard().ToString();
                x++;
            } while (x < 5);
        }

        private int stack = 10000;
        private static void Main()
        {
            Deck deck = new Deck();
            deck.Shuffle();
            var amountOfHands = 0;
            var biggest = HandEvaluators.Evaluate(new[] { "Ks", "Ac" }, new[] { "8d", "3c", "7s", "10h", "2s" });
          
            
          
            Samus.Player[] players =
            {
                new Samus.Player("johnny"),
                new Samus.Player("Coralie")
            };
            
            Samus.Player.SetWholeCards(players, deck);
            
                      
            PreFlop.Play(players, amountOfHands);































            var count = 0;
            var johnnyWin = 0;
            var coralieWin = 0;

            while (count < 1000000)
            {

                Deck deck1 = new Deck();
                deck1.Shuffle();
                             

                string p1Cards = "";
                string p2Cards = "";

                for (int i = 0; i < 2; ++i)
                {
                    p1Cards += deck1.DealCard().ToString();
                    p2Cards += deck1.DealCard().ToString();
                }
                p1Cards = p1Cards.Substring(0, 2) + "," + p1Cards.Substring(2)+ ",";

                SetCommunityCards(deck1);
                var communeCardsString = string.Join(",", CommunityCards);
                // string[] player1Cards = new { deck.DealCard().ToString()};
                IStringCardsHolder[] players1 =
                {
                    new Player("Johnny", deck1.DealCard().ToString(), deck1.DealCard().ToString(), CommunityCards[0], CommunityCards[1], CommunityCards[2], CommunityCards[3], CommunityCards[4]),
                    new Player("Coralie", deck1.DealCard().ToString(), deck1.DealCard().ToString(), CommunityCards[0], CommunityCards[1], CommunityCards[2], CommunityCards[3], CommunityCards[4] ),
                   

                };
                
                // foreach (var p in HandEvaluators.Evaluate(players1))
                // {
                //    // if()
                //     //Console.WriteLine("{0}: {1} -> {2}" + "    " + count, p.Rank == int.MaxValue ? "  " : p.Rank.ToString(), ((Player)p.CardsHolder).Name, p.Evaluation);
                // }

                foreach (var p in HandEvaluators.Evaluate(players1).SelectMany(x => x))
                {
                    if (p.Rank == 1)
                    {
                        string best = p.Evaluation.ToString();
                        if (best.Contains("Royal") || best.Contains("Straight Flush"))
                        {
                            Console.WriteLine("ROYAL FLUSH  at the " + count + " try.");
                            Console.ReadKey();
                        }
                        if (((Player)p.CardsHolder).Name.Contains("C"))
                        {
                            ++coralieWin;
                        }
                        if (((Player)p.CardsHolder).Name.Contains("J"))
                        {
                            ++johnnyWin;
                        }
                        var winner = HandEvaluators.Evaluate(p.CardsHolder.PlayerCards, p.CardsHolder.CommunityCards);
                        Console.WriteLine(((Player)p.CardsHolder).Name + " is the winner with: " + p.Evaluation + "\t " + count);
                        ++count;
                        Thread.Sleep(30);
                        break;
                    }
                }                              
            }
            Console.ReadKey();
        }
    }
}