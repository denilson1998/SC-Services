using System.Collections.Generic;

namespace Delivery.Infrastructure.Services.GoogleMaps;

public class GoogleMapsDistanceMatrixResponse
{
    public IEnumerable<string> destination_addresses { get; set; }
    public IEnumerable<string> origin_addresses { get; set; }
    public IEnumerable<GoogleMapsDistanceMatrixResponseRow> rows { get; set; }
}

public class GoogleMapsDistanceMatrixResponseRow
{
    public IEnumerable<GoogleMapsDistanceMatrixResponseElement> elements { get; set; }
}

public class GoogleMapsDistanceMatrixResponseElement
{
    public GoogleMapsResponseTextValue distance { get; set; }
    public GoogleMapsResponseTextValue duration { get; set; }
    public string status { get; set; }
}

public class GoogleMapsResponseTextValue
{
    public string text { get; set; }
    public int value { get; set; }
}
