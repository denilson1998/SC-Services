using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using SharedKernel.Contracts;

namespace Payments.Api.Endpoints.Bills;
public class ListBillsCommand : ListBillsQueryFilters
{
    [FromRoute]
    [Required]
    public int OrganizationId { get; set; }
}
