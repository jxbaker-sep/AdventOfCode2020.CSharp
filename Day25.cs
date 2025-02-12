using System.Threading.Tasks.Dataflow;
using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;


namespace AdventOfCode2020.CSharp;

public class Day25
{
  [Theory]
  [InlineData("Day25", 10548634)]
  public void Part1(string file, long expected)
  {
    var keys = Convert(AoCLoader.LoadLines(file));
    Compute(keys[0], keys[1]).Should().Be(expected);
  }

  [Fact]
  public void Sanity()
  {
    Compute(5764801, 17807724).Should().Be(14897079);
  }

  long Compute(long cardPrivateKey, long doorPrivateKey)
  {
    var cardLoopSize = Detect(cardPrivateKey, 7);
    var doorLoopSize = Detect(doorPrivateKey, 7);
    var result = Generate(doorPrivateKey, cardLoopSize);
    Generate(cardPrivateKey, doorLoopSize).Should().Be(result);
    return result;
  }

  long Generate(long subjectNumber, long loopSize)
  {
    var result = subjectNumber;
    while (--loopSize > 0)
    {
      result = (result * subjectNumber) % 20201227;
    }
    return result;
  }

  long Detect(long publicKey, long subjectNumber)
  {
    var loopSize = 1;
    var result = subjectNumber;
    while (result != publicKey)
    {
      loopSize += 1;
      result = (result * 7) % 20201227;
    }
    return loopSize;
  }

  private static List<long> Convert(List<string> input)
  {
    return input.Select(it => System.Convert.ToInt64(it)).ToList();
  }
}
