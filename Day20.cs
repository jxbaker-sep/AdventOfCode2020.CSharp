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

    // Sanity check: every image has a unique set of keys
    images.All(image => image.LL.SelectMany(it => it).Distinct().Count() == image.LL.SelectMany(it => it).Count())
      .Should().BeTrue();

    var legals = Alignments(images.ToDictionary(it => it.Id, it => it))
      .Where(Legal).Select(Score).First().Should().Be(expected);
  }

  static long Score(Dictionary<long, Image> images)
  {
    return images.Where(image => image.Value.Matched.Count(it => it != null) == 2).Select(it => it.Key)
      .Aggregate((a,b) => a*b);

  }

  static bool Legal(Dictionary<long, Image> images) {
    var invalids = images.Values.Count(image => image.Matched.Count(it => it != null) < 2);
    if (invalids > 0) return false;
    var twos = images.Values.Count(image => image.Matched.Count(it => it != null) == 2);
    if (twos != 4) return false;
    var threes = images.Values.Count(image => image.Matched.Count(it => it != null) == 3);
    var fours = images.Values.Count(image => image.Matched.Count(it => it != null) == 4);
    
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
    foreach(var ll1 in Enumerable.Range(0, 4).Where(it => m1.Matched[it] == null))
    {
      foreach(var other in images.Values.Where(it => it.Id != m1.Id) )
      {
        foreach(var ll2 in Enumerable.Range(0, 4).Where(it => other.Matched[it] == null))
        {
          if (m1.LL[ll1].Intersect(other.LL[ll2]).Any())
          {
            atLeastOne = true;
            var clone = images.Clone();
            List<long?> mm1 = [..m1.Matched];
            List<long?> mm2 = [..other.Matched];
            mm1[ll1] = other.Id;
            mm2[ll2] = m1.Id;
            clone[m1.Id] = m1 with {Matched = mm1};
            clone[other.Id] = other with {Matched = mm2};
            foreach(var next in Alignments(clone)) yield return next;
            yield break;
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

  [Fact]
  public void TransformationSanity()
  {
    Image sample = new(0, [['A', 'B', 'C'], ['D', 'E', 'F'], ['H', 'I', 'J']], [0,null,2,3]);
    for(var y = 0; y < 3; y++) 
    {
      Console.WriteLine(sample.Data[y].Join());
    }
    Console.WriteLine("xxxxxxxxxxx");
    Console.WriteLine();
    for(var y = 0; y < 3; y++) 
    {
      Console.WriteLine(sample.RotateRight().Data[y].Join());
    }
  }

  public record Image(long Id, List<List<char>> Data, IReadOnlyList<long?> Matched)
  {
    public List<List<string>> LL = [Get(Data[0]), Get(Data.Select(it => it[^1]).ToList()), Get(Data[^1]), Get(Data.Select(it => it[0]).ToList())];

    private static List<string> Get(List<char> input) {
      List<string> result = [input.Join()];
      input = [..input];
      input.Reverse();
      result.Add(input.Join());
      return result;
    }

    public Image FlipVertically()
    {
      List<List<char>> copy = [..Data];
      copy.Reverse();
      return new(Id, copy, [Matched[2], Matched[1], Matched[0], Matched[3]]);
    }

    public Image FlipHorizontally()
    {
      List<List<char>> copy = [..Data.Select(d => {List<char> c = [..d]; c.Reverse(); return c;})];
      return new(Id, copy, [Matched[0], Matched[3], Matched[2], Matched[1]]);
    }

    public Image RotateRight()
    {
      List<List<char>> copy = [];
      for(var y = 0; y < Data[0].Count; y++)
      {
        List<char> row = [];
        copy.Add(row);
        for(var x = Data.Count-1; x >= 0; x--)
        {
          row.Add(Data[x][y]);
        }
      }
      return new(Id, copy, [Matched[3], Matched[0], Matched[1], Matched[2]]);
    }
  }

  private static List<Image> Convert(string input)
  {
    var pps = input.Split("\n\n").Select(it => it.Split("\n").ToList()).ToList();

    return pps.Select(pp => new Image(P.Format("Tile {}:", P.Long).Parse(pp[0]), pp[1..].Select(it => it.ToCharArray().ToList()).ToList(), [null, null, null, null])).ToList();
  }
}