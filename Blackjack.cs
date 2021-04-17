using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace blackjack
{

    class Blackjack 
    {
        static void Main (string[] args) {
            Console.WriteLine("Welcome to the table, sir!");
            int numOfPlayers = 0;
            while (numOfPlayers < 1 || numOfPlayers > 10) 
            {
                try
                {
                    Console.WriteLine("How many players are in your party today?");
                    numOfPlayers = Int32.Parse(Console.ReadLine());
                    if (numOfPlayers < 1 || numOfPlayers > 10)
                    {
                        Console.WriteLine("I'm not sure we can accommodate that number sir...");
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("I'm sorry sir, I'm not sure I understand.");
                }
            }
            string playerName = "";
            var players = new List<Player>();
            for (var i = 1; i <= numOfPlayers; i++)
            {
                while (players.Count < i)
                {
                    Console.WriteLine($"What is the name of player {i}?");
                    playerName = Console.ReadLine();
                    if (!string.IsNullOrEmpty(playerName))
                    {
                        players.Add(new Player(playerName, 1000));
                        playerName = "";
                    }
                }
            }

            Console.WriteLine("Good luck sir!");
            Thread.Sleep(1000);
            
            var leavers = new List<Player>();
            var dealer = new Player();
            var deck = new Deck();
            int round = 1;

            while (players.Count > 0)
            {
                Console.WriteLine("\n**********");
                Console.WriteLine($"Round {round}");

                // create deck & place bets
                deck.Create();
                deck.Shuffle();
                foreach (var player in players)
                {
                    player.Bet = 0;
                    int minBet = player.Chips >= 50 ? 50 : player.Chips;
                    Console.WriteLine($"{player.Name}'s turn to bet ({player.Chips} | minimum {minBet} | or e(x)it):");
                    string action = Console.ReadLine().ToLower();
                    while (player.Bet == 0)
                    {
                        if (action == "exit" || action == "x")
                        {
                            Console.WriteLine($"{player.Name} left the table.");
                            leavers.Add(player);
                            break;
                        }
                        try
                        {
                            int bet = Int32.Parse(action);
                            if (player.Chips >= bet && bet >= minBet)
                            {
                                player.Chips -= bet;
                                player.Bet = bet;
                            }
                            else
                            {
                                Console.WriteLine("Invalid amount.");
                                action = Console.ReadLine();
                            }
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("I'm sorry sir, I'm not sure I understand.");
                            action = Console.ReadLine();
                        }
                    }
                }
                // to avoid deleting from iterators of a foreach
                foreach (var player in leavers)
                {
                    players.Remove(player);
                }
                numOfPlayers = players.Count();

                // deal cards
                foreach (var player in players) 
                {
                    player.Cards = deck.Deal(2);
                }
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < numOfPlayers; j++)
                    {
                        Console.WriteLine($"{players[j].Name}: {players[j].Cards[i].FullName}");
                        Thread.Sleep(500);
                    }
                }
                dealer.Cards = deck.Deal(2);
                Console.WriteLine($"Dealer shows the {dealer.Cards[0].FullName}.");
                Thread.Sleep(1000);

                // player options
                foreach (var player in players)
                {
                    Console.WriteLine($"\n{player.Name} [{player.Chips}]: {player.Cards[0].FullName} | {player.Cards[1].FullName}");
                    if (player.Total() == 21) {
                        Console.WriteLine("Blackjack!");
                        Thread.Sleep(1000);
                    }
                    else {
                        var validMoves = player.ValidMoves();
                        bool isFinished = false;
                        while (!isFinished)
                        {
                            var firstLetter = validMoves.Select(move => move == "surrender" ? move[1].ToString() : move[0].ToString()).ToList();
                            Console.WriteLine($"What would you like to do? ({String.Join(", ", validMoves)})");
                            string action = Console.ReadLine().ToLower();
                            if (validMoves.Contains(action) || firstLetter.Contains(action))
                            {
                                int index = player.Cards.Count();
                                switch (action)
                                {
                                    case "hit":
                                    case "h":
                                        player.Cards.AddRange(deck.Deal(1));
                                        validMoves.Remove("double down");
                                        validMoves.Remove("split");
                                        validMoves.Remove("surrender");
                                        Console.WriteLine($"You get the {player.Cards[index].FullName}.");
                                        break;

                                    case "stick":
                                    case "s":
                                        isFinished = true;
                                        break;

                                    case "double down":
                                    case "d":
                                        player.Cards.AddRange(deck.Deal(1));
                                        Console.WriteLine($"You get the {player.Cards[index].FullName}.");
                                        player.Chips -= player.Bet;
                                        player.Bet *= 2;
                                        isFinished = true;
                                        break;

                                    default:
                                    Console.WriteLine($"{player.Name} surrenders.");
                                        player.Chips += player.Bet / 2;
                                        player.Cards.Clear();
                                        isFinished = true;
                                        break;
                                }
                                Thread.Sleep(1000);

                                // check bust
                                if (player.Total() > 21)
                                {
                                    if (player.HasAce())
                                    {
                                        player.ReduceAce();
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{player.Name} busts!");
                                        isFinished = true;
                                        Thread.Sleep(1000);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("I'm sorry sir, I'm not sure I understand.");
                            }
                        }
                    }
                }

                // dealer cards
                Console.WriteLine($"\nDealer reveals the {dealer.Cards[1].FullName}.");
                Thread.Sleep(2000);
                while (dealer.Total() < 17)
                {
                    int index = dealer.Cards.Count();
                    dealer.Cards.AddRange(deck.Deal(1));
                    Console.WriteLine($"Dealer gets the {dealer.Cards[index].FullName}.");
                    Thread.Sleep(1000);
                    if (dealer.Total() > 21 && dealer.HasAce())
                    {
                        dealer.ReduceAce();
                    }
                }
                if (dealer.Total() > 21)
                {
                    Console.WriteLine("Dealer busts!");
                    Thread.Sleep(1000);
                }

                // check win
                foreach (var player in players)
                {
                    if (player.HasBlackjack() && dealer.Total() != 21)
                    {
                        int bonus = Convert.ToInt32(player.Bet * 1.5);
                        Console.WriteLine($"{player.Name} wins {bonus}.");
                        player.Chips += player.Bet + bonus;
                    }
                    else if ((dealer.Total() > 21 || player.Total() > dealer.Total()) && player.Total() <= 21)
                    {
                        Console.WriteLine($"{player.Name} wins {player.Bet}.");
                        player.Chips += player.Bet * 2;
                    }
                    else if (player.Total() == dealer.Total() && player.Total() <= 21)
                    {
                        Console.WriteLine($"A push for {player.Name}.");
                        player.Chips += player.Bet;
                    }
                    else {
                        Console.WriteLine($"{player.Name} loses.");
                        if (player.Chips <= 0)
                        {
                            Console.WriteLine($"{player.Name} is broke!");
                            leavers.Add(player);
                        }
                    }
                    Thread.Sleep(1000);
                }

                // losers leave
                foreach (var player in leavers)
                {
                    Console.WriteLine($"{player.Name} left the table.");
                    players.Remove(player);
                }

                round++;
            }

            Console.WriteLine("Game over, thank you for playing!");
        }
    }
}