using SharedKernel.Constants;

namespace SharedKernel.Extensions;

public static class DecimalExtensions
{
    public static bool IsNerlyEqualThan(this decimal source, decimal target)
    {
        var valOne = decimal.Round(source, DecimalPrecisionScale.AppPrecision);
        var valTwo = decimal.Round(target, DecimalPrecisionScale.AppPrecision);

        var tolerance = (decimal)(1 / Math.Pow(10, DecimalPrecisionScale.AppPrecision));
        var diff = Math.Abs(valOne - valTwo);
        return diff <= tolerance;
    }

    public static bool IsGreaterThan(this decimal source, decimal target)
    {
        var isNotNerlyEqual = !source.IsNerlyEqualThan(target);
        return source > target && isNotNerlyEqual;
    }
}
