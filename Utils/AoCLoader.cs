using System.Collections.Generic;
using System.IO;

namespace AdventOfCode2020.CSharp.Utils;

public static class AoCLoader
{
  public static List<string> LoadLines(string item)
  {
    var path = $"/home/jxbaker@net.sep.com/dev/AdventOfCode2020.Input/{item}.txt";
    return [.. File.ReadAllLines(path)];
  }

  public static string LoadFile(string item)
  {
    var path = $"/home/jxbaker@net.sep.com/dev/AdventOfCode2020.Input/{item}.txt";
    return File.ReadAllText(path);
  }
}