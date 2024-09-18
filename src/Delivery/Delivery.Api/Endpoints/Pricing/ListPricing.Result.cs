using System;

namespace Delivery.Api.Endpoints.Pricing;
public class ListPricingResult
{
    public int Id { get; set; }
    public string Alias { get; set; }
    public decimal? PricePerEstimatedKilometer { get; set; }
    public decimal? PricePerEstimatedMinute { get; set; }
    public decimal MinimumPrice { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; }
}
