
using System.Linq;
using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2020.CSharp;

public class Day06
{
  [Theory]
  [InlineData("Day06.Sample", 11)]
  [InlineData("Day06", 6585)]
  public void Part1(string file, int expected)
  {
    var data = Convert(AoCLoader.LoadFile(file)).ToHashSet();
    
    data.Select(it => it.SelectMany(s => s.ToCharArray()).ToHashSet())
      .Sum(it => it.Count)
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("Day06.Sample", 6)]
  [InlineData("Day06", 3276)]
  public void Part2(string file, int expected)
  {
    var data = Convert(AoCLoader.LoadFile(file)).ToHashSet();
    
    data.Select(it => it.Select(s => s.ToCharArray()).Aggregate((p, n) => p.Intersect(n).ToArray()))
      .Sum(it => it.Length)
      .Should().Be(expected);
  }

  private static List<List<string>> Convert(string list)
  {
    return list.Split("\n\n").Select(it => it.Split("\n").ToList()).ToList();
  }
}