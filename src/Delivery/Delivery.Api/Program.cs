using Delivery.Api.Extensions;
using Delivery.Infrastructure.Persistence;
using Delivery.Infrastructure.Services;
using Delivery.Domain.Interfaces.Services;
using Delivery.Infrastructure.Services.GoogleMaps;
using Delivery.Infrastructure.Services.QRService;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using Sitec.Delivery.EventBus;
using Sitec.Delivery.EventBus.Abstractions;
using Delivery.Infrastructure.Services.Logistics;
using Delivery.Infrastructure.Services.Tookan;
using SharedKernel.Interfaces;
using Delivery.Api.Identities;
using SharedKernel;
using Delivery.Domain.Interfaces.Repositories;
using Delivery.Infrastructure.Persistence.Repositories;
using Delivery.Domain.Services.Webhook;
using Delivery.Infrastructure.Services.WebHook;
using Microsoft.Extensions.Logging;
using Delivery.Infrastructure.Services.Authenticaction;
using Delivery.Domain.Services.Authentication;

// namespace Delivery.Api;
var development = "_development";
var production = "_production";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: development,
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    options.AddPolicy(name: production,
        builder => { builder.WithOrigins(); });
});

builder.Services.AddControllers()
    .AddDapr()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddDaprClient();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DeliveryDb"),
        b => b.MigrationsAssembly("Delivery.Infrastructure")));
// builder.Services.AddAutoMapper();
builder.Services.AddAutoMapper(typeof(Program));


builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<IGoogleMapsService, GoogleMapsService>();
builder.Services.AddTransient<IPricingRepository, PricingRepository>();
builder.Services.AddTransient<IFareRepository, FareRepository>();
builder.Services.AddTransient<ICourierTasksRepository, CourierTasksRepository>();
builder.Services.AddTransient<IWebhookRepository, WebhookRepository>();
builder.Services.AddTransient<IWebhooksComunicationService, WebhooksComunicationService>();
builder.Services.AddHttpClient<ICourierCommunicationService, TookanCourierService>();
builder.Services.AddCustomSwagger(builder.Configuration);
builder.Services.AddCustomAuthentication(builder.Configuration);
builder.Services.AddCustomAuthorization();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IEventBus, DaprEventBus>();
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
builder.Services.AddTransient<IAuthenticationApi, AuthenticationApi>();
builder.Services.AddTransient<Auth0AuthenticationOauthHandler>();
builder.Services.AddHttpClient<IPaymentsCommunicationService, PaymentsCommunicationService>()
    .AddHttpMessageHandler<Auth0AuthenticationOauthHandler>();

builder.Services.Configure<TookanCourierConfig>(builder.Configuration.GetSection("TookanCourierConfig"));
builder.Services.Configure<GoogleMapsConfig>(builder.Configuration.GetSection("GoogleMapsConfig"));
builder.Services.Configure<PaymentsServiceConfig>(builder.Configuration.GetSection("PaymentsServiceConfig"));
builder.Services.Configure<Auth0ApiConfig>(builder.Configuration.GetSection("Auth0ApiConfig"));

builder.Services.AddScoped<IMultiTenantService, MultiTenantService>();
builder.Services.AddLogging(builder => builder.AddConsole());

// builder.WebHost.UseUrls("http://0.0.0.0:80");
builder.Services.AddQuartzExtension(builder.Configuration);
var app = builder.Build();
if (true)
{
    app.UseCors(development);
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Delivery.Api v1");
        c.OAuthClientId("o2e9CppAtWld2dUv7tNDfvpcP7jtp3NE");
        c.OAuthClientSecret("N57SFugvzxvr9fs0XIrYRJvxyAKo-j5MFCkyRjt4hGpFecsfxmm487t4sNu6ahd3");
        c.OAuthAppName("ERP Swagger");
        c.OAuthUsePkce();
    });
}

// app.UseHttpsRedirection();

app.UseRouting();
app.UseCloudEvents();
// app.MapControllers();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapSubscribeHandler();
    endpoints.MapControllers();
});
app.Run();