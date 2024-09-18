namespace SharedKernel.Interfaces;

public interface IEventDispatcher
{
    public Task DispatchAsync(IEnumerable<IDomainEvent> events);
    public Task DispatchAsync(IDomainEvent ev);

}