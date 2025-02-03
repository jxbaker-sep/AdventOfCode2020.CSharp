using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2020.CSharp;

public class Day05
{
  [Theory]
  [InlineData("Day05", 913)]
  public void Part1(string file, int expected)
  {
    var ids = Convert(AoCLoader.LoadLines(file)).ToHashSet();
    
    ids.Max().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day05", 717)]
  public void Part2(string file, int expected)
  {
    var ids = Convert(AoCLoader.LoadLines(file)).ToHashSet();
    
    var max = ids.Max();
    var min = ids.Max();

    var placementSort = Enumerable.Repeat(-1, max + 1).ToArray();

    foreach(var item in ids) placementSort[item] = item;

    Array.LastIndexOf(placementSort, -1)
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("FBFBBFFRLR", 357)]
  [InlineData("BFFFBBFRRR", 567)]
  [InlineData("FFFBBBFRRR", 119)]
  [InlineData("BBFFBBFRLL", 820)]
  public void Sanity1(string input, int expected)
  {
    Convert([input])[0].Should().Be(expected);
  }

  private static int SeatId(List<int> data)
  {
    return data.Aggregate(0, (p, n) => (p << 1) + n);
  }

  private static List<int> Convert(List<string> list)
  {
    var f = P.String("F").Select(it => 0);
    var b = P.String("B").Select(it => 1);
    var l = P.String("L").Select(it => 0);
    var r = P.String("R").Select(it => 1);
    return (f|b|l|r).Range(10,10).Select(it => SeatId(it)).ParseMany(list);
  }
}