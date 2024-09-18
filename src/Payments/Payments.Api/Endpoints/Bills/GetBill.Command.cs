using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using SharedKernel.Contracts;

namespace Payments.Api.Endpoints.Bills;
public class GetBillCommand : ListQueryRequest
{
    [FromRoute]
    [Required]
    public int OrganizationId { get; set; }

    [FromRoute]
    [Required]
    public int BillId { get; set; }
}