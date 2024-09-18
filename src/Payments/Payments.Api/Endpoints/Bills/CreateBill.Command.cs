using Microsoft.AspNetCore.Mvc;
using SharedKernel.Constants;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Payments.Api.Endpoints.Bills;

public class CreateBillCommand
{
    [JsonIgnore]
    [Required]
    [FromRoute]
    public int OrganizationId { get; set; }

    [FromBody]
    public CreateBillCommandBody CreateBilllBody { get; set; }
}

public class CreateBillCommandBody
{
    [Required]
    public decimal Amount { get; set; }

    [Required]
    public Currency Currency { get; set; }

    public bool CreateBankQr { get; set; } = true;
    public int? VoucherId { get; set; }
}