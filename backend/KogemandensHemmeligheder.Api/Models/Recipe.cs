namespace KogemandensHemmeligheder.Api.Models;

public class Recipe
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int PreparationTimeInMinutes { get; set; }
    public int CookingTimeInMinutes { get; set; }
    public List<RecipeIngredient> RecipeIngredients { get; set; } = new();
    public List<string> Instructions { get; set; } = new();
}
