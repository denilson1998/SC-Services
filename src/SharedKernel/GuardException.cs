namespace SharedKernel;

public class GuardException : ArgumentException
{
    private readonly string _message;
    public GuardException(string message, string paramName)
        : base(message, paramName)
    {
        _message = message;
    }
    public override string Message
    {
        get
        {
            return _message;
        }
    }
}
