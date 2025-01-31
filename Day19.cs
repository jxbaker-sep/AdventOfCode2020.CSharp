using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2020.CSharp;

public class Day19
{
  [Theory]
  [InlineData("Day19.Sample", 2)]
  [InlineData("Day19.Sample.2", 3)]
  [InlineData("Day19", 124)]
  public void Part1(string file, int expected)
  {
    var world = Convert(AoCLoader.LoadFile(file));

    world.Inputs.Count(it => world.Parser.TryParse(it, out var _))
      .Should().Be(expected);
  }

  public record World(Parser<string> Parser, List<string> Inputs);

  private static World Convert(string input)
  {
    var pps = input.Split("\n\n").Select(it => it.Split("\n").ToList()).ToList();

    return new(CreateParser(pps[0]), pps[1]);
  }

  private static Parser<string> CreateParser(List<string> list)
  {
    List<DeferredParser<string>> ps = Enumerable.Range(0, Math.Max(list.Count, 43)).Select(_ => new DeferredParser<string>()).ToList();

    var plainp = P.Format("{}: {}", P.Int, P.Int.Trim().Plus()).End();
    var literalp = P.Format("{}: \"{}\"", P.Int, P.Any).End();
    var alternatep = P.Format("{}: {} | {}", P.Int, P.Int.Trim().Plus(), P.Int.Trim().Plus()).End();

    foreach(var item in list)
    {
      if (plainp.TryParse(item, out var plain))
      {
        ps[plain.First].Actual = plain.Second[1..].Aggregate(ps[plain.Second[0]].Select(it=>it), (prev, current) => prev + ps[current]);
      }
      else if (literalp.TryParse(item, out var literal))
      {
        ps[literal.First].Actual = P.String($"{literal.Second}");
      }
      else if (alternatep.TryParse(item, out var alternate))
      {
        var second = alternate.Second[1..].Aggregate(ps[alternate.Second[0]].Select(it=>it), (prev, current) => prev + ps[current]);
        var third = alternate.Third[1..].Aggregate(ps[alternate.Third[0]].Select(it=>it), (prev, current) => prev + ps[current]);
        ps[alternate.First].Actual = second | third;
      }
      else throw new ApplicationException();
    }

    return ps[0].End();
  }
}

