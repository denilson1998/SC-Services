using Google.Api;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Payments.Api.Extensions;
using Payments.Api.Identities;
using Payments.Domain.Interfaces.Repositories;
using Payments.Infrastructure.Persistence;
using Payments.Infrastructure.Persistence.Repositories;
using Payments.Infrastructure.Services;
using Payments.Infrastructure.Services.Authentication;
using SharedKernel;
using SharedKernel.DataFilters;
using SharedKernel.Interfaces;
using Sitec.Delivery.EventBus;
using Sitec.Delivery.EventBus.Abstractions;
using System.Text.Json.Serialization;

var development = "_development";
var production = "_production";

var builder = WebApplication.CreateBuilder(args);

// This method gets called by the runtime. Use this method to add services to the container.
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
                        builder => builder.WithOrigins());
});

builder.Services.AddControllers()
    .AddDapr()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddDaprClient();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, _) => module.EnableSqlCommandTextInstrumentation = true);
builder.Services.AddLogging(builder => builder.AddConsole());
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PaymentsDb"),
        b => b.MigrationsAssembly("Payments.Infrastructure")));
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IMultiTenantService, MultiTenantService>();
builder.Services.AddTransient<IVoucherRepository, VoucherRepository>();
builder.Services.AddTransient<IBillRepository, BillReporsitoy>();
builder.Services.AddTransient<IBankQrsRepository, BanckQrsRepository>();
builder.Services.AddTransient<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddScoped<IEventBus, DaprEventBus>();
builder.Services.AddTransactionDependencies();
builder.Services.AddCustomSwagger(builder.Configuration);
builder.Services.AddCustomAuthentication(builder.Configuration);
builder.Services.AddCustomAuthorization();
builder.Services.AddMemoryCache();
builder.Services.Configure<ConnectionStringConfig>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<Auth0Config>(builder.Configuration.GetSection("Auth0"));
builder.Services.Configure<QRServiceConfig>(builder.Configuration.GetSection("QRServiceConfig"));
builder.Services.Configure<FassilAzureAdConfig>(builder.Configuration.GetSection("FassilAzureAd"));
builder.WebHost.UseUrls("http://0.0.0.0:80");

builder.Services.AddHealthChecks()
    .AddSqlServer(
    connectionString: builder.Configuration.GetConnectionString("PaymentsDb"),
    name: "PaymentsDb",
    failureStatus: HealthStatus.Degraded);

builder.Services.AddQuartzExtension(builder.Configuration);

var app = builder.Build();
if (builder.Environment.IsDevelopment())
{
    app.UseCors(development);
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payments.Api v1");
        c.OAuthClientId(builder.Configuration.GetSection("Auth0:SwaggerClientId").Value);
        c.OAuthClientSecret(builder.Configuration.GetSection("Auth0:ClientSecret").Value);
        c.OAuthAppName(builder.Configuration.GetSection("Auth0:SwaggerAppName").Value);
        c.OAuthUsePkce();
    });
}
else
{
    app.UseCors(production);
}
//app.UseHttpsRedirection();

app.UseRouting();
app.UseCloudEvents();

app.UseAuthentication();

app.UseAuthorization();

// app.ConfigureEventBus();

app.UseEndpoints(endpoints =>
{
    endpoints.MapSubscribeHandler();
    endpoints.MapControllers();
});
app.Run();
