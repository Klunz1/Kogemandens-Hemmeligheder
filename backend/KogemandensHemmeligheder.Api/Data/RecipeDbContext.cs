using KogemandensHemmeligheder.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace KogemandensHemmeligheder.Api.Data;

public class RecipeDbContext(DbContextOptions<RecipeDbContext> options)
    : DbContext(options)
{
    public DbSet<Recipe> Recipes => Set<Recipe>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Recipe>()
            .HasMany(recipe => recipe.Ingredients)
            .WithOne()
            .HasForeignKey("RecipeId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}