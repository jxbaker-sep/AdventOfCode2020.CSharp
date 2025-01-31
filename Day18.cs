using System.Numerics;
using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Microsoft.VisualBasic;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2020.CSharp;

public class Day18
{
  [Theory]
  [InlineData("Day18", 7293529867931)]
  public void Part1(string file, long expected)
  {
    Convert(AoCLoader.LoadLines(file))
      .Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("1", 1)]
  [InlineData("1 + 1", 2)]
  [InlineData("1 + 2 * 3 + 4 * 5 + 6", 71)]
  [InlineData("1 + (2 * 3) + (4 * (5 + 6))", 51)]
  [InlineData("2 * 3 + (4 * 5)", 26)]
  [InlineData("5 + (8 * 3 + 9 + 3 * 4 * 3)", 437)]
  [InlineData("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", 12240)]
  [InlineData("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 13632)]
  public void Sanity(string input, long expected)
  {
    Convert1(input).Should().Be(expected);
  }

  private static long Convert1(string input)
  {    
    DeferredParser<long> lhsp = new();
    var lhs_head = P.Long.Trim() | lhsp.Between("(", ")");
    var lhs_tail = P.Sequence(P.Choice("+", "*").Trim(), lhs_head).Star();
    lhsp.Actual = P.Sequence(lhs_head, lhs_tail)
      .Select(it => {
        return it.Second.Aggregate(it.First, (prev, current) => current.First switch {
          "+" => prev + current.Second,
          "*" => prev * current.Second,
          _ => throw new ApplicationException()
        });
      });
    return lhsp.Parse(input);
  }

  private static List<long> Convert(List<string> input) => input.Select(Convert1).ToList();
}

