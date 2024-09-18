using Delivery.Domain.Services.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SharedKernel.DataFilters.Pagination.Wrappers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Delivery.Infrastructure.Services.Authenticaction
{
    public class AuthenticationApi : IAuthenticationApi
    {
        private readonly HttpClient _httpClient;

        public AuthenticationApi(HttpClient httpClient, IOptions<Auth0ApiConfig> Auth0ApiConfig)
        {
            httpClient.BaseAddress = new Uri(Auth0ApiConfig.Value.BaseUrl);
            _httpClient = httpClient;
        }

        public async Task<Response<CredentialResponse>> ClientCredentials(CredentialRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var requestDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "oauth/token")
            {
                Content = new FormUrlEncodedContent(requestDictionary)
            };

            var requestResult = await _httpClient.SendAsync(httpRequestMessage);
            var contentString = await requestResult.Content.ReadAsStringAsync();
            var response = new Response<CredentialResponse>(null);
            if (requestResult.StatusCode == HttpStatusCode.OK)
            {
                response.Result = JsonConvert.DeserializeObject<CredentialResponse>(contentString);
            }
            else
            {
                response.Errors = new string[] { requestResult.StatusCode.ToString(), contentString };
            }
            return response;
        }
    }
}