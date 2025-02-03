
using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;

namespace AdventOfCode2020.CSharp;

public class Day03
{
  [Theory]
  [InlineData("Day03.Sample", 7)]
  [InlineData("Day03", 162)]
  public void Part1(string file, int expected)
  {
    var data = Convert(AoCLoader.LoadLines(file)).ToHashSet();
    
    DoSlope(new(1,3), data).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day03.Sample", 336)]
  [InlineData("Day03", 3064612320)]
  public void Part2(string file, long expected)
  {
    var data = Convert(AoCLoader.LoadLines(file)).ToHashSet();
    
    (DoSlope(new(1,1), data)
     * DoSlope(new(1,3), data)
     * DoSlope(new(1,5), data)
     * DoSlope(new(1,7), data)
     * DoSlope(new(2,1), data)
    ).Should().Be(expected);
  }

  long DoSlope(Vector v, HashSet<Point> data)
  {
    var maxY = data.MaxBy(it => it.Y)!.Y;
    var maxX = data.MaxBy(it => it.X)!.X;
    var current = Point.Zero;
    long result = 0;
    while (current.Y <= maxY)
    {
      current = new Point(current.Y + v.Y, (current.X + v.X) % (maxX + 1));
      if (data.Contains(current)) result += 1;
    }
    return result;
  }

  private static HashSet<Point> Convert(List<string> list)
  {
    return list.Gridify().Where(kv=>kv.Value == '#').Select(it => it.Key).ToHashSet();
  }
}