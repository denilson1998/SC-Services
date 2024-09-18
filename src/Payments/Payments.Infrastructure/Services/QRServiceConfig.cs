using SharedKernel;
namespace Payments.Infrastructure.Services;

public class QRServiceConfig
{
    public string BaseUrl { get; set; }
    public string ClientId { get; set; }
    public string Scope { get; set; }
}
