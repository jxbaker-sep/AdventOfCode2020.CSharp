namespace AdventOfCode2020.CSharp.Utils;

public record Hex(long X, long Y)
{
  public static Hex Zero {get;} = new(0, 0);

  public static Hex operator+(Hex h, HexVector v) => new(h.X + v.dX, h.Y + v.dY);
}

public record HexVector(long dX, long dY)
{
  public static HexVector North {get;} = new(0, 1);
  public static HexVector NorthEast {get;} = new(1, 1);
  public static HexVector NorthWest {get;} = new(-1, 0);
  public static HexVector South {get;} = new(0, -1);
  public static HexVector SouthEast {get;} = new(1, 0);
  public static HexVector SouthWest {get;} = new(-1, -1);
}