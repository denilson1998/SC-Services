using Delivery.Domain.Dtos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Delivery.Infrastructure.Services.GoogleMaps
{
    public class GoogleMapsService : IGoogleMapsService
    {
        private readonly HttpClient _httpClient;
        private readonly GoogleMapsConfig _googleMapsConfig;

        public GoogleMapsService(HttpClient httpClient, IOptions<GoogleMapsConfig> googleMapsApiKey)
        {
            _httpClient = httpClient;
            httpClient.BaseAddress = new Uri(googleMapsApiKey.Value.BaseUrl);
            _googleMapsConfig = googleMapsApiKey.Value;
        }

        public async Task<DistanceAndDuration> GetDistanceAndDurationAsync(LatLong origin, LatLong destination, DateTime? DepartureTime)
        {
            var query = $"?key={_googleMapsConfig.ApiKey}";
            query += $"&origins={origin.Latitude},{origin.Longitude}";
            query += $"&destinations={destination.Latitude},{destination.Longitude}";
            var response = await _httpClient.GetAsync($"maps/api/distancematrix/json{query}");
            if (response.StatusCode is not HttpStatusCode.OK)
            {
                return null;
            }
            var responseStringContent = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<GoogleMapsDistanceMatrixResponse>(responseStringContent);
            var firstRow = result.rows.ToList()[0].elements.ToList()[0];
            return new DistanceAndDuration()
            {
                Distance = firstRow.distance is not null ? firstRow.distance.value : null,
                Duration = firstRow.duration is not null ? firstRow.duration.value : null
            };
        }

        public async Task<DistanceAndDuration> GetDistanceAndDurationAsync(LatLong origin, LatLong destination)
        {
            return await GetDistanceAndDurationAsync(origin, destination, DateTime.Now);
        }

    }
}