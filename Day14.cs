using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2020.CSharp;

public class Day14
{
  [Theory]
  [InlineData("Day14.Sample", 165)]
  [InlineData("Day14", 14722016054794)]
  public void Part1(string file, long expected)
  {
    var program = Convert(AoCLoader.LoadLines(file));
    
    var mask = "";
    Dictionary<long, long> cache = [];
    foreach(var item in program)
    {
      if (item is MaskInstruction mi)
      {
        mask = mi.Mask.Reverse().Join();
      }
      else if (item is MemInstruction mem)
      {
        cache[mem.Address] = DoMask(mask, mem.Value);
      }
    }

    cache.Values.Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day14.Sample.2", 208)]
  [InlineData("Day14", 3618217244644)]
  public void Part2(string file, long expected)
  {
    var program = Convert(AoCLoader.LoadLines(file));
    
    var mask = "";
    Dictionary<long, long> cache = [];
    foreach(var item in program)
    {
      if (item is MaskInstruction mi)
      {
        mask = mi.Mask.Reverse().Join();
      }
      else if (item is MemInstruction mem)
      {
        foreach(var address in GetAddresses(mask, mem.Address))
        {
          cache[address] = mem.Value;
        }
      }
    }

    cache.Values.Sum().Should().Be(expected);
  }

  private static IEnumerable<long> GetAddresses(string mask, long address)
  {
    if (mask.Length == 0) {
      yield return 0; 
      yield break;
    }
    foreach(var remainder in GetAddresses(mask[1..], address >> 1))
    {
      if (mask[0] == '0') yield return (remainder << 1) + (address & 1);
      if (mask[0] == '1') yield return (remainder << 1) + 1;
      if (mask[0] == 'X') { 
        yield return (remainder << 1) + 0;
        yield return (remainder << 1) + 1;
      }
    }
  }

  public static long DoMask(string mask, long value)
  {
    var result = 0L;
    for(var i = 0; i < 36; i++)
    {
      if (mask[i] == '0') {} // continue
      else if (mask[i] == '1') result += (1L << i);
      else result += value & (1L << i);
    }
    return result;
  }

  public interface IEntry;
  public record MaskInstruction(string Mask): IEntry;
  public record MemInstruction(long Address, long Value) : IEntry;

  private static List<IEntry> Convert(List<string> list)
  {
    Parser<IEntry> maskp = P.Format("mask = {}", P.Any.Star().Join()).End().Select(it => new MaskInstruction(it) as IEntry);
    Parser<IEntry> memp = P.Format("mem[{}] = {}", P.Long, P.Long).End().Select(it => (IEntry)new MemInstruction(it.First, it.Second));
    return (maskp | memp).ParseMany(list);
  }
}