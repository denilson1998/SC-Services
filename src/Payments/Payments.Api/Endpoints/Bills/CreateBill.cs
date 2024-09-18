using Ardalis.ApiEndpoints;
using Ardalis.RouteAndBodyModelBinding;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Payments.Domain.Entities;
using Payments.Domain.Interfaces.Repositories;
using Payments.Domain.Interfaces.Services;
using Payments.Infrastructure.Persistence;
using SharedKernel.Constants;
using SharedKernel.Events;
using Sitec.Delivery.EventBus.Abstractions;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Payments.Api.Endpoints.Bills;
public class CreateBill : EndpointBaseAsync
    .WithRequest<CreateBillCommand>
    .WithActionResult<CreateBillResult>
{
    private readonly IBankConfigRepository _bankConfigRepository;
    private readonly IQRCommunicationService _qrCommunicationService;
    private readonly IBillRepository _billRepository;
    private readonly IVoucherRepository _voucherRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly IEventBus _eventBus;

    public CreateBill(IBankConfigRepository bankConfigRepository,
        IQRCommunicationService qrCommunicationService,
        IBillRepository billRepository,
        IVoucherRepository voucherRepository,
        IMapper mapper,
        ILogger<CreateBill> logger,
        IEventBus eventBus)
    {
        _bankConfigRepository = bankConfigRepository;
        _qrCommunicationService = qrCommunicationService;
        _billRepository = billRepository;
        _voucherRepository = voucherRepository;
        _mapper = mapper;
        _logger = logger;
        _eventBus = eventBus;
    }
    // TODO versionar
    [HttpPost("/organizations/{OrganizationId}/bills")]
    [Authorize(Policy = "UserM2MPolicy")]
    [SwaggerOperation(
        Summary = "Create a bill",
        Description = "Create a bill",
        OperationId = "Bills.Batch",
        Tags = new[] { "BillsEndpoint" })
    ]
    public override async Task<ActionResult<CreateBillResult>> HandleAsync([FromRoute] CreateBillCommand request, CancellationToken cancellationToken = default)
    {
        var bill = new Bill(
            request.OrganizationId,
            request.CreateBilllBody.Amount,
            request.CreateBilllBody.Currency
        );

        var bankConfig = await _bankConfigRepository.GetBankConfigAsync(cancellationToken);

        if (request.CreateBilllBody.VoucherId is not null)
        {
            var voucherFound = await _voucherRepository.GetAsync(request.CreateBilllBody.VoucherId, cancellationToken);
            voucherFound.RegisterVoucherUse();
            bill.RegisterPayment(voucherFound);
        }

        if (request.CreateBilllBody.CreateBankQr && !bill.IsCompleted)
        {
            var transferQrDto = new TransferQrDto
            {
                AccountNumber = bankConfig.BankAccountNumber,
                Amount = bill.RemainingAmount,
                ClientCode = bankConfig.ClientId,
                Currency = request.CreateBilllBody.Currency,
                Description = "SC Services Delivery",
                ExpirationDate = DateTime.UtcNow.AddMonths(1),
                SingleUse = true,
                SystemModules = bankConfig.BankAccountType
            };
              var generatedQrString = await _qrCommunicationService.GenerateQrStringAsync(transferQrDto);
            if (generatedQrString is null) {
                return null;
            }
            var newBankQr = new BankQr()
            {
                ClientId = bankConfig.ClientId,
                QrId = generatedQrString.Id,
                BankAccountNumber = bankConfig.BankAccountNumber,
                Currency = request.CreateBilllBody.Currency,
                Description = "SC Services Delivery",
                BankAccountType = BankAccountType.SavingAccount,
                Amount = bill.RemainingAmount,
                ExpirationDate = DateTime.UtcNow.AddDays(1),
                EncryptedQrString = generatedQrString.Encrypt,
                IsPaid = false,
                ClientName = generatedQrString.AccountHolder,
            };
            bill.BankQrs = new List<BankQr>
            {
                newBankQr
            };
        }
        var billResult = await _billRepository.CreateAsync(bill);
        if (bill.IsCompleted)
        {
            //TODO deberíamos pasarlo a un domainevent y un dispatcher?
            await _eventBus.PublishAsync(
                new BillPaidEvent(
                    bill.OrganizationId,
                    bill.Id
                )
            );
        }
        var result = _mapper.Map<CreateBillResult>(billResult);

        return Created("", result);
    }
}
