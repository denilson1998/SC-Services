using System;

namespace Delivery.Domain.Dtos;

public class LatLong
{
    public string Latitude { get; set; }
    public string Longitude { get; set; }

    public LatLong(string latitude, string longitude)
    {
        if (!double.TryParse(latitude, out double parsedLatitude))
        {
            throw new ArgumentException($"Latitude could not be parsed: {latitude}");
        }

        if (!double.TryParse(longitude, out double parsedLongitude))
        {
            throw new ArgumentException($"Longitude could not be parsed: {longitude}");
        }

        if (parsedLatitude > 90 || parsedLatitude < -90)
        {
            throw new ArgumentException($"Latitude must be between -90 and 90");
        }

        if (parsedLongitude > 180 || parsedLongitude < -180)
        {
            throw new ArgumentException($"Longitude must be between -180 and 180");
        }

        Latitude = latitude;
        Longitude = longitude;
    }
}