using System.Text;
using KogemandensHemmeligheder.Api.Data;
using KogemandensHemmeligheder.Api.DTOs;
using KogemandensHemmeligheder.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KogemandensHemmeligheder.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecipeController : ControllerBase
{
    private readonly RecipeDbContext _db;

    public RecipeController(RecipeDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<RecipeResponse>>> GetAllRecipes()
    {
        var recipes = await _db.Recipes
            .AsNoTracking()
            .Include(recipe => recipe.RecipeIngredients)
            .ThenInclude(recipeIngredient => recipeIngredient.Ingredient)
            .ToListAsync();

        return Ok(recipes.Select(ToResponse).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RecipeResponse>> GetRecipeById(int id)
    {
        var recipe = await _db.Recipes
            .AsNoTracking()
            .Include(recipe => recipe.RecipeIngredients)
            .ThenInclude(recipeIngredient => recipeIngredient.Ingredient)
            .FirstOrDefaultAsync(recipe => recipe.Id == id);

        return recipe is null
            ? NotFound()
            : Ok(ToResponse(recipe));
    }

    [HttpPost]
    public async Task<ActionResult<RecipeResponse>> CreateRecipe(CreateRecipeRequest request)
    {
        var recipeName = request.Name.Trim();
        if (recipeName.Length == 0)
        {
            ModelState.AddModelError(nameof(request.Name), "A recipe name is required.");
            return ValidationProblem(ModelState);
        }

        var ingredientLines = request.Ingredients
            .Select(line => new
            {
                Name = line.Name.Trim(),
                NormalizedName = NormalizeIngredientName(line.Name),
                line.Unit,
                line.Quantity
            })
            .ToList();

        if (ingredientLines.Any(line => line.Name.Length == 0))
        {
            ModelState.AddModelError(nameof(request.Ingredients), "Ingredient names cannot be empty.");
            return ValidationProblem(ModelState);
        }

        var normalizedNames = ingredientLines
            .Select(line => line.NormalizedName)
            .Distinct()
            .ToList();

        var ingredientsByName = await _db.Ingredients
            .Where(ingredient => normalizedNames.Contains(ingredient.NormalizedName))
            .ToDictionaryAsync(ingredient => ingredient.NormalizedName);

        var recipe = new Recipe
        {
            Name = recipeName,
            Description = request.Description?.Trim(),
            PreparationTimeInMinutes = request.PreparationTimeInMinutes,
            CookingTimeInMinutes = request.CookingTimeInMinutes,
            Instructions = request.Instructions
        };

        foreach (var line in ingredientLines)
        {
            if (!ingredientsByName.TryGetValue(line.NormalizedName, out var ingredient))
            {
                ingredient = new Ingredient
                {
                    Name = line.Name,
                    NormalizedName = line.NormalizedName
                };
                _db.Ingredients.Add(ingredient);
                ingredientsByName.Add(line.NormalizedName, ingredient);
            }

            recipe.RecipeIngredients.Add(new RecipeIngredient
            {
                Recipe = recipe,
                Ingredient = ingredient,
                Unit = line.Unit,
                Quantity = line.Quantity
            });
        }

        _db.Recipes.Add(recipe);
        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetRecipeById),
            new { id = recipe.Id },
            ToResponse(recipe));
    }

    private static string NormalizeIngredientName(string name) =>
        name.Trim().Normalize(NormalizationForm.FormKC).ToUpperInvariant();

    private static RecipeResponse ToResponse(Recipe recipe) => new(
        recipe.Id,
        recipe.Name,
        recipe.Description,
        recipe.PreparationTimeInMinutes,
        recipe.CookingTimeInMinutes,
        recipe.RecipeIngredients
            .Select(recipeIngredient => new RecipeIngredientResponse(
                recipeIngredient.Ingredient.Id,
                recipeIngredient.Ingredient.Name,
                recipeIngredient.Quantity,
                recipeIngredient.Unit))
            .ToList(),
        recipe.Instructions);
}
