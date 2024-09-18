using System;
using System.Threading.Tasks;
using SharedKernel.Contracts;
using SharedKernel.Contracts.Payments;
namespace Delivery.Domain.Interfaces.Services;
public interface IPaymentsCommunicationService
{
    public Task<CreateBillResponse> CreateBillAsync(CreateBillDto command, int timeout = 100);
    public Task<PagedResponse<ListBillsResult>> GetBillsAsync(ListBillsQueryFilters command, int timeout = 100);
}
