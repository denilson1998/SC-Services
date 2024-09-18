using Microsoft.Extensions.DependencyInjection;

namespace Sitec.Delivery.Healtchecks;

public static class DaprHealthCheckBuilderExtensions
{
    public static IHealthChecksBuilder AddDapr(this IHealthChecksBuilder builder) =>
        builder.AddCheck<DaprHealthCheck>("dapr");
}
