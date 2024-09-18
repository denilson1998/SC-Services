using SharedKernel.Constants;

namespace SharedKernel.DataFilters.Parameters;

public class FilterParameters
{
    public string Field { get; set; } = null!;
    public string Value { get; set; } = null!;
    public Filter? Operation { get; set; }
    public String Concatenator { get; set; } = Constants.Concatenator.And;
    public List<FilterParameters> Children { get; set; }
}
