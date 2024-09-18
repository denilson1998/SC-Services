namespace SharedKernel.Constants
{
    public enum Filter
    {
        Equal = 0,
        NotEqual = 1,
        Contains = 2,
        LessThan = 3,
        GreaterThan = 4,
        LessThanOrEqual = 5,
        GreaterThanOrEqual = 6
    }

    public static class Concatenator
    {
        public static readonly string And = "AND";
        public static readonly string Or = "OR";
    }

    public static class Operation
    {
        public const string Equal = "Equal";
        public const string NotEqual = "NotEqual";
        public const string Contains = "Contains";
        public const string LessThan = "LessThan";
        public const string GreaterThan = "GreaterThan";
        public const string LessThanOrEqual = "LessThanOrEqual";
        public const string GreaterThanOrEqual = "GreaterThanOrEqual";
    }
}