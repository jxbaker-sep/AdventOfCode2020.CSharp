using System.Threading.Tasks.Dataflow;
using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;


namespace AdventOfCode2020.CSharp;

public class Day24
{
  [Theory]
  [InlineData("Day24.Sample", 10, 2208)]
  [InlineData("Day24", 312, 0)]
  public void Part1(string file, int expected1, int expected2)
  {
    var instructions = Convert(AoCLoader.LoadLines(file));

    var hexToFlips = instructions.Select(it => it.Aggregate(Hex.Zero, (prev, next) => prev + next))
      .GroupToDictionary(it => it, it => it, it => it.Count);

    var blackHexes = hexToFlips.Where(it => it.Value % 2 == 1).Select(it => it.Key).ToHashSet();

    blackHexes.Count.Should().Be(expected1);

    foreach(var _ in Enumerable.Range(0, 100))
    {
      var counts = blackHexes.SelectMany(it => Open(it)).GroupToDictionary(it => it, it => it, it => it.Count);
      HashSet<Hex> next = [];
      foreach(var hex in blackHexes) 
      {
        var count = counts.GetValueOrDefault(hex);
        if (count == 1) next.Add(hex);
      }
      foreach(var hex in counts.Keys.Except(blackHexes))
      {
        var count = counts.GetValueOrDefault(hex);
        if (count == 2) next.Add(hex);
      }
      blackHexes = next;
    }
    blackHexes.Count.Should().Be(expected2);
  }

  static IEnumerable<Hex> Open(Hex me) {
    yield return me + HexVector.North;
    yield return me + HexVector.NorthWest;
    yield return me + HexVector.NorthEast;
    yield return me + HexVector.South;
    yield return me + HexVector.SouthEast;
    yield return me + HexVector.SouthWest;
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
