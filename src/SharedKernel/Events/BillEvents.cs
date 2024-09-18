using System.ComponentModel.DataAnnotations;

namespace SharedKernel.Events;
public record BillPaidEvent : IntegrationEvent
{
    [Required]
    public int OrganizationId { get; set; }
    [Required]
    public int BillId { get; set; }
    public BillPaidEvent(
        int organizationId,
        int billId
    ) {
        OrganizationId = organizationId;
        BillId = billId;
    }
}
