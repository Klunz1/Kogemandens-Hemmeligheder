using KogemandensHemmeligheder.Api.Enums;

namespace KogemandensHemmeligheder.Api.DTOs;

public sealed record RecipeResponse(
    int Id,
    string Name,
    string? Description,
    int PreparationTimeInMinutes,
    int CookingTimeInMinutes,
    List<RecipeIngredientResponse> Ingredients,
    List<string> Instructions);

public sealed record RecipeIngredientResponse(
    int IngredientId,
    string Name,
    decimal Quantity,
    MeasurementUnit Unit);
