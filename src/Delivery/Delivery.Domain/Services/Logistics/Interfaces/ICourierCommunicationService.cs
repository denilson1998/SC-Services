using System.Collections.Generic;
using System.Threading.Tasks;
using Delivery.Domain.Services.Contracts;

namespace Delivery.Domain.Interfaces.Services;
public interface ICourierCommunicationService
{
    Task<CreateExternalPickupAndDeliveryResponse> CreatePickupAndDeliveryAsync(CreateExternalPickupAndDeliveryCommand command);
    Task<List<ListCourierTasksResponse>> GetCourierTasksAsync(IEnumerable<string> jobIds);
    Task<List<ListCourierAgentsResponse>> GetCourierAgentsAsync();
}
