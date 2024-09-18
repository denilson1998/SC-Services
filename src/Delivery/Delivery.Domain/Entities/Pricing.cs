using System;
using SharedKernel.AbstractEntities;

namespace Delivery.Domain.Entities;

public class Pricing : AuditableEntity
{
    public string Alias { get; set; }
    public decimal? PricePerEstimatedKilometer { get; protected set; }
    public decimal? PricePerEstimatedMinute { get; protected set; }
    public decimal MinimumPrice { get; protected set; }
    public bool IsActive { get; set; }

    private Pricing()
    {
    }

    public Pricing(
        string alias,
        decimal? pricePerEstimatedKilometer,
        decimal? pricePerEstimatedMinute,
        decimal minimumPrice
    )
    {
        if (minimumPrice <= 0)
        {
            throw new ArgumentException("Minimum price must be greater than 0");
        }

        if (pricePerEstimatedKilometer is null && pricePerEstimatedMinute is null)
        {
            throw new ArgumentException(
                "Price per estimated kilometer and price per estimated minute cannot be null at the same time");
        }

        Alias = alias;
        PricePerEstimatedKilometer = pricePerEstimatedKilometer;
        PricePerEstimatedMinute = pricePerEstimatedMinute;
        MinimumPrice = minimumPrice;
        IsActive = false;
    }

    public decimal CalculatePrice(
        int estimatedDistance,
        int estimatedDuration
    )
    {
        var price = 0m;

        if (PricePerEstimatedKilometer.HasValue)
        {
            price += PricePerEstimatedKilometer.Value * estimatedDistance;
        }

        if (PricePerEstimatedMinute.HasValue)
        {
            price += PricePerEstimatedMinute.Value * estimatedDuration;
        }

        return price < MinimumPrice ? MinimumPrice : price;
    }
}