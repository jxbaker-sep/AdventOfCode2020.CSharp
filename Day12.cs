using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Microsoft.VisualBasic;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;


namespace AdventOfCode2020.CSharp;

public class Day12
{
  [Theory]
  [InlineData("Day12.Sample", 25)]
  [InlineData("Day12", 441)]
  public void Part1(string file, int expected)
  {
    var instructions = Convert(AoCLoader.LoadLines(file));
    instructions.Aggregate(new PV(Point.Zero, Vector.East), (acc, current) => current.Go(acc))
      .Point.ManhattanDistance(Point.Zero)
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("Day12.Sample", 286)]
  [InlineData("Day12", 40014)]
  public void Part2(string file, int expected)
  {
    var instructions = Convert(AoCLoader.LoadLines(file));
    instructions.Aggregate(new PWaypoint(Point.Zero, Vector.East * 10 + Vector.North), (acc, current) => current.GoWaypoint(acc))
      .Point.ManhattanDistance(Point.Zero)
      .Should().Be(expected);
  }

  public record PV(Point Point, Vector Heading);
  public record PWaypoint(Point Point, Vector WayPoint);
  public record Instruction(char Action, int Order)
  {
    public PV Go(PV current)
    {
      return Action switch
      {
        'N' => current with { Point = current.Point + Vector.North * Order},
        'S' => current with { Point = current.Point + Vector.South * Order},
        'E' => current with { Point = current.Point + Vector.East * Order},
        'W' => current with { Point = current.Point + Vector.West * Order},
        'L' => current with { Heading = Order == 90 ? current.Heading.RotateLeft() :
                                        Order == 180 ? current.Heading.RotateLeft().RotateLeft() :
                                        Order == 270 ? current.Heading.RotateLeft().RotateLeft().RotateLeft() :
                                        throw new ApplicationException()},
        'R' => current with { Heading = Order == 90 ? current.Heading.RotateRight() :
                                        Order == 180 ? current.Heading.RotateRight().RotateRight() :
                                        Order == 270 ? current.Heading.RotateRight().RotateRight().RotateRight() :
                                        throw new ApplicationException()},
        'F' => current with { Point = current.Point + current.Heading * Order},
        _ => throw new ApplicationException()
      };
    }

    public PWaypoint GoWaypoint(PWaypoint current)
    {
      return Action switch
      {
        'N' => current with { WayPoint = current.WayPoint + Vector.North * Order},
        'S' => current with { WayPoint = current.WayPoint + Vector.South * Order},
        'E' => current with { WayPoint = current.WayPoint + Vector.East * Order},
        'W' => current with { WayPoint = current.WayPoint + Vector.West * Order},
        'L' => current with { WayPoint = (Order == 90 ? current.WayPoint.RotateLeft() :
                                        Order == 180 ? current.WayPoint.RotateLeft().RotateLeft() :
                                        Order == 270 ? current.WayPoint.RotateLeft().RotateLeft().RotateLeft() :
                                        throw new ApplicationException())},
        'R' => current with { WayPoint = (Order == 90 ? current.WayPoint.RotateRight() :
                                        Order == 180 ? current.WayPoint.RotateRight().RotateRight() :
                                        Order == 270 ? current.WayPoint.RotateRight().RotateRight().RotateRight() :
                                        throw new ApplicationException())},
        'F' => current with { Point = current.Point + current.WayPoint * Order},
        _ => throw new ApplicationException()
      };
    }
  }

  private static List<Instruction> Convert(List<string> list)
  {
    return P.Format("{}{}", P.Any, P.Int)
      .Select(it => new Instruction(it.First, it.Second))
      .ParseMany(list);
  }
}