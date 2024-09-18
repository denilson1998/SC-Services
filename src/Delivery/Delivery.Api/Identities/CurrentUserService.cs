using Delivery.Domain.Common;
using Microsoft.AspNetCore.Http;
using SharedKernel.Interfaces;
using System.Security.Claims;

namespace Delivery.Api.Identities;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public int ClientId => int.Parse(_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimConstants.ClientId)!);
}