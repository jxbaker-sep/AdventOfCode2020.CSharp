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
    var grid = Convert(AoCLoader.LoadLines(file));
    
    grid.Sum(Calculate).Should().Be(expected);
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
    Calculate(Convert1(input)).Should().Be(expected);
  }

  public long Calculate(List<Token> tokens)
  {
    long add(long a, long b) => a + b;
    long mul(long a, long b) => a * b;
    Stack<long> values = [];
    values.Push(0);
    Stack<Func<long,long,long>> pending = [];
    pending.Push(add);
    foreach(var token in tokens)
    {
      if (token.Type == TokenType.LParen)
      {
        values.Push(0);
        pending.Push(add);
      }
      else if (token.Type == TokenType.RParen)
      {
        var b = values.Pop();
        var a = values.Pop();
        var action = pending.Pop();
        values.Push(action(a, b));
      }
      else if (token.Type == TokenType.Add) pending.Push(add);
      else if (token.Type == TokenType.Mul) pending.Push(mul);
      else if (token.Type == TokenType.Number) values.Push(pending.Pop()(values.Pop(), token.Value));
      else throw new ApplicationException();
    }
    return values.Single();
  }

  public enum TokenType { Number, Add, Mul, LParen, RParen };
  public record Token(TokenType Type, long Value);

  private static List<Token> Convert1(string input)
  {    
    var n = P.Long.Select(it => new Token(TokenType.Number, it));
    var a = P.String("+").Select(it => new Token(TokenType.Add, 0));
    var m = P.String("*").Select(it => new Token(TokenType.Mul, 0));
    var l = P.String("(").Select(it => new Token(TokenType.LParen, 0));
    var r = P.String(")").Select(it => new Token(TokenType.RParen, 0));

    return (n|a|m|l|r).Trim().Plus().End().Parse(input);
  }

  private static List<List<Token>> Convert(List<string> input) => input.Select(Convert1).ToList();
}

