
using System.Linq;
using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2020.CSharp;

public class Day09
{
  [Theory]
  [InlineData("Day09.Sample", 5, 127)]
  [InlineData("Day09", 25, 26134589)]
  public void Part1(string file, int preamble, long expected)
  {
    var data = Convert(AoCLoader.LoadLines(file));
    
    var current = new LinkedList<long>();
    foreach(var item in data.Take(preamble)) current.AddLast(item);
    var sums = current.ToList().Pairs().Select(it => it.First + it.Second);
    var r_sum = sums.GroupToDictionary(it => it, it => it, it => it.Count);

    foreach(var next in data.Skip(preamble))
    {
      if (!r_sum.TryGetValue(next, out var cached) || cached == 0) 
      {
        next.Should().Be(expected);
        return;
      }
      var removed = current.First!.Value;
      current.RemoveFirst();
      foreach(var other in current) {
        r_sum[other + next] = r_sum.GetValueOrDefault(other + next) + 1;
        r_sum[removed + other] -= 1;
      }
      
      current.AddLast(next);
    }
    throw new ApplicationException();
  }

  [Theory]
  [InlineData("Day09.Sample", 127, 62)]
  [InlineData("Day09", 26134589, 3535124)]
  public void Part2(string file, long weakness, long expected)
  {
    var data = Convert(AoCLoader.LoadLines(file));
    
    for(var first = 0; first < data.Count - 1; first++)
    {
      long current = data[first];
      var min = current;
      var max = current;
      for(var last = first + 1; last < data.Count && current < weakness; last++)
      {
        current += data[last];
        min = Math.Min(min, data[last]);
        max = Math.Max(max, data[last]);
        if (current == weakness)
        {
          (min + max).Should().Be(expected);
          return;
        }
      }
    }
    throw new ApplicationException();
  }

  private static List<long> Convert(List<string> list)
  {
    return P.Long.ParseMany(list);
  }
}