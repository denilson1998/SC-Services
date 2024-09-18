namespace SharedKernel.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        int ClientId { get; }
    }
}
