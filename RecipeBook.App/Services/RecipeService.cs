using Microsoft.EntityFrameworkCore;
using RecipeBook.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeBook.App.Services
{
    public class RecipeService
    {
        private readonly Func<RecipeBookContext> _contextFactory;
        public RecipeService(Func<RecipeBookContext> contextFactory) => _contextFactory = contextFactory;

        public async Task<List<Recipe>> GetRecipesAsync(string? search = null, int? categoryId = null, int? maxTime = null)
        {
            await using var ctx = _contextFactory();
            var query = ctx.Recipes
                .Include(r => r.Category)
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .AsSplitQuery();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(r => r.Title.Contains(search) || r.Ingredients.Any(i => i.Name.Contains(search)));

            if (categoryId.HasValue) query = query.Where(r => r.CategoryId == categoryId.Value);
            if (maxTime.HasValue) query = query.Where(r => r.CookTimeMinutes <= maxTime.Value);

            return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        }

        public async Task SaveRecipeAsync(Recipe recipe)
        {
            await using var ctx = _contextFactory();
            var existing = await ctx.Recipes.Include(r => r.Ingredients).Include(r => r.Steps).FirstOrDefaultAsync(r => r.Id == recipe.Id);

            if (existing == null)
            {
                ctx.Recipes.Add(recipe);
            }
            else
            {
                ctx.Entry(existing).CurrentValues.SetValues(recipe);
                existing.CategoryId = recipe.CategoryId;
                existing.CookTimeMinutes = recipe.CookTimeMinutes;
                existing.Difficulty = recipe.Difficulty;
                existing.Description = recipe.Description;

                existing.Ingredients.Clear();
                foreach (var i in recipe.Ingredients) existing.Ingredients.Add(i);

                existing.Steps.Clear();
                foreach (var s in recipe.Steps) existing.Steps.Add(s);
            }
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteRecipeAsync(int id)
        {
            await using var ctx = _contextFactory();
            var recipe = await ctx.Recipes.FindAsync(id);
            if (recipe != null) { ctx.Recipes.Remove(recipe); await ctx.SaveChangesAsync(); }
        }

        public async Task<List<Category>> GetCategoriesAsync() => await _contextFactory().Categories.ToListAsync();
        public async Task<List<Ingredient>> GetAllIngredientsAsync() => await _contextFactory().Ingredients.ToListAsync();
    }
}