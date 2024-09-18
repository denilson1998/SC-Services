using Delivery.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Delivery.Api.Authorization
{
    public class UserWithoutParametersHandler : AuthorizationHandler<UserWithoutParameterRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            UserWithoutParameterRequirement requirement)
        {
            var claim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimConstants.ClientId);
            if (claim is null)
            {
                await Task.CompletedTask;
                return;
            }

            var clientId = Convert.ToInt32(claim.Value);

            context.Succeed(requirement);
            await Task.CompletedTask;
        }
    }

    public class UserWithoutParameterRequirement : IAuthorizationRequirement
    {
    }
}