using SharedKernel.DataFilters.Pagination.Wrappers;
namespace SharedKernel.Contracts;

public class PagedResponse<T> 
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public List<T> Data { get; set; }
    public PagedResponse(List<T> data, int skippedRecordsCount, int pageSize, int totalRecords)
    {
        var totalPages = (pageSize != 0) ? (totalRecords / (double)pageSize) : 0;
        var roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

        PageNumber = Convert.ToInt32(Math.Ceiling(skippedRecordsCount / (double)pageSize));
        PageSize = pageSize;
        Data = data;
        TotalPages = roundedTotalPages;
        TotalRecords = totalRecords;
    }
}