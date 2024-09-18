using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace Payments.Api.Extensions
{
    public static class SwaggerInjectionExtensions
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Payments.Api", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(configuration.GetSection("IdentityProvider:BaseUrl").Value + "/authorize?audience=" + configuration.GetSection("IdentityProvider:Audience").Value),

                            TokenUrl = new Uri($"{configuration.GetSection("IdentityProvider:BaseUrl").Value}/oauth/token"),
                            //TODO agregar scopes
                            // Scopes = new Dictionary<string, string>
                            // {
                            //     {"atlas", "Atlas API"}
                            // }
                        }
                    }
                });
                c.EnableAnnotations();
                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });
            return services;
        }
    }
}