using System.Diagnostics.Metrics;
using System.Net.Sockets;
using System.Threading.Tasks.Dataflow;
using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Newtonsoft.Json.Bson;
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

  [Theory]
  [InlineData("Day20.Sample", 273)]
  [InlineData("Day20", 1565)]
  public void Part2(string file, int expected)
  {
    var images = Convert(AoCLoader.LoadFile(file));

    var legal = Alignments(images.ToDictionary(it => it.Id, it => it))
      .Where(Legal).First();

    var corner = legal.Values.Where(it => it.Matched.Count(z => z != null) == 2).First();
    // rotate into top-left
    while (corner.Matched[0] != null || corner.Matched[3] != null) corner = corner.RotateRight();
    List<List<Image>> grid = [[corner]];
    var rowIndex = -1;
    while (true)
    {
      rowIndex++;
      var row = rowIndex == 0 ? grid[0] : [];
      var previous = rowIndex == 0 ? corner : grid[rowIndex-1][0];
      if (rowIndex != 0)
      {
        if (previous.Matched[2] == null) break;
        grid.Add(row);
        var next = legal[previous.Matched[2] ?? throw new ApplicationException()].Align(previous.LL[2][0], 0);
        row.Add(next);
        previous = next;
      }
      while (previous.Matched[1] is {} nextId)
      {
        var next = legal[nextId].Align(previous.LL[1][0], 3);
        row.Add(next);
        previous = next;
      }
    }

    var sideLength = grid.Count;
    var masterImage = Enumerable.Range(0, sideLength * 8)
      .Select(_ => Enumerable.Repeat('*', sideLength * 8).ToList()).ToList();
    for(var y = 0; y < sideLength; y++)
    {
      for(var x = 0; x < sideLength; x++)
      {
        var image = grid[y][x];
        for(var y2 = 1; y2 < image.Data.Count-1; y2++)
        {
          for(var x2 = 1; x2 < image.Data[0].Count-1; x2++)
          {
            masterImage[y * 8 + y2 - 1][x * 8 + x2 - 1] = image.Data[y2][x2];
          }
        }
      }
    }

    var mip = new Image(-1, masterImage, [null,null,null,null]);
    
    var sentinel = mip.Data.SelectMany(it => it).Count(it => it == '#');

    foreach(var it in new List<Image>{mip, mip.RotateRight(), mip.RotateRight().RotateRight(),mip.RotateRight().RotateRight().RotateRight(),
                    mip.FlipHorizontally(), mip.FlipHorizontally().RotateRight(),mip.FlipHorizontally().RotateRight().RotateRight(),mip.FlipHorizontally().RotateRight().RotateRight().RotateRight()})
    {
      var x = FindSeaMonsters(it);
      if (x != 0)
      {
        (sentinel - x).Should().Be(expected);
        return;
      }
    }
    throw new ApplicationException();
  }

  static int FindSeaMonsters(Image image)
  {
    var seaMonster = AoCLoader.LoadLines("Day20.SeaMonster").Gridify().Where(kv => kv.Value == '#')
      .Select(it => new Vector(it.Key.Y, it.Key.X)).ToList();

    var maxX = seaMonster.Max(it => it.X);
    var maxY = seaMonster.Max(it => it.Y);
    HashSet<Point> found = [];
    for(var y = 0; y < image.Data.Count - maxY; y++)
    {
      for(var x = 0; x < image.Data.Count - maxX; x++)
      {
        var p = new Point(y,x);
        if (seaMonster.Select(v => p + v).All(p2 => image.Data[(int)p2.Y][(int)p2.X] == '#'))
        {
          foreach(var p2 in seaMonster.Select(v => p + v)) found.Add(p2);
        }
      }
    }
    return found.Count;
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

  public record Image(long Id, List<List<char>> Data, IReadOnlyList<long?> Matched)
  {
    public List<List<string>> LL = [Get(Data[0]), Get(Data.Select(it => it[^1]).ToList()), Get(Data[^1]), Get(Data.Select(it => it[0]).ToList())];

    public void Print()
    {
      foreach(var y in Enumerable.Range(0, Data.Count))
      {
        Console.WriteLine(Data[y].Join());
      }
      Console.WriteLine("====================================");
    }

    private static List<string> Get(List<char> input) {
      List<string> result = [input.Join()];
      input = [..input];
      input.Reverse();
      result.Add(input.Join());
      return result;
    }

    public Image Align(string key, int incoming)
    {
      var index = LL.WithIndices().First(ll => ll.Value.Contains(key)).Index;
      var rotations = incoming - index;
      if (rotations < 0) rotations += 4;
      var result = this;
      foreach(var _ in Enumerable.Range(0, rotations)) result = result.RotateRight();
      if (result.LL[incoming][1] == key) 
      {
        if (incoming == 3) result = result.FlipVertically();
        else result = result.FlipHorizontally();
      } 
      // sanity
      result.LL[incoming][0].Should().Be(key);
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