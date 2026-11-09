using KogemandensHemmeligheder.Api.Data;
using KogemandensHemmeligheder.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KogemandensHemmeligheder.Api.Controllers
{
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
        public async Task<ActionResult<List<Recipe>>> GetAllRecipes()
        {
            var recipes = await _db.Recipes
                .Include(recipe => recipe.Ingredients)
                .ToListAsync();

            return Ok(recipes);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Recipe>> GetRecipeById(int id)
        {
            var recipe = await _db.Recipes
                .Include(recipe => recipe.Ingredients)
                .FirstOrDefaultAsync(recipe => recipe.Id == id);

            return recipe is null
            ? NotFound()
            : Ok(recipe);
        }

        [HttpPost]
        public async Task<ActionResult<Recipe>> CreateRecipe(Recipe recipe)
        {
            _db.Recipes.Add(recipe);
            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetRecipeById),
                new { id = recipe.Id },
                recipe);
        }
    }
}
