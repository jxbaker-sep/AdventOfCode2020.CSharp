using System.Net.Http.Headers;
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
    Compute(100, input).Should().Be(expected);
  }

  [Theory]
  [InlineData("389125467", "67384529")]
  public void Sanity1(string input, string expected)
  {
    Compute(100, input).Should().Be(expected);
  }

  private static string Compute(int iterations, string input)
  {
    var l = new LinkedList<int>(input.ToCharArray().Select(it => it - '0'));
    var hv = l.Max();
    var current = l.First!;
    foreach(var _ in Enumerable.Range(0, iterations))
    {
      var n1 = current.NextWrapped();
      var n2 = n1.NextWrapped();
      var n3 = n2.NextWrapped();
      l.Remove(n1);
      l.Remove(n2);
      l.Remove(n3);
      var destinationLabel = current.Value - 1;
      while (new[]{n1.Value, n2.Value, n3.Value}.Contains(destinationLabel) || destinationLabel < 1)
      {
        if (destinationLabel < 1) { destinationLabel = hv; continue; }
        destinationLabel--;
      }
      var destinationCup = l.Nodes().First(n => n.Value == destinationLabel);
      l.AddAfter(destinationCup, n1);
      l.AddAfter(n1, n2);
      l.AddAfter(n2, n3);
      current = current.NextWrapped();
    }

    var one = l.Nodes().First(n => n.Value == 1).NextWrapped();
    var result = "";
    while (one.Value != 1)
    {
      result = $"{result}{one.Value}";
      one = one.NextWrapped();
    }
    return result;
  }
}
