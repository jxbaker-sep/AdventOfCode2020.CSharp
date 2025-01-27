
using System.Linq;
using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2020.CSharp;

public class Day08
{
  [Theory]
  [InlineData("Day08.Sample", 5)]
  [InlineData("Day08", 2080)]
  public void Part1(string file, int expected)
  {
    var instructions = Convert(AoCLoader.LoadLines(file));
    Run(instructions).Should().Be((expected, true));
  }

  [Theory]
  [InlineData("Day08.Sample", 8)]
  [InlineData("Day08", 2477)]
  public void Part2(string file, int expected)
  {
    var instructions = Convert(AoCLoader.LoadLines(file));

    foreach(var (i, index) in instructions.Select((it, index) => (it,index)))
    {
      if (i.Label == "acc") continue;
      var copy = instructions.ToList();
      copy[index] = i with {Label = i.Label == "nop" ? "jmp" : "nop"};
      var (a,b) = Run(copy);
      if (!b)
      {
        a.Should().Be(expected);
        return;
      }
    }
    throw new ApplicationException();
  }

  public static (long, bool) Run(List<Instruction> instructions)
  {
    HashSet<long> closed = [];
    long accumulator = 0;
    long pc = 0;

    while (!closed.Contains(pc))
    {
      if (pc == instructions.Count) return (accumulator, false);
      closed.Add(pc);
      switch (instructions[(int)pc].Label)
      {
        case "acc":
          accumulator += instructions[(int)pc].Value;
          pc += 1;
          break;
        case "nop":
          pc += 1;
          break;
        case "jmp":
          pc += instructions[(int)pc].Value;
          break;
      }
    }
    return (accumulator, true);
  }

  public record Instruction(string Label, long Value);

  private static List<Instruction> Convert(List<string> list)
  {
    return P.Format("{} {}{}", P.Word, P.String("+").Optional(), P.Long)
      .Select(it => new Instruction(it.First, it.Third))
      .End()
      .ParseMany(list);
  }
}