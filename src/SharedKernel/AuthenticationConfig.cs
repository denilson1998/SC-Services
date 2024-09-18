namespace SharedKernel;
public class IdentityProviderConfig
{
    public string ClientId { get; set; }
    public string BaseUrl { get; set; }
    public string Audience { get; set; }
    public string Secret { get; set; }
    public string TenantId { get; set; }
    public IdentityProvider IdentityProvider { get; set; }
}

public enum IdentityProvider
{
    FassilAzureAd,
    PetoAuth0
}
