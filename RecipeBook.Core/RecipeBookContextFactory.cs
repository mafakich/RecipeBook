using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RecipeBook.Core;

public class RecipeBookContextFactory : IDesignTimeDbContextFactory<RecipeBookContext>
{
    public RecipeBookContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RecipeBookContext>();
        optionsBuilder.UseSqlite("Data Source=recipes.db");
        return new RecipeBookContext(optionsBuilder.Options);
    }
}