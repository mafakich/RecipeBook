using Microsoft.EntityFrameworkCore;

namespace RecipeBook.Core;

public class RecipeBookContext : DbContext
{
    public RecipeBookContext(DbContextOptions<RecipeBookContext> options) : base(options) { }

    public DbSet<Recipe> Recipes { get; set; } = null!;
    public DbSet<Ingredient> Ingredients { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<RecipeStep> RecipeSteps { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Recipe>()
            .HasOne(r => r.Category)
            .WithMany(c => c.Recipes)
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Recipe>()
            .HasMany(r => r.Ingredients)
            .WithMany(i => i.Recipes)
            .UsingEntity<Dictionary<string, object>>(
                "RecipeIngredients",
                j => j.HasOne<Ingredient>().WithMany().HasForeignKey("IngredientId"),
                j => j.HasOne<Recipe>().WithMany().HasForeignKey("RecipeId"),
                j => j.HasKey("RecipeId", "IngredientId"));

        modelBuilder.Entity<RecipeStep>()
            .HasOne(rs => rs.Recipe)
            .WithMany(r => r.Steps)
            .HasForeignKey(rs => rs.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Recipe>().Property(r => r.Title).IsRequired();
    }
}