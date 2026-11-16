using System.ComponentModel.DataAnnotations;
using KogemandensHemmeligheder.Api.Enums;

namespace KogemandensHemmeligheder.Api.DTOs;

public sealed class CreateRecipeRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public required string Name { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }
    public int PreparationTimeInMinutes { get; set; }
    public int CookingTimeInMinutes { get; set; }
    public List<string> Instructions { get; set; } = new();
    
    [MinLength(1)]
    public List<CreateRecipeIngredientRequest> Ingredients { get; set; } = new();
}

public sealed class CreateRecipeIngredientRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; set; }

    [Range(typeof(decimal), "0.0001", "1000000", ParseLimitsInInvariantCulture = true)]
    public decimal Quantity { get; set; }

    [EnumDataType(typeof(MeasurementUnit))]
    public MeasurementUnit Unit { get; set; }
}
