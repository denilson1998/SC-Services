using Delivery.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using SharedKernel.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Delivery.Api.Authorization
{
    public class UserM2MHandler : AuthorizationHandler<UserM2MRequirement>
    {
        private readonly IMultiTenantService _multiTenantService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _auth0M2MBaseUrl;

        public UserM2MHandler(IMultiTenantService multiTenantService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _multiTenantService = multiTenantService;
            _httpContextAccessor = httpContextAccessor;
            _auth0M2MBaseUrl = configuration.GetSection("Auth0ApiConfig:BaseUrl").Value;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            UserM2MRequirement requirement)
        {
            var routeData = _httpContextAccessor.HttpContext!.GetRouteData();
            var issuer = _httpContextAccessor!.HttpContext!.User.Claims.FirstOrDefault((c) => c.Type == "iss")?.Value
                .Replace("https://", "").Replace("/", "");
            var issM2M = _auth0M2MBaseUrl.Replace("https://", "").Replace("/", "");
            var success = int.TryParse(routeData.Values["organizationId"].ToString(), out int organizationId);
            if (!success)
            {
                await Task.CompletedTask;
                return;
            }

            if (issuer == issM2M)
            {
                _multiTenantService.OverrideOrganizationId(organizationId);
                context.Succeed(requirement);
                await Task.CompletedTask;
                return;
            }

            var claim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimConstants.ClientId);
            if (claim is null)
            {
                await Task.CompletedTask;
                return;
            }

            var clientId = Convert.ToInt32(claim.Value);
            _multiTenantService.OverrideOrganizationId(organizationId);

            context.Succeed(requirement);
            await Task.CompletedTask;
        }
    }

    public class UserM2MRequirement : IAuthorizationRequirement
    {
    }
}