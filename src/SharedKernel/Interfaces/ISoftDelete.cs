namespace SharedKernel.Interfaces
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTime? DeletionDateTime { get; set; }
    }
}