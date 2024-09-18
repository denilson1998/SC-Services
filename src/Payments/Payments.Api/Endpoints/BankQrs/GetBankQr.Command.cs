namespace Payments.Api.Endpoints.Payments;

public class GetBankQrCommand
{
    public int OrganizationId { get; set; }

    public int BankQrId { get; set; }
}
