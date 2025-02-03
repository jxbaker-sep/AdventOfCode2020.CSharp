using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;

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

  [Theory]
  [InlineData("Day17.Sample", 848)]
  [InlineData("Day17", 2292)]
  public void Part2(string file, int expected)
  {
    var grid = Convert(AoCLoader.LoadLines(file)).Select(it => new Point4(it.X, it.Y, 0, 0)).ToHashSet();
    
    foreach(var _ in Enumerable.Repeat(0, 6)) {
      var weights = grid.SelectMany(it => it.Neighbors()).GroupToDictionary(it => it, it => it, it => it.Count);

      HashSet<Point4> next = [];
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
  public IEnumerable<Point3> Neighbors() {
    List<long> zed = [-1,0,1];
    foreach(var dx in zed) foreach(var dy in zed) foreach(var dz in zed) if (dz != 0 || dy != 0 || dx != 0) yield return new Point3(X + dx, Y + dy, Z + dz);
  }
}

public record Point4(long X, long Y, long Z, long W)
{
  public IEnumerable<Point4> Neighbors() {
    List<long> zed = [-1,0,1];
    foreach(var dx in zed) foreach(var dy in zed) foreach(var dz in zed) foreach(var dw in zed) if (dz != 0 || dy != 0 || dx != 0 || dw != 0) yield return new Point4(X + dx, Y + dy, Z + dz, W + dw);
  }
}