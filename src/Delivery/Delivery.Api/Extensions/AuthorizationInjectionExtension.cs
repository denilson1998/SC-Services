using Delivery.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Delivery.Api.Extensions
{
    public static class AuthorizationInjectionExtension
    {
        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("Auth0")
                    .Build();

                options.AddPolicy("UserPolicy", policy =>
                {
                    policy.AuthenticationSchemes.Add("Auth0");
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new UserRequirement());
                });

                options.AddPolicy("UserWithoutParameters", policy =>
                {
                    policy.AuthenticationSchemes.Add("Auth0");
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new UserWithoutParameterRequirement());
                });

                options.AddPolicy("MachinePolicy", policy =>
                {
                    policy.AuthenticationSchemes.Add("Auth0Machine");
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new MachineRequirement());
                });
                options.AddPolicy("UserM2MPolicy", policy =>
                {
                    policy.AuthenticationSchemes.Add("Auth0");
                    policy.AuthenticationSchemes.Add("Auth0Machine");
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new UserM2MRequirement());
                });
            });

            services.AddScoped<IAuthorizationHandler, UserHandler>();
            services.AddScoped<IAuthorizationHandler, UserWithoutParametersHandler>();
            services.AddScoped<IAuthorizationHandler, MachineHandler>();
            services.AddScoped<IAuthorizationHandler, UserM2MHandler>();

            return services;
        }
    }
}