using System.Net.Sockets;
using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
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
    var world = Convert(AoCLoader.LoadFile(file), false);

    world.Inputs.Count(it => world.Parser.TryParse(it, out var _))
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("Day19.Sample.2", 12)]
  [InlineData("Day19", 228)]
  public void Part2(string file, int expected)
  {
    var world = Convert(AoCLoader.LoadFile(file), true);

    world.Inputs.Count(it => world.Parser.TryParse(it, out var _))
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("aab")] // 8 11
  [InlineData("aaab")] // 8 8 11
  [InlineData("aaabb")] // 8 11 11
  public void Sanity2(string input)
  {
    var parser = CreateParser(@"
0: 8 11
42: ""a""
31: ""b""
8: 42 | 42 8
11: 42 31 | 42 11 31".Split("\n").Skip(1).ToList(), true);

    parser.TryParse(input, out var _).Should().BeTrue();
  }

  public record World(Parser<string> Parser, List<string> Inputs);

  private static World Convert(string input, bool rewrite)
  {
    var pps = input.Split("\n\n").Select(it => it.Split("\n").ToList()).ToList();

    return new(CreateParser(pps[0], rewrite), pps[1]);
  }

  private static Parser<string> CreateParser(List<string> list, bool rewrite)
  {
    List<DeferredParser<string>> ps = Enumerable.Range(0, Math.Max(list.Count, 43)).Select(_ => new DeferredParser<string>()).ToList();

    var plainp = P.Format("{}: {}", P.Int, P.Int.Trim().Plus()).End();
    var literalp = P.Format("{}: \"{}\"", P.Int, P.Any).End();
    var alternatep = P.Format("{}: {} | {}", P.Int, P.Int.Trim().Plus(), P.Int.Trim().Plus()).End();

    foreach(var item in list)
    {
      if (plainp.TryParse(item, out var plain))
      {
        ps[plain.First].Actual = plain.Second[1..].Aggregate(ps[plain.Second[0]] as Parser<string>, (prev, current) => prev + ps[current]);
      }
      else if (literalp.TryParse(item, out var literal))
      {
        ps[literal.First].Actual = P.String($"{literal.Second}");
      }
      else if (alternatep.TryParse(item, out var alternate))
      {
        var second = alternate.Second[1..].Aggregate(ps[alternate.Second[0]] as Parser<string>, (prev, current) => prev + ps[current]);
        var third = alternate.Third[1..].Aggregate(ps[alternate.Third[0]] as Parser<string>, (prev, current) => prev + ps[current]);
        ps[alternate.First].Actual = (second | third);
      }
      else throw new ApplicationException();
    }

    if (rewrite)
    {
      ps[0].Actual = ps[42].PlusUntil(ps[11]).Select(it => it.Accumulator.Join() + it.Sentinel);
      ps[11].Actual = ps[42].PlusUntil(ps[11] + ps[31]).Select(it => it.Accumulator.Join() + it.Sentinel) | (ps[42] + ps[31]);
    }

    return ps[0].End();
  }
}