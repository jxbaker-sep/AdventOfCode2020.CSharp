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
  [InlineData("Day18", 60807587180737)]
  public void Part2(string file, long expected)
  {
    AoCLoader.LoadLines(file).Select(Convert2)
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

  [Theory]
  [InlineData("1", 1)]
  [InlineData("1 + 1", 2)]
  [InlineData("1 + 2 * 3 + 4 * 5 + 6", 231)]
  [InlineData("1 + (2 * 3) + (4 * (5 + 6))", 51)]
  [InlineData("2 * 3 + (4 * 5)", 46)]
  [InlineData("5 + (8 * 3 + 9 + 3 * 4 * 3)", 1445)]
  [InlineData("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", 669060)]
  [InlineData("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 23340)]
  public void Sanity2(string input, long expected)
  {
    Convert2(input).Should().Be(expected);
  }

  private static long Convert1(string input)
  {    
    DeferredParser<long> expressionp = new();
    var head = P.Long.Trim() | expressionp.Between("(", ")");
    var tail = P.Sequence(P.Choice("+", "*").Trim(), head).Star();
    expressionp.Actual = P.Sequence(head, tail)
      .Select(it => {
        return it.Second.Aggregate(it.First, (prev, current) => current.First switch {
          "+" => prev + current.Second,
          "*" => prev * current.Second,
          _ => throw new ApplicationException()
        });
      });
    return expressionp.End().Parse(input);
  }

  private static long Convert2(string input)
  {    
    DeferredParser<long> expressionp = new();
    var head = P.Long.Trim() | expressionp.Between("(", ")");
    var tail = (P.Sequence(P.String("+").Trim(), head)
      |P.Sequence(P.String("*").Trim(), expressionp)).Star();
    expressionp.Actual = P.Sequence(head, tail)
      .Select(it => it.Second.Aggregate(it.First, (prev, current) => current.First switch {
          "+" => prev + current.Second,
          "*" => prev * current.Second,
          _ => throw new ApplicationException()
        })
      );
    return expressionp.End().Parse(input);
  }

  private static List<long> Convert(List<string> input) => input.Select(Convert1).ToList();
}

