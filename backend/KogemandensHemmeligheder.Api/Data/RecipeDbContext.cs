using KogemandensHemmeligheder.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace KogemandensHemmeligheder.Api.Data;

public class RecipeDbContext(DbContextOptions<RecipeDbContext> options)
    : DbContext(options)
{
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ingredient>()
            .HasIndex(ingredient => ingredient.NormalizedName)
            .IsUnique();

        modelBuilder.Entity<Recipe>()
            .HasMany(recipe => recipe.RecipeIngredients)
            .WithOne(recipeIngredient => recipeIngredient.Recipe)
            .HasForeignKey(recipeIngredient => recipeIngredient.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(recipeIngredient => recipeIngredient.Ingredient)
            .WithMany()
            .HasForeignKey(recipeIngredient => recipeIngredient.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}