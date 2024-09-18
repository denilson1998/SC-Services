namespace SharedKernel.Contracts;

public class ListQueryRequest
{
    public int Limit { get; set; } = 100;
    public int Skip { get; set; } = 0;
    public DateTime? Since { get; set; }
    public DateTime? Before { get; set; }
}
