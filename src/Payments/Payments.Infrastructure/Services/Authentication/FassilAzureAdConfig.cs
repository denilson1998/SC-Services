using SharedKernel;

namespace Payments.Infrastructure.Services.Authentication
{
    public class FassilAzureAdConfig : IdentityProviderConfig
    {
        new public IdentityProvider IdentityProvider { get; set; } = IdentityProvider.FassilAzureAd;
    }
}
