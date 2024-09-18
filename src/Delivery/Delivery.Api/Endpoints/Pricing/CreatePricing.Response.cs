namespace Delivery.Api.Endpoints.Pricing;

public class CreatePricingResponse
{
    public int Id { get; set; }
    public string Alias { get; set; }
    public decimal? PricePerEstimatedKilometer { get; set; }
    public decimal? PricePerEstimatedMinute { get; set; }
    public decimal MinimumPrice { get; set; }
    public bool IsActive { get; set; }
}