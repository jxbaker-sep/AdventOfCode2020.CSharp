using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2020.CSharp;

public class Day21
{
  [Theory]
  [InlineData("Day21.Sample", 5, "mxmxvkd,sqjhc,fvjkl")]
  [InlineData("Day21", 1829, "mxkh,gkcqxs,bvh,sp,rgc,krjn,bpbdlmg,tdbcfb")]
  public void Part1(string file, int expected, string expected2)
  {
    var foods = Convert(AoCLoader.LoadLines(file));

    var allIngredients = foods.SelectMany(it => it.Ingredients).Distinct().ToList();
    var allAllergens = foods.SelectMany(it => it.Allergens).Distinct().ToList();
    var a2i = allAllergens.ToDictionary(it => it, it => allIngredients.ToList());
    var onSome = allIngredients.ToList();
    foreach(var allergen in allAllergens)
    {
      var temp = foods.Where(it => it.Allergens.Contains(allergen))
        .Aggregate(allIngredients.ToList(), (acc, next) => acc.Intersect(next.Ingredients).ToList());
      a2i[allergen] = temp;
      onSome = onSome.Except(temp).ToList();
    }

    foods.SelectMany(it => it.Ingredients.Intersect(onSome)).Count().Should().Be(expected);

    Dictionary<string, string> mapped = [];

    while (a2i.Any(it => it.Value.Count > 0))
    {
      var (allergen, ingredient) = a2i.Where(it => it.Value.Count == 1).First();
      mapped[allergen] = ingredient[0];
      foreach(var key in a2i.Keys.ToList())
      {
        a2i[key] = a2i[key].Except(ingredient).ToList();
      }
    }

    mapped.OrderBy(it => it.Key).Select(it=>it.Value).Join(",").Should().Be(expected2);
  }

  public record Food(List<string> Ingredients, List<string> Allergens);

  private static List<Food> Convert(List<string> input)
  {
    return P.Format("{} (contains {})", P.Word.Trim().Plus(), P.Word.Trim().Plus(","))
      .Select(it => new Food(it.First, it.Second))
      .ParseMany(input);
  }
}