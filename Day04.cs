
using System.Linq;
using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2020.CSharp;

public class Day04
{
  [Theory]
  [InlineData("Day04.Sample", 2)]
  [InlineData("Day04", 216)]
  public void Part1(string file, int expected)
  {
    var data = Convert(AoCLoader.LoadFile(file)).ToHashSet();
    
    data.Count(Valid1).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day04", 150)]
  public void Part2(string file, int expected)
  {
    var data = Convert(AoCLoader.LoadFile(file)).ToHashSet();
    
    data.Where(Valid1).Count(Valid2).Should().Be(expected);
  }

  [Fact]
  public void Sanity1()
  {
    var g= @"eyr:1972 cid:100
hcl:#18171d ecl:amb hgt:170 pid:186cm iyr:2018 byr:1926

iyr:2019
hcl:#602927 eyr:1967 hgt:170cm
ecl:grn pid:012533040 byr:1946

hcl:dab227 iyr:2012
ecl:brn hgt:182cm pid:021572410 eyr:2020 byr:1992 cid:277

hgt:59cm ecl:zzz
eyr:2038 hcl:74454a iyr:2023
pid:3556412378 byr:2007";
    Convert(g).Where(Valid1).Count(Valid2).Should().Be(0);
  }

  [Fact]
  public void Sanity2()
  {
    var g= @"pid:087499704 hgt:74in ecl:grn iyr:2012 eyr:2030 byr:1980
hcl:#623a2f

eyr:2029 ecl:blu cid:129 byr:1989
iyr:2014 pid:896056539 hcl:#a97842 hgt:165cm

hcl:#888785
hgt:164cm byr:2001 iyr:2015 cid:88
pid:545766238 ecl:hzl
eyr:2022

iyr:2010 hgt:158cm hcl:#b6652a ecl:blu byr:1944 eyr:2021 pid:093154719";
    Convert(g).Where(Valid1).Count(Valid2).Should().Be(4);
  }

  [Fact]
  public void Sanity3()
  {
    ValidByr("2002").Should().BeTrue();
    ValidByr("2003").Should().BeFalse();

    ValidHgt("60in").Should().BeTrue();
    ValidHgt("190cm").Should().BeTrue();
    ValidHgt("190in").Should().BeFalse();
    ValidHgt("190").Should().BeFalse();

    ValidHcl("#123abc").Should().BeTrue();
    ValidHcl("#123abz").Should().BeFalse();
    ValidHcl("123abc").Should().BeFalse();

    ValidEcl("brn").Should().BeTrue();
    ValidEcl("wat").Should().BeFalse();

    ValidPid("000000001").Should().BeTrue();
    ValidPid("0123456789").Should().BeFalse();
  }

  private bool Valid1(Passport passport)
  {
    return passport.Fields.Keys.Except(new[]{"cid"}).Count() == 7;
  }

  private bool Valid2(Passport passport)
  {
    return ValidByr(passport.Fields["byr"]) &&
    ValidIyr(passport.Fields["iyr"]) &&
    ValidEyr(passport.Fields["eyr"]) &&
    ValidHgt(passport.Fields["hgt"]) &&
    ValidHcl(passport.Fields["hcl"]) &&
    ValidEcl(passport.Fields["ecl"]) &&
    ValidPid(passport.Fields["pid"]);
  }


  public bool ValidByr(string byr) => P.Digit.Range(4, 4).Join().End()
    .Select(System.Convert.ToInt32).Where(it => 1920 <= it && it <= 2002).TryParse(byr, out var _); 
  public bool ValidIyr(string iyr) => P.Digit.Range(4, 4).Join().End()
    .Select(System.Convert.ToInt32).Where(it => 2010 <= it && it <= 2020).TryParse(iyr, out var _); 
  public bool ValidEyr(string eyr) => P.Digit.Range(4, 4).Join().End()
    .Select(System.Convert.ToInt32).Where(it => 2020 <= it && it <= 2030).TryParse(eyr, out var _); 
  public bool ValidHgt(string hgt) => (
        P.Format("{}cm", P.Long).Where(it => 150 <= it && it <= 193).End()
        | P.Format("{}in", P.Long).Where(it => 59 <= it && it <= 76).End()
      ).TryParse(hgt, out var _);
  public bool ValidHcl(string hcl) => P.Format("#{}", P.Any.Where(it => char.IsDigit(it) || ('a' <= it && it <= 'f')).Range(6,6)).End().TryParse(hcl, out var _);
  public bool ValidEcl(string hcl) => P.Choice("amb", "blu", "brn", "gry", "grn", "hzl", "oth").End().TryParse(hcl, out var _);
  public bool ValidPid(string hcl) => P.Digit.Range(9,9).End().TryParse(hcl, out var _);
  

  public record Passport(Dictionary<string, string> Fields);

  private static List<Passport> Convert(string list)
  {
    var pps = list.Split("\n\n").ToList();
    var notSpace = P.Any.Where(it => !char.IsWhiteSpace(it));
    var field = P.Format("{}:{}", P.Word, notSpace.Plus().Join()).Trim();
    var passport = field.Star().Select(it => new Passport(it.ToDictionary(z => z.First, z => z.Second)));
    return passport.ParseMany(pps);
  }
}