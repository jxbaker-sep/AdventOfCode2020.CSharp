using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Microsoft.VisualBasic;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;


namespace AdventOfCode2020.CSharp;

public class Day13
{
  [Theory]
  [InlineData("Day13.Sample", 295)]
  [InlineData("Day13", 3385)]
  public void Part1(string file, int expected)
  {
    var (timestamp, schedule) = Convert(AoCLoader.LoadLines(file));
    
    var needle = schedule.Where(x => x > -1)
      .MinBy(x => x * (1 + timestamp / x));
    var min = needle * (1 + timestamp / needle);
    ((min - timestamp) * needle).Should().Be(expected);
  }


  private static (long, List<long>) Convert(List<string> list)
  {
    var timestamp = System.Convert.ToInt64(list[0]);
    return (timestamp, (P.Long | P.String("x").Select(_ => -1L)).Star(",").Parse(list[1]));
  }
}