using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// using Payments.Api.Authentication;

namespace Payments.Api.Extensions
{
    public static class AuthenticationInjectionExtension
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication()
             .AddJwtBearer("Auth0", options =>
             {
                 options.Authority = configuration.GetSection("IdentityProvider:BaseUrl").Value;
                 options.Audience = configuration.GetSection("IdentityProvider:Audience").Value;
                 options.TokenValidationParameters.ValidateAudience = true;
             })
             .AddJwtBearer("Auth0Machine", options =>
             {
                 options.Authority = configuration.GetSection("Auth0ApiConfig:BaseUrl").Value;
                 options.Audience = configuration.GetSection("Auth0ApiConfig:Audience").Value;
                 options.TokenValidationParameters.ValidateAudience = true;
             });

            return services;
        }
    }
}