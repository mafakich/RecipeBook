using System.Collections.ObjectModel;

namespace RecipeBook.Core;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}

public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}

public class RecipeStep
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public int StepNumber { get; set; }
    public string Text { get; set; } = string.Empty;
    public Recipe? Recipe { get; set; }
}

public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public int CookTimeMinutes { get; set; }
    public Difficulty Difficulty { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    public ICollection<RecipeStep> Steps { get; set; } = new List<RecipeStep>();
}

public class Product
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Weight { get; set; }
}