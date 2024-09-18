using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Payments.Api.Authorization
{
    public class Auth0AuthenticationHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _auth0BaseUrl;

        public Auth0AuthenticationHandler(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _auth0BaseUrl = configuration.GetSection("Auth0:BaseUrl").Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string token = "";
            string schemaToken = "Bearer";
            var issuer = _httpContextAccessor!.HttpContext!.User.Claims.FirstOrDefault((c) => c.Type == "iss")?.Value.Replace("https://", "").Replace("/", "");
            string tokenFromHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (issuer == _auth0BaseUrl)
            {
                token = tokenFromHeader.Split(" ").LastOrDefault();
                schemaToken = tokenFromHeader.Split(" ").FirstOrDefault();
            }
            request.Headers.Authorization = new AuthenticationHeaderValue(schemaToken, token);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}