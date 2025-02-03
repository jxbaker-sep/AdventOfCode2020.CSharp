using AdventOfCode2020.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2020.CSharp;

public class Day21
{
  [Theory]
  [InlineData("Day21.Sample", 5)]
  [InlineData("Day21", 1829)]
  public void Part1(string file, int expected)
  {
    var foods = Convert(AoCLoader.LoadLines(file));

    var allIngredients = foods.SelectMany(it => it.Ingredients).Distinct().ToList();
    var allAllergens = foods.SelectMany(it => it.Allergens).Distinct().ToList();
    var i2a = allIngredients.ToDictionary(it => it, it => allAllergens.ToList());
    var onSome = allIngredients.ToList();
    foreach(var allergen in allAllergens)
    {
      var temp = foods.Where(it => it.Allergens.Contains(allergen))
        .Aggregate(allIngredients.ToList(), (acc, next) => acc.Intersect(next.Ingredients).ToList());
      onSome = onSome.Except(temp).ToList();
    }

    foods.SelectMany(it => it.Ingredients.Intersect(onSome)).Count().Should().Be(expected);
  }

  public record Food(List<string> Ingredients, List<string> Allergens);

  private static List<Food> Convert(List<string> input)
  {
    return P.Format("{} (contains {})", P.Word.Trim().Plus(), P.Word.Trim().Plus(","))
      .Select(it => new Food(it.First, it.Second))
      .ParseMany(input);
  }
}