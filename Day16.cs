using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2020.CSharp;

public class Day16
{
  [Theory]
  [InlineData("Day16.Sample", 71)]
  [InlineData("Day16", 21956)]
  public void Part1(string file, long expected)
  {
    var world = Convert(AoCLoader.LoadFile(file));
    
    var max = world.Rules.SelectMany(it => it.Valid())
      .Concat(world.YourTicket.Fields)
      .Concat(world.NearbyTickets.SelectMany(it => it.Fields))
      .Max();

    var array = Enumerable.Repeat(0, max + 1).ToArray();
    foreach(var item in world.Rules.SelectMany(it => it.Valid())) array[item] = 1;

    world.NearbyTickets.SelectMany(it => it.Fields.Select(f => (long)f))
      .Where(it => array[it] == 0)
      .Sum()
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("Day16", 3709435214239)]
  public void Part2(string file, long expected)
  {
    var world = Convert(AoCLoader.LoadFile(file));
    
    var order = DoPart2(world);
    var x = world.YourTicket.Fields.WithIndices()
      .Where(it => order[it.Index] < 6)
      .Select(it => (long)it.Value)
      .ToList();
    x.Product().Should().Be(expected);
  }

  [Fact]
  public void Part2_Sanity()
  {
    var world = Convert(@"class: 0-1 or 4-19
row: 0-5 or 8-19
seat: 0-13 or 16-19

your ticket:
11,12,13

nearby tickets:
3,9,18
15,1,5
5,14,9");

    DoPart2(world).Join().Should().Be("102");
  }

  static List<int> DoPart2(World world)
  {
    var max = world.Rules.SelectMany(it => it.Valid())
      .Concat(world.YourTicket.Fields)
      .Concat(world.NearbyTickets.SelectMany(it => it.Fields))
      .Max();

    var valueToRules = Enumerable.Range(0, max + 1).Select(_ => new List<int>()).ToList();
    foreach(var item in world.Rules.WithIndices()
      .SelectMany(it => it.Value.Valid().Select(v => (Value: v, it.Index)))) valueToRules[item.Value].Add(item.Index);

    var tickets = world.NearbyTickets.Where(ticket => ticket.Fields.All(field => valueToRules[field].Count > 0)).ToList();

    var fieldToRules = Enumerable.Range(0, tickets[0].Fields.Count).Select(_ => Enumerable.Range(0, world.Rules.Count).ToList()).ToList();

    foreach(var ticket in tickets)
    {
      foreach(var (value, index) in ticket.Fields.WithIndices())
      {
        fieldToRules[index] = fieldToRules[index].Intersect(valueToRules[value]).ToList();
      }
    }

    while (fieldToRules.Any(poss => poss.Count > 1))
    {
      var ones = fieldToRules.Where(poss => poss.Count == 1).SelectMany(it => it).ToList();
      foreach(var (value, index) in fieldToRules.WithIndices().Where(poss => poss.Value.Count > 1).ToList())
      {
        fieldToRules[index] = fieldToRules[index].Except(ones).ToList();
      }
    }

    return fieldToRules.SelectMany(it => it).ToList();
  }

  public record Rule(string Label, int a, int b, int c, int d)
  {
    public IEnumerable<int> Valid()
    {
      for (var x = a; x <= b; x++) yield return x;
      for (var x = c; x <= d; x++) yield return x;
    }
  }
  public record Ticket(List<int> Fields);

  public record World(List<Rule> Rules, Ticket YourTicket, List<Ticket> NearbyTickets);

  private static World Convert(string input)
  {
    var pps = input.Split("\n\n").Select(it => it.Split("\n")).ToList();
    var words = P.Word.Plus(P.String(" ")).Select(it => it.Join(" "));
    var rulep = P.Format("{}: {}-{} or {}-{}", words, P.Int, P.Int, P.Int, P.Int).Select(it => new Rule(it.First.Join(" "), it.Second, it.Third, it.Fourth, it.Fifth)).End();

    var ticketp = P.Int.Star(",").End().Select(it => new Ticket(it));

    return new(rulep.ParseMany(pps[0]), ticketp.Parse(pps[1][1]), ticketp.ParseMany(pps[2][1..]));
  }
}