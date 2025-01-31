using System.Diagnostics.Metrics;
using System.Net.Sockets;
using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2020.CSharp;

public class Day20
{
  [Theory]
  [InlineData("Day20.Sample", 20899048083289)]
  [InlineData("Day20", 59187348943703)]
  public void Part1(string file, long expected)
  {
    var images = Convert(AoCLoader.LoadFile(file));

    var legals = Alignments(images.ToDictionary(it => it.Id, it => it))
      .Where(Legal).Select(Score).First().Should().Be(expected);
  }

  static long Score(Dictionary<long, Image> images)
  {
    return images.Where(image => image.Value.Matched.Count(it => it) == 2).Select(it => it.Key)
      .Aggregate((a,b) => a*b);

  }

  static bool Legal(Dictionary<long, Image> images) {
    var invalids = images.Values.Count(image => image.Matched.Count(it => it) < 2);
    if (invalids > 0) return false;
    var twos = images.Values.Count(image => image.Matched.Count(it => it) == 2);
    if (twos != 4) return false;
    var threes = images.Values.Count(image => image.Matched.Count(it => it) == 3);
    var fours = images.Values.Count(image => image.Matched.Count(it => it) == 4);
    
    var sideLength = images.Count == 9 ? 3 : images.Count == 144 ? 12 : throw new ApplicationException();
    if (threes != (sideLength-2) * 4) return false;
    return twos + threes + fours == images.Count;
  }

  static IEnumerable<Dictionary<long, Image>> Alignments(Dictionary<long, Image> images)
  {
    if (images.Count == 0) {
      yield return new();
      yield break;
    }
    var m1 = images.First().Value;
    var atLeastOne = false;
    foreach(var ll1 in Enumerable.Range(0, 4).Where(it => !m1.Matched[it]))
    {
      foreach(var other in images.Values.Where(it => it.Id != m1.Id) )
      {
        foreach(var ll2 in Enumerable.Range(0, 4).Where(it => !other.Matched[it]))
        {
          if (m1.LL[ll1].Intersect(other.LL[ll2]).Any())
          {
            atLeastOne = true;
            var clone = images.Clone();
            List<bool> mm1 = [..m1.Matched];
            List<bool> mm2 = [..other.Matched];
            mm1[ll1] = true;
            mm2[ll2] = true;
            clone[m1.Id] = m1 with {Matched = mm1};
            clone[other.Id] = other with {Matched = mm2};
            foreach(var next in Alignments(clone)) yield return next;
          }
        }
      }
    }
    if (!atLeastOne)
    {
      var clone = images.Clone();
      clone.Remove(m1.Id);
      foreach(var next in Alignments(clone)) {
        var clone2 = next.Clone();
        clone2[m1.Id] = m1;
        yield return clone2;
      }
    }
  }

  public record Image(long Id, List<List<char>> Data, IReadOnlyList<bool> Matched)
  {
    public List<List<string>> LL = [Get(Data[0]), Get(Data.Select(it => it[^1]).ToList()), Get(Data[^1]), Get(Data.Select(it => it[0]).ToList())];

    private static List<string> Get(List<char> input) {
      List<string> result = [input.Join()];
      input = [..input];
      input.Reverse();
      result.Add(input.Join());
      return result;
    }
  }

  private static List<Image> Convert(string input)
  {
    var pps = input.Split("\n\n").Select(it => it.Split("\n").ToList()).ToList();

    return pps.Select(pp => new Image(P.Format("Tile {}:", P.Long).Parse(pp[0]), pp[1..].Select(it => it.ToCharArray().ToList()).ToList(), [false, false, false, false])).ToList();
  }
}