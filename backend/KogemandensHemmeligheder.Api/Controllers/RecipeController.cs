using KogemandensHemmeligheder.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KogemandensHemmeligheder.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private static readonly List<Recipe> Recipes = [];
        private static int _nextId = 1;

        [HttpGet]
        public ActionResult<List<Recipe>> GetAllRecipes()
        {
            return Ok(Recipes);
        }

        [HttpGet("{id:int}")]
        public ActionResult<Recipe> GetRecipeById(int id)
        {
            var recipe = Recipes.FirstOrDefault(recipe => recipe.Id == id);

            return recipe is null
            ? NotFound()
            : Ok(recipe);
        }

        [HttpPost]
        public ActionResult<Recipe> CreateRecipe(Recipe recipe)
        {
            recipe.Id = _nextId++;
            Recipes.Add(recipe);

            return CreatedAtAction(
                nameof(GetRecipeById),
                new { id = recipe.Id },
                recipe);
        }
    }
}
