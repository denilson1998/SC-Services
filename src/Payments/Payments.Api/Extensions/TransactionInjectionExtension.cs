using Microsoft.Extensions.DependencyInjection;
using Payments.Api.Authorization;
using Payments.Domain.Interfaces.Repositories;
using Payments.Domain.Interfaces.Services;
using Payments.Infrastructure;
using Payments.Infrastructure.Authentication;
using Payments.Infrastructure.Persistence.Repositories;
using Payments.Infrastructure.Services.QRService;
using SharedKernel.DataFilters;

namespace Payments.Api.Extensions;
public static class TransactionInjectionExtension
{
    public static IServiceCollection AddTransactionDependencies(this IServiceCollection services)
    {
        services.AddTransient<IQRCommunicationService, QRCommunicationService>();
        services.AddTransient<IBankConfigRepository, BankConfigRepository>();
        services.AddTransient<Auth0AuthenticationHandler>();
        services.AddTransient<TokenAuthenticationHandler>();
        services.AddHttpClient<IQRCommunicationService, QRCommunicationService>().AddHttpMessageHandler<TokenAuthenticationHandler>();
        return services;
    }
}
