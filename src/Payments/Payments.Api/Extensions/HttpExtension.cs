using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SharedKernel.DataFilters.Parameters;

namespace Payments.Api.Extensions
{
    public static class HttpExtension
    {
        public static GeneralParameters ExtractPagingParams(this HttpRequest request)
        {
            // Get the header Data Options
            if (request is not null && request.Headers.TryGetValue("DATA-OPTIONS", out var options))
            {
                // Converting from JSON to GeneralParameters
                var parameters = new GeneralParameters();
                JsonConvert.PopulateObject(options, parameters);
                return parameters;
            }
            return null;
        }
    }
}
