using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Microsoft.VisualBasic;
using Parser;
using Utils;

namespace AdventOfCode2020.CSharp;

public class Day11
{
  [Theory]
  [InlineData("Day11.Sample", 37)]
  [InlineData("Day11", 2166)]
  public void Part1(string file, int expected)
  {
    var grid = Convert(AoCLoader.LoadLines(file));

    DoIt(grid, point => point.CompassRoseNeighbors());

    grid.Count(kv => kv.Value == '#').Should().Be(expected);
  }

  [Theory]
  [InlineData("Day11.Sample", 26)]
  [InlineData("Day11", 0)]
  public void Part2(string file, int expected)
  {
    var grid = Convert(AoCLoader.LoadLines(file));

    IEnumerable<Point> Open(Point point)
    {
      foreach(var vector in Vector.CompassRose)
      {
        var z = point.Follow(vector).TakeWhile(p2 => grid.ContainsKey(p2)).Where(it => grid[it] != '.').Take(1).ToList();
        if (z.Count > 0) yield return z[0];
      }
    }

    DoIt(grid, Open);

    grid.Count(kv => kv.Value == '#').Should().Be(expected);
  }

  private void DoIt(Dictionary<Point, char> grid, Func<Point, IEnumerable<Point>> open)
  {
    HashSet<string> seen = [];
    string Key() => grid.OrderBy(kv=>kv.Key.Y).ThenBy(kv=>kv.Key.X).Select(kv=>kv.Value).Join();

    while (!seen.Contains(Key()))
    {
      seen.Add(Key());
      Dictionary<Point, int> counts = grid
        .SelectMany(kv => open(kv.Key))
        .Where(neighbor => grid.GetValueOrDefault(neighbor) == '#')
        .GroupToDictionary(it => it, it => it, it => it.Count);

      foreach(var (point, value) in grid.ToList())
      {
        if (value == '.') continue;
        var count = counts.GetValueOrDefault(point);
        if (value == 'L' && count == 0) grid[point] = '#';
        if (value == '#' && count >= 4) grid[point] = 'L';
      }
    }
  }

  private static Dictionary<Point, char> Convert(List<string> list)
  {
    return list.Gridify();
  }
}