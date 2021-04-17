namespace blackjack
{
    public class Card
        {
            public string FullName;
            public int Value;
            public Card(string name, string suit, int value)
            {
                Value = value;
                FullName = name + " of " + suit;
            }
        }
}