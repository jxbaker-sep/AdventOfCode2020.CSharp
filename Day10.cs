using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2020.CSharp;

public class Day10
{
  [Theory]
  [InlineData("Day10.Sample.1", 7 * 5)]
  [InlineData("Day10.Sample.2", 22 * 10)]
  [InlineData("Day10", 2346)]
  public void Part1(string file, long expected)
  {
    var data = Convert(AoCLoader.LoadLines(file));
    
    data.Sort();
    data = [0, ..data, data[^1] + 3];
    var ds = data.Windows(2).Select(it => it[1] - it[0])
      .GroupToDictionary(it => it, it => it, it => (long)it.Count);

    (ds[1] * ds[3]).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day10.Sample.1", 8)]
  [InlineData("Day10.Sample.2", 19208)]
  [InlineData("Day10", 6044831973376)]
  public void Part2(string file, long expected)
  {
    var data = Convert(AoCLoader.LoadLines(file));
    
    data.Sort();
    data = [0, ..data, data[^1] + 3];
    Arrange(data).Should().Be(expected);
  }

  Dictionary<string, long> cache = [];

  long Arrange(List<long> data)
  {
    if (data.Count == 1) return 1;
    var key = data.Join(",");
    if (cache.TryGetValue(key, out var cached)) return cached;
    var self = data[0];
    long result = 0;
    if (data[1] <= self + 3) result += Arrange(data[1..]);
    if (data.Count >= 3 && data[2] <= self + 3) result += Arrange(data[2..]);
    if (data.Count >= 4 && data[3] <= self + 3) result += Arrange(data[3..]);
    cache[key] = result;
    return result;
  }

  private static List<long> Convert(List<string> list)
  {
    return P.Long.ParseMany(list);
  }
}