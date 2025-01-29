using System.Numerics;
using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Microsoft.VisualBasic;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2020.CSharp;

public class Day15
{
  [Theory]
  [InlineData("Day15", 929)]
  public void Part1(string file, long expected)
  {
    var program = Convert(AoCLoader.LoadFile(file));
    Do1(program, 2020).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day15", 16671510L)]
  public void Part2(string file, long expected)
  {
    var program = Convert(AoCLoader.LoadFile(file));
    Do1(program, 30000000).Should().Be(expected);
  }

  [Theory]
  [InlineData("0,3,6", 4, 0)]
  [InlineData("0,3,6", 5, 3)]
  [InlineData("0,3,6", 2020, 436)]
  [InlineData("1,3,2", 2020, 1)]
  [InlineData("1,2,3", 2020, 27)]
  [InlineData("2,3,1", 2020, 78)]
  [InlineData("3,2,1", 2020, 438)]
  [InlineData("3,1,2", 2020, 1836)]
  public void Sanity(string input, int turn, long expected)
  {
    Do1(Convert(input), turn).Should().Be(expected);
  }

  long Do1(List<long> l, int turns)
  {
    var seen = l[..^1].WithIndices().ToDictionary(it => it.Value, it => it.Index + 1);
    var spoken = l[^1];
    for (int i = l.Count; i < turns ; i++)
    {
      if (seen.TryGetValue(spoken, out var last))
      {
        seen[spoken] = i;
        spoken = i - last;
      }
      else
      {
        seen[spoken] = i;
        spoken = 0;
      }
    }

    return spoken;
  }

  private static List<long> Convert(string input)
  {
    return P.Long.Star(",").End().Parse(input);
  }
}