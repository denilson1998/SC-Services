using Payments.Domain.Entities;
using Payments.Domain.Interfaces.Services;
using Payments.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using SharedKernel.Contracts;
using SharedKernel.Events;
using Sitec.Delivery.EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Payments.Api.Endpoints.Webhooks;
using SharedKernel.Constants;
using Payments.Domain.Interfaces.Repositories;

namespace Payments.Api.Workers;

[DisallowConcurrentExecution]
public class SyncBankQrPayments : IJob
{
    private readonly ILogger<SyncBankQrPayments> _logger;
    private readonly HttpClient _httpClient;
    private readonly IWorkerRepository _workerRepository;
    private readonly IQRCommunicationService _qrService;
    private readonly IEventBus _eventBus;
    private readonly IPaymentsRepository _paymentRepository;
    public SyncBankQrPayments(
        ILogger<SyncBankQrPayments> logger,
        HttpClient httpClient,
        IWorkerRepository workerRepository,
        IQRCommunicationService qrService,
        IEventBus eventBus,
        IPaymentsRepository paymentRepository
        )
    {
        _logger = logger;
        _httpClient = httpClient;
        _workerRepository = workerRepository;
        _qrService = qrService;
        _eventBus = eventBus;
        _paymentRepository = paymentRepository;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        //crear tabla de workers y última corrida
        Worker workerFound = await GetWorker();

        var command = new ListBankQrPaymentsRequest
        {
            StartDate = workerFound.DataSyncedSince?.AddMinutes(-5),
            PerPage = int.MaxValue
        };

        var bankQrPayments = await _qrService.GetBankQrPayments(command);

        if (bankQrPayments.Data.Count > 0)
        {

            var qrPayments = await _paymentRepository.GetQrPaymentsAsync();

            var bankPayIds = qrPayments.Where(qr => !bankQrPayments.Data.ConvertAll(p => p.QrPayId).Contains(qr.BankPayId.ToString()));

            var payIds = bankPayIds.Select(c => c.BankPayId.ToString()).ToList();

            var newBankQrPayments = bankQrPayments.Data.Where(b => payIds.Contains(b.QrPayId)).ToList();

            // TODO usar un Task.WhenAll ?
            // TODO revisar si es mejor single events o batch events como usamos para tookan
            newBankQrPayments.ForEach(async b =>
            {
                var newEvent = new RegisterQrPaymentCommand()
                {
                    QrId = b.QrId,
                    PayId = Guid.Parse(b.QrPayId),
                    PaymentDate = b.PaymentDate,
                    AccountNumber = b.AccountNumber,
                    Amount = b.Amount,
                    BankOriginAccountNumber = b.AccountNumber,
                    BankOriginName = b.BankOriginName,
                    ClientName = b.ClientName,
                    Currency = b.Currency,
                    // Description = b.d
                };
                await _eventBus.PublishAsync(newEvent);
            }
            );
            workerFound.DataSyncedSince = DateTime.Now;

            await _paymentRepository.UpdateAsync();
        }
    }

    private async Task<Worker> GetWorker()
    {

        var workerFound = await _workerRepository.GetWorkerByName(nameof(SyncBankQrPayments));

        if (workerFound is null)
        {
            workerFound = new Worker()
            {
                Name = nameof(SyncBankQrPayments),
            };

            await _workerRepository.CreateWorkerAsync(workerFound);
        }

        return workerFound;
    }
}