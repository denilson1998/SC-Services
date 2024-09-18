using Delivery.Domain.Dtos;
using System;
using System.Threading.Tasks;

namespace Delivery.Infrastructure.Services.GoogleMaps;
public interface IGoogleMapsService
{
    public Task<DistanceAndDuration> GetDistanceAndDurationAsync(LatLong origin, LatLong destination, DateTime? DepartureTime);
    public Task<DistanceAndDuration> GetDistanceAndDurationAsync(LatLong origin, LatLong destination);

}