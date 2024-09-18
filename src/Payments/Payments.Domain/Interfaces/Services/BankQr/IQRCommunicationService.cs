using Payments.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Payments.Domain.Interfaces.Services;
public interface IQRCommunicationService
{
    public Task<TransferQrResponse> GenerateQrStringAsync(TransferQrDto bankQr, int timeout = 100);
    public Task<ListBankQrPaymentsPagedResult> GetBankQrPayments(ListBankQrPaymentsRequest request, int timeout = 100);
}
