
using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2020.CSharp;

public class Day01
{
  [Theory]
  [InlineData("Day01.Sample", 514579)]
  [InlineData("Day01", 270144)]
  public void Part1(string file, long expected)
  {
    var data = Convert(AoCLoader.LoadLines(file)).ToHashSet();

    const long target = 2020;
    long? result = null;
    foreach(var item in data)
    {
      if (data.Contains(target - item))
      {
        result = item * (target-item);
        break;
      }
    }

    result.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day01.Sample", 241861950)]
  [InlineData("Day01", 261342720)]
  public void Part2(string file, long expected)
  {
    var data = Convert(AoCLoader.LoadLines(file)).ToHashSet();
    GetSum2(data).Should().Be(expected);
  }

  private static long GetSum2(HashSet<long> data)
  {
    const long target = 2020;
    foreach (var item in data)
    {
      foreach (var item2 in data)
      {
        if (item2 == item) continue;
        if (data.Contains(target - item - item2))
        {
          return item * (target - item - item2) * item2;
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