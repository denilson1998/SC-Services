using Delivery.Domain.Services.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.Infrastructure.Services.Authenticaction
{
    public class Auth0AuthenticationOauthHandler : DelegatingHandler
    {
        private readonly IAuthenticationApi _authenticationApi;
        private readonly IMemoryCache _cache;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _audience;
        private const int CacheExpirationInMinutes = 55;

        public Auth0AuthenticationOauthHandler(IAuthenticationApi authenticationApi, IMemoryCache cache, IConfiguration configuration)
        {
            _authenticationApi = authenticationApi;
            _cache = cache;
            _clientId = configuration.GetSection("Auth0ApiConfig:ClientId").Value;
            _clientSecret = configuration.GetSection("Auth0ApiConfig:ClientSecret").Value;
            _audience = configuration.GetSection("Auth0ApiConfig:Audience").Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string token = "";
            string schemaToken = "Bearer";

            token = await _cache.GetOrCreateAsync("ScDeliveryToken", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationInMinutes);
                return GetToken();
            });
            request.Headers.Authorization = new AuthenticationHeaderValue(schemaToken, token);
            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string> GetToken()
        {
            var credentials = new CredentialRequest
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                Audience = _audience
            };
            var response = await _authenticationApi.ClientCredentials(credentials);
            if (response.Result is null)
                throw new HttpRequestException($"The request to obtain the token failed with these errors {response.Errors}");
            return response.Result.AccessToken; ;
        }
    }
}