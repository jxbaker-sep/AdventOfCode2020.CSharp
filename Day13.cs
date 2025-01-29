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

  [Theory]
  [InlineData("Day13.Sample", 1068781)]
  [InlineData("Day13", 600689120448303)]
  public void Part2(string file, int expected)
  {
    var (timestamp, schedule) = Convert(AoCLoader.LoadLines(file));

    Sieve(schedule.Select((id, index) => (id, index))
      .Where(it => it.id != -1)
      .Select(it => (remainder: MathMod(it.id - it.index, it.id), modulus: it.id))
      .ToList())
    .Should().Be(expected);
  }

  private static long MathMod(long a, long b)
  {
    return ((a % b) + b) % b;
  }

  static long Sieve(List<(long remainder, long modulus)> r) // 
  {
    r = r.OrderByDescending(it => it.modulus).ToList();
    long increment = r[0].modulus;
    var current = r[0].remainder;
    var n = 1;
    var mod = r[1].modulus;
    var remainder = r[1].remainder;
    while (n < r.Count)
    {
      if (current % mod == remainder)
      {
        increment *= mod;
        n += 1;
        if (n >= r.Count) return current;
        mod = r[n].modulus;
        remainder = r[n].remainder;
        continue;
      }
      current += increment;
    }
    throw new ApplicationException();
  }


  private static (long, List<long>) Convert(List<string> list)
  {
    var timestamp = System.Convert.ToInt64(list[0]);
    return (timestamp, (P.Long | P.String("x").Select(_ => -1L)).Star(",").Parse(list[1]));
  }
}