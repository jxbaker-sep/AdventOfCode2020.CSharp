
using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2020.CSharp;

public class Day02
{
  [Theory]
  [InlineData("Day02.Sample", 2)]
  [InlineData("Day02", 383)]
  public void Part1(string file, int expected)
  {
    var data = Convert(AoCLoader.LoadLines(file)).ToHashSet();
    data.Count(Valid1).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day02.Sample", 1)]
  [InlineData("Day02", 272)]
  public void Part2(string file, int expected)
  {
    var data = Convert(AoCLoader.LoadLines(file)).ToHashSet();
    data.Count(Valid2).Should().Be(expected);
  }

  private bool Valid1(PolicyAndPassword pap)
  {
    var x = pap.Password.Where(c => c == pap.Required).Count();
    return pap.Min <= x && x <= pap.Max;
  }

  private bool Valid2(PolicyAndPassword pap)
  {
    var first = pap.Password[pap.Min-1] == pap.Required;
    var second = pap.Password[pap.Max-1] == pap.Required;
    return first ^ second;
  }

  public record PolicyAndPassword(int Min, int Max, char Required, string Password);

  private static List<PolicyAndPassword> Convert(List<string> list)
  {
    return P.Format("{}-{} {}: {}", P.Int, P.Int, P.Any, P.Word)
      .Select(it => new PolicyAndPassword(it.First, it.Second, it.Third, it.Fourth))
      .ParseMany(list);
  }
}