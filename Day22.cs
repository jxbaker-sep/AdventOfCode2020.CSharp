using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2020.CSharp;

public class Day22
{
  [Theory]
  [InlineData("Day22.Sample", 306)]
  [InlineData("Day22", 29764)]
  public void Part1(string file, long expected)
  {
    var zed = Convert(AoCLoader.LoadFile(file));
    LinkedList<long> player1 = new(zed.Item1);
    LinkedList<long> player2 = new(zed.Item2);

    while (player1.Count > 0 && player2.Count > 0)
    {
      var item1 = player1.First!.Value;
      var item2 = player2.First!.Value;
      player1.RemoveFirst();
      player2.RemoveFirst();
      if (item1 > item2) { player1.AddLast(item1); player1.AddLast(item2); }
      else { player2.AddLast(item2); player2.AddLast(item1); }
    }

    var winner = player1.Count > 0 ? player1 : player2;
    winner.WithIndices().Select(it => it.Value * ( winner.Count - it.Index)).Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day22.Sample", 291)]
  [InlineData("Day22", 32588)]
  public void Part2(string file, long expected)
  {
    var data = Convert(AoCLoader.LoadFile(file));

    var winner = RecursiveCombat(data.Item1, data.Item2);

    winner.Deck.WithIndices().Select(it => it.Value * ( winner.Deck.Count - it.Index)).Sum().Should().Be(expected);
  }


  public enum Winner { Player1, Player2 };
  int Game = 0;

  public (Winner Winner, List<long> Deck) RecursiveCombat(IEnumerable<long> player1Original, IEnumerable<long> player2Original)
  {
    HashSet<string> Cache = [];
    var mygame = ++Game;
    var round = 0;
    
    LinkedList<long> player1 = new(player1Original);
    LinkedList<long> player2 = new(player2Original);
    while (player1.Count > 0 && player2.Count > 0)
    {
      var key = player1.Join(",") + ";" + player2.Join(",");
      if (!Cache.Add(key)) {
        return (Winner.Player1, player1Original.ToList());
      }
      round += 1;
      // Console.WriteLine($"==== Round {round} (Game {mygame})");
      // Console.WriteLine(player1.Join(","));
      // Console.WriteLine(player2.Join(","));
      var item1 = player1.First!.Value;
      var item2 = player2.First!.Value;
      player1.RemoveFirst();
      player2.RemoveFirst();
      if (player1.Count >= item1 && player2.Count >= item2)
      {
        var winner = RecursiveCombat(player1.Take((int)item1), player2.Take((int)item2));
        if (winner.Winner == Winner.Player1) { player1.AddLast(item1); player1.AddLast(item2); }
        else { player2.AddLast(item2); player2.AddLast(item1); }
      }
      else
      {
        if (item1 > item2) { player1.AddLast(item1); player1.AddLast(item2); }
        else { player2.AddLast(item2); player2.AddLast(item1); }
      }
    }
    return player1.Count > 0 ? (Winner.Player1, player1.ToList()) : (Winner.Player2, player2.ToList());
  }

  private static (List<long>, List<long>) Convert(string input)
  {
    return P.Format("Player 1: {} Player 2: {}", P.Long.Trim().Star(), P.Long.Trim().Star())
      .Parse(input);
  }
}