using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Payments.Api.Endpoints.Bills;

public class RegisterBillPaymentCommand
{
    [JsonIgnore]
    [FromRoute]
    public int OrganizationId { get; set; }

    [JsonIgnore]
    [FromRoute]
    public int BillId { get; set; }

    [FromBody]
    public RegisterBillPaymentCommandBody RegisterBillPaymentCommandBody { get; set; }
}

public class RegisterBillPaymentCommandBody
{
    public int? VoucherId { get; set; }
}