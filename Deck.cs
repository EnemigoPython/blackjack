using System;
using System.Collections.Generic;

namespace blackjack
{

    public class Deck
    {
        public List<Card> Cards = new List<Card>();
        string[] Suits = {"Hearts", "Diamonds", "Spades", "Clubs"};
        string[] FaceCards = {"Jack", "Queen", "King"};

        public Deck() {
            Create();
        }

        public void Create()
        {
            Cards.Clear();
            foreach (var Suit in Suits) 
            {
                Cards.Add(new Card("Ace", Suit, 11));
                for (int i = 2; i < 11; i++) 
                {
                    Cards.Add(new Card(i.ToString(), Suit, i));
                }
                foreach (var FaceCard in FaceCards) 
                {
                    Cards.Add(new Card(FaceCard, Suit, 10));
                }
            }
        }

        public void Shuffle() 
        {
            var newOrder = new List<Card>();
            while (Cards.Count > 0) 
            {
                int index = new Random().Next(Cards.Count);
                newOrder.Add(Cards[index]);
                Cards.RemoveAt(index);
            }
            Cards = newOrder;
        }

        public List<Card> Deal(int num)
        {
            var cards = Cards.GetRange(0, num);
            Cards.RemoveRange(0, num);
            return cards;
        }
    }
}