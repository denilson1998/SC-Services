using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SharedKernel;
using SharedKernel.Constants;

namespace Payments.Api.Endpoints.Webhooks;

public record RegisterQrPaymentCommand : IntegrationEvent
{
    public string QrId { get; set; }
    public Guid PayId { get; set; }
    public string Voucher { get; set; }
    public string ClientName { get; set; }
    public string BankOriginName { get; set; }
    public string BankOriginAccountNumber { get; set; }
    public double Amount { get; set; }
    public Currency Currency { get; set; }
    public DateTime PaymentDate { get; set; }
    public string AccountNumber { get; set; }
    public string Description { get; set; }
    public string Metadata { get; set; }
}
