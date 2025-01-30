using System.Numerics;
using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Microsoft.VisualBasic;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2020.CSharp;

public class Day17
{
  [Theory]
  [InlineData("Day17.Sample", 112)]
  [InlineData("Day17", 242)]
  public void Part1(string file, int expected)
  {
    var grid = Convert(AoCLoader.LoadLines(file));
    
    foreach(var _ in Enumerable.Repeat(0, 6)) {
      var weights = grid.SelectMany(it => it.Neighbors()).GroupToDictionary(it => it, it => it, it => it.Count);

      HashSet<Point3> next = [];
      foreach(var cube in weights.Keys.Union(grid)) {
        var weight = weights.GetValueOrDefault(cube);
        if (grid.Contains(cube)) {
          if (weight == 2 || weight == 3) next.Add(cube);
        }
        else if (weight == 3) next.Add(cube);
      }
      grid = next;
    }

    grid.Count.Should().Be(expected);
  }

  private static HashSet<Point3> Convert(List<string> input) => input.Gridify()
    .Where(it => it.Value == '#')
    .Select(it => it.Key)
    .Select(it => new Point3(it.X, it.Y, 0))
    .ToHashSet();
}

public record Point3(long X, long Y, long Z)
{
  public long ManhattanDistance(Point3 other) => Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);

  public IEnumerable<Point3> Neighbors() {
    List<long> zed = [-1,0,1];
    foreach(var dx in zed) foreach(var dy in zed) foreach(var dz in zed) if (dz != 0 || dy != 0 || dx != 0) yield return new Point3(X + dx, Y + dy, Z + dz);
  }
}