using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;

namespace Sitec.Delivery.Healtchecks;
public static class HealthCheckEndpointRouteBuilderExtensions
{
    public static void MapCustomHealthChecks(
        this WebApplication app,
        string healthPattern = "/hc",
        string livenessPattern = "/liveness",
        Func<HttpContext, HealthReport, Task> responseWriter = default)
    {
        app.MapHealthChecks(healthPattern, new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = responseWriter,
        });
        app.MapHealthChecks(livenessPattern, new HealthCheckOptions
        {
            Predicate = r => r.Name.Contains("self")
        });
    }
}
