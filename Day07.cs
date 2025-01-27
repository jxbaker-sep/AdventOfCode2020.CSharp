
using System.Linq;
using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2020.CSharp;

public class Day07
{
  [Theory]
  [InlineData("Day07.Sample", 4)]
  [InlineData("Day07", 197)]
  public void Part1(string file, int expected)
  {
    var rules = Convert(AoCLoader.LoadLines(file));

    rules.Count(it => CanContain(it.Value, "shiny gold", rules)).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day07.Sample", 32)]
  [InlineData("Day07.Sample.2", 126)]
  [InlineData("Day07", 85324)]
  public void Part2(string file, int expected)
  {
    var data = Convert(AoCLoader.LoadLines(file));

    Count(data["shiny gold"], data).Should().Be(expected);
  }

  public long Count(Bag bag, Dictionary<string, Bag> rules)
  {
    return bag.Contains.Sum(it => it.Count + it.Count * Count(rules[it.Label], rules));
  }

  public bool CanContain(Bag bag, string label, Dictionary<string, Bag> rules)
  {
    return bag.Contains.Any(it => it.Label == label || CanContain(rules[it.Label], label, rules));
  }

  public record Bag(string Label, List<(long Count, string Label)> Contains);

  private static Dictionary<string, Bag> Convert(List<string> list)
  {
    var label = P.Format("{} {}", P.Word, P.Word).Select(it => $"{it.First} {it.Second}");
    var numbered = P.Format("{} {} {}", P.Long, label, P.String("bags") | P.String("bag"))
      .Select(it => (it.First, it.Second));
    var zlist = P.String("no other bags").Select(_ => new List<(long First, string Second)>())
      | numbered.Plus(",");
    var rule = P.Format("{} bags contain {}.", label, zlist).End()
      .Select(it => new Bag(it.First, it.Second));
    return rule.ParseMany(list).ToDictionary(it => it.Label, it => it);
  }
}