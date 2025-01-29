using System.Numerics;
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
  public void Part2ViaSieve(string file, long expected)
  {
    var (timestamp, schedule) = Convert(AoCLoader.LoadLines(file));

    Sieve(schedule.Select((id, index) => (id, index))
      .Where(it => it.id != -1)
      .Select(it => (remainder: MathMod(it.id - it.index, it.id), modulus: it.id))
      .ToList())
    .Should().Be(expected);
  }

  [Theory]
  [InlineData("Day13.Sample", 1068781)]
  [InlineData("Day13", 600689120448303)]
  public void Part2ViaCRT(string file, long expected)
  {
    var (timestamp, schedule) = Convert(AoCLoader.LoadLines(file));

    CRT(schedule.Select((id, index) => (id, index))
      .Where(it => it.id != -1)
      .Select(it => (remainder: MathMod(it.id - it.index, it.id), modulus: it.id))
      .ToList())
    .Should().Be(expected);
  }

  private static long MathMod(long a, long b)
  {
    return ((a % b) + b) % b;
  }

  private static BigInteger MathMod(BigInteger a, BigInteger b)
  {
    return ((a % b) + b) % b;
  }

  [Fact]
  public void CrtTest()
  {
    CRT([(0, 3), (3, 4), (4,5)]).Should().Be(39);
  }

  static long CRT(List<(long remainder, long modulus)> r)
  {
    checked {
    r = [.. r.OrderByDescending(it => it.modulus)];
    var current = r[0];
    foreach(var next in r[1..])
    {
      var ec = ExtendedEuclidAlgorithm(current.modulus, next.modulus);
      var vex = new BigInteger(next.remainder) * ec.coeffA * current.modulus + new BigInteger(current.remainder) * ec.coeffB * next.modulus;
      vex = MathMod(vex, current.modulus * next.modulus);
      current = ((long)vex, current.modulus * next.modulus);
    }

    return current.remainder;
    }
  }

  private static (long gcd, long coeffA, long coeffB) ExtendedEuclidAlgorithm(long a, long b)
  {
    // requirement a > b
    long r2 = a;
    long r1 = b;
    long s2 = 1;
    long s1 = 0;
    long t2 = 0;
    long t1 = 1;

    while (r1 != 0)
    {
      var q = r2 / r1;
      (r2, r1) = (r1, r2 - q * r1);
      (s2, s1) = (s1, s2 - q * s1);
      (t2, t1) = (t1, t2 - q * t1);
    }

    return (r2, s2, t2);
  }

  static long Sieve(List<(long remainder, long modulus)> r)
  {
    r = [.. r.OrderByDescending(it => it.modulus)];
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