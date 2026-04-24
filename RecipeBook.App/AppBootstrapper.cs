using Microsoft.EntityFrameworkCore;
using RecipeBook.App.Services;
using RecipeBook.Core;

namespace RecipeBook.App;

public static class AppBootstrapper
{
    public static RecipeService GetService()
    {
        var options = new DbContextOptionsBuilder<RecipeBookContext>()
            .UseSqlite("Data Source=recipes.db")
            .Options;

        using var ctx = new RecipeBookContext(options);
        ctx.Database.EnsureCreated();
        return new RecipeService(() => new RecipeBookContext(options));
    }
}