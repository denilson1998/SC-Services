using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Delivery.Api.Authorization
{
    public class MachineHandler : AuthorizationHandler<MachineRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _auth0M2MBaseUrl;

        public MachineHandler(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _auth0M2MBaseUrl = configuration.GetSection("Auth0ApiConfig:BaseUrl").Value;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            MachineRequirement requirement)
        {
            var routeData = _httpContextAccessor.HttpContext!.GetRouteData();
            var issuer = _httpContextAccessor!.HttpContext!.User.Claims.FirstOrDefault((c) => c.Type == "iss")?.Value
                .Replace("https://", "").Replace("/", "");
            var issM2M = _auth0M2MBaseUrl.Replace("https://", "").Replace("/", "");
            
            if (issuer != issM2M)
            {
                await Task.CompletedTask;
                return;
            }

            context.Succeed(requirement);
            await Task.CompletedTask;
        }
    }

    public class MachineRequirement : IAuthorizationRequirement
    {
    }
}