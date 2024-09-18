using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Payments.Infrastructure.Services;
using Payments.Infrastructure.Services.Authentication;
using SharedKernel;

namespace Payments.Infrastructure.Authentication;
public class TokenAuthenticationHandler : DelegatingHandler
{
    private readonly IMemoryCache _cache;
    private readonly Dictionary<IdentityProvider, IdentityProviderConfig> _identityProviders;
    private readonly List<ApiWithIdentityProvider> _apps;

    public TokenAuthenticationHandler(IMemoryCache memoryCache,
        IOptions<FassilAzureAdConfig> fassilAzureAdConfig,
        IOptions<QRServiceConfig> qrServiceConfig
        )
    {
        _cache = memoryCache;
        _identityProviders = new Dictionary<IdentityProvider, IdentityProviderConfig>()
        {
            { IdentityProvider.FassilAzureAd, fassilAzureAdConfig.Value },
        };

        _apps = new List<ApiWithIdentityProvider>()
        {
            new ApiWithIdentityProvider()
                {
                    BaseUrl = qrServiceConfig.Value.BaseUrl,
                    IdentityProvider = IdentityProvider.FassilAzureAd,
                    Scopes = new List<string>() { qrServiceConfig.Value.Scope }
                },
        };
    }

    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestBaseUrl = request.RequestUri.ToString().Replace(request.RequestUri.AbsolutePath, string.Empty);

        var app = _apps.Find(app => app.BaseUrl.Contains(requestBaseUrl));
        if (app == null)
        {
            throw new Exception($"App not found, baseUrl: {requestBaseUrl}");
        }

        var identityProviderConfig = _identityProviders.FirstOrDefault(i => i.Key == app.IdentityProvider).Value;

        if (identityProviderConfig == null)
        {
            throw new Exception($"Identity provider config not found, {requestBaseUrl}");
        }

        var token = string.Empty;
        if (identityProviderConfig.IdentityProvider is IdentityProvider.FassilAzureAd)
        {
            token = await GetAzureAccessToken(identityProviderConfig, app.Scopes);
        }

        if (identityProviderConfig.IdentityProvider is IdentityProvider.PetoAuth0)
        {
            token = await GetAuth0AccessToken(identityProviderConfig);
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }

    public async Task<string> GetAzureAccessToken(IdentityProviderConfig config, IEnumerable<string> scopes)
    {
        var cacheKey = $"{config.BaseUrl}-access-token";
        if (_cache.TryGetValue(cacheKey, out string accessToken))
        {
            return accessToken;
        }
        var identityProviderClient = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                            .WithClientSecret(config.Secret)
                            .WithAuthority(config.BaseUrl, config.TenantId)
                            .Build();

        var authResult = await identityProviderClient.AcquireTokenForClient(scopes: scopes).ExecuteAsync();
        var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(55));
        _cache.Set(cacheKey, authResult?.AccessToken, cacheEntryOptions);
        return authResult?.AccessToken;
    }

    public static async Task<string> GetAuth0AccessToken(IdentityProviderConfig config)
    {
        var client = new HttpClient();
        var dict = new
        {
            grant_type = "client_credentials",
            client_id = config.ClientId,
            client_secret = config.Secret,
            audience = config.Audience,
        };
        var json = JsonConvert.SerializeObject(dict);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"https://{config.BaseUrl}/oauth/token", httpContent);
        var responseStringContent = response.Content.ReadAsStringAsync().Result;
        var responseContent = JsonConvert.DeserializeObject<Auth0ApiManagementTokenRequestResponse>(responseStringContent);
        return responseContent.access_token;
    }
}

public class ApiWithIdentityProvider
{
    public string BaseUrl { get; set; }
    public List<string> Scopes { get; set; }
    public IdentityProvider IdentityProvider { get; set; }
}
public class Auth0ApiManagementTokenRequestResponse
{
    public string access_token { get; set; }
}
