using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;

namespace AdventOfCode2020.CSharp;

public class Day23
{
  [Theory]
  [InlineData("Day23", "26354798")]
  public void Part1(string file, string expected)
  {
    var input = AoCLoader.LoadLines(file).First();
    Compute(100, input, false).Join().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day23", 166298218695L)]
  public void Part2(string file, long expected)
  {
    var input = AoCLoader.LoadLines(file).First();
    Compute(10_000_000, input, true).Take(2).Select(it=>(long)it).Product().Should().Be(expected);
  }

  [Theory]
  [InlineData("12345", 1, "5234")]
  [InlineData("389125467", 1, "54673289")]
  [InlineData("389125467", 100, "67384529")]
  public void Sanity1(string input, int iterations, string expected)
  {
    Compute(iterations, input, false).Join().Should().Be(expected);
  }

  [Theory]
  [InlineData("389125467", 10_000_000, 149245887792)]
  public void Sanity2(string input, int iterations, long expected)
  {
    var z = Compute(iterations, input, true)
      .Select(it => (long)it).Take(2).ToList();
    Console.WriteLine($"{z[0]} * {z[1]}");
    z.Product()
      .Should().Be(expected);
  }

  private static IEnumerable<int> Compute(int iterations, string input, bool extend)
  {
    const int oneMillion = 1_000_000;
    var numbers = input.ToCharArray().Select(it => it - '0').ToList();
    var hv = numbers.Max();
    int[] after = Enumerable.Repeat(0, Math.Max(extend ? oneMillion : 0, hv) + 1).ToArray();
    foreach(var (number, index) in numbers.WithIndices()) 
    {
      after[number] = numbers[(index+1)%numbers.Count];
    }
    if (extend)
    {
      after[numbers[^1]] = hv+1;
      foreach(var index in Enumerable.Range(hv+1, oneMillion - hv))
      {
        after[index] = index + 1;
      }
      after[oneMillion] = numbers[0];
    }
    var currentLabel = numbers[0];
    hv = Math.Max(extend ? oneMillion : 0, hv);
    foreach(var _ in Enumerable.Range(0, iterations))
    {
      var l1 = after[currentLabel];
      var l2 = after[l1];
      var l3 = after[l2];
      after[currentLabel] = after[l3];
      var nextLabel = currentLabel - 1;
      while (nextLabel < 1 || nextLabel == l1 || nextLabel == l2 || nextLabel == l3)
      {
        if (nextLabel < 1) {nextLabel = hv; continue;}
        nextLabel--;
      }
      var temp = after[nextLabel];
      after[nextLabel] = l1;
      after[l3] = temp;
      currentLabel = after[currentLabel];
    }

    var n = 1;
    while (after[n] != 1) {
      yield return after[n];
      n = after[n];
    }
  }
}
