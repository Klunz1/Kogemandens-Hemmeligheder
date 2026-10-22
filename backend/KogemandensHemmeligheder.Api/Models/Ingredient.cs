using KogemandensHemmeligheder.Api.Enums;

namespace KogemandensHemmeligheder.Api.Models;
public class Ingredient
{
    public required string Name { get; set; }
    public MeasurementUnit Unit { get; set; }
    public decimal Quantity { get; set; }
}