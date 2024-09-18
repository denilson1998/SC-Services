namespace Payments.Domain.Interfaces.Services;
public class TransferQrResponse
{
    public string Id { get; set; }
    public string Encrypt { get; set; } = string.Empty;
    public string AccountHolder { get; set; } = string.Empty;
}
