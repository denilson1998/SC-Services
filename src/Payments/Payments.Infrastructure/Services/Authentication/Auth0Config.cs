using SharedKernel;

namespace Payments.Infrastructure.Services.Authentication
{
    public class Auth0Config : IdentityProviderConfig
    {
        new public IdentityProvider IdentityProvider { get; set; } = IdentityProvider.PetoAuth0;
    }
}
