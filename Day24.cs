using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;


namespace AdventOfCode2020.CSharp;

public class Day24
{
  [Theory]
  [InlineData("Day24.Sample", 10)]
  [InlineData("Day24", 312)]
  public void Part1(string file, int expected)
  {
    var instructions = Convert(AoCLoader.LoadLines(file));

    var hexToFlips = instructions.Select(it => it.Aggregate(Hex.Zero, (prev, next) => prev + next))
      .GroupToDictionary(it => it, it => it, it => it.Count);

    hexToFlips.Count(it => it.Value % 2 == 1).Should().Be(expected);
  }

  private static List<List<HexVector>> Convert(List<string> input)
  {
    // directions are rotated wrt vectors available in HexVector
    var e = P.String("e").Select(it => HexVector.North);
    var se = P.String("se").Select(it => HexVector.NorthEast);
    var sw = P.String("sw").Select(it => HexVector.SouthEast);
    var w = P.String("w").Select(it => HexVector.South);
    var nw = P.String("nw").Select(it => HexVector.SouthWest);
    var ne = P.String("ne").Select(it => HexVector.NorthWest);
    return (se|ne|sw|nw|e|w).Plus().ParseMany(input);
  }

}
