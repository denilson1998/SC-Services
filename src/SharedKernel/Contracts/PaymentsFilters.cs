namespace SharedKernel.Contracts;

public class ListBillsQueryFilters : ListQueryRequest
{
    public bool? IsCompleted { get; set; }
}