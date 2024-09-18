  using Ardalis.ApiEndpoints;
using AutoMapper;
using Dapr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.Domain.Entities;
using Payments.Domain.Interfaces.Repositories;
using SharedKernel;
using SharedKernel.Events;
using Sitec.Delivery.EventBus.Abstractions;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Payments.Api.Endpoints.Webhooks;

public class RegisterQrPayment : EndpointBaseAsync
    .WithRequest<RegisterQrPaymentCommand>
    .WithActionResult
{
    private readonly IMapper _mapper;
    private readonly IBankQrsRepository _bankQrsRepository;
    private readonly IBillRepository _billRepository;
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly IEventBus _eventBus;

    public RegisterQrPayment(
        IMapper mapper,
        IBankQrsRepository bankQrsRepository,
        IBillRepository billRepository,
        IEventBus eventBus)
    {
        _mapper = mapper;
        _bankQrsRepository = bankQrsRepository;
        _billRepository = billRepository;
        _eventBus = eventBus;
    }

    [Topic(GlobalConstanst.DAPR_PUBSUB_NAME, "RegisterQrPaymentCommand")]
    [HttpPost("/registerQrPayment")]
    //[Authorize(Policy = "MachinePolicy")]
    [SwaggerOperation(
        Summary = "Get a QR Detail",
        Description = "Get a QR Detail",
        OperationId = "BankQrs.Get",
        Tags = new[] { "BankQrsEndpoint" })
    ]
    public override async Task<ActionResult> HandleAsync([FromBody] RegisterQrPaymentCommand command, CancellationToken cancellationToken = default)
    {
        var bankQrFound = await _bankQrsRepository.GetByIdIncludePaymentsAsync(command.QrId);

        if (bankQrFound is null)
        {
            return NotFound("BankQr no fue encontrado");
        }

        var billFound = await _billRepository.GetBillIncludeBanckQrAsync(command.QrId);

        var qrPayment = new QrPayment(
            billFound.OrganizationId,
            (decimal)command.Amount,
            command.Currency,
            command.ClientName,
            command.BankOriginAccountNumber,
            command.Voucher,
            command.PayId
        );
        bankQrFound.RegisterPayment(qrPayment);
        billFound.RegisterPayment(qrPayment);
        await _billRepository.UpdateBill(billFound);
        //await _dbContext.SaveChangesAsync(cancellationToken);

        if (billFound.IsCompleted)
        {
            //TODO deberíamos pasarlo a un domainevent y un dispatcher?
            await _eventBus.PublishAsync(
                new BillPaidEvent(
                    billFound.OrganizationId,
                    billFound.Id
                )
            );
        }

        return Ok();
    }
}