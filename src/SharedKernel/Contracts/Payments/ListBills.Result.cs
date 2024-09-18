using SharedKernel.Constants;

namespace SharedKernel.Contracts.Payments;
public class ListBillsResult
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public Currency Currency { get; set; }
    public bool IsCompleted { get; set; }
}
