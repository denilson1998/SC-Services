namespace SharedKernel.DataFilters.Parameters;

public class GeneralParameters
{
    public bool WithDeleted { get; set; }

    public PaginationParameters PaginationOptions { get; set; }

    public List<SortParameters> SortOptions { get; set; }

    public List<FilterParameters> FilterOptions { get; set; }
}
