using System.Collections.Generic;
using System.Linq;

namespace blackjack {
    public class Player {
        public string Name;
        public int Chips;
        public int Bet;
        public List<Card> Cards;
        public Player() {}
        public Player(string name, int chips) 
        {
            Name = name;
            Chips = chips;
        }

        public int Total() 
        {
            return Cards.Count > 0 ? Cards.Aggregate(0, (sum, card) => sum + card.Value) : 0;
        }

        public bool HasAce()
        {
            return Cards.Any(card => card.Value == 11);
        }

        public bool HasBlackjack()
        {
            return Cards.Count == 2 && Total() == 21;
        }

        public void ReduceAce()
        {
            var Ace = Cards.Where(card => card.Value == 11).ToArray();
            Ace[0].Value = 1;
        }

        public List<string> ValidMoves()
        {
            var validMoves = new List<string> {"stick", "hit", "surrender"};
            if (Chips >= Bet)
            {
                validMoves.Add("double down");
            }
            return validMoves;
        }
    }
}