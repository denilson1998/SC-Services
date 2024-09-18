using Microsoft.AspNetCore.Mvc;
using SharedKernel.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Payments.Api.Endpoints.Vouchers;

public class CreateVoucherCommand
{
    [JsonIgnore]
    [Required]
    [FromRoute]
    public int OrganizationId { get; set; }

    [FromBody]
    public CreateVoucherCommandBody CreateVoucherBody { get; set; }
}

public class CreateVoucherCommandBody
{
    [Required]
    public decimal Value { get; set; }

    public Currency Currency { get; set; }

    public bool IsPercentage { get; set; }

    public int Quantity { get; set; } = 1;

    public DateTime ValidSince { get; set; }

    public DateTime ValidUntil { get; set; }
}