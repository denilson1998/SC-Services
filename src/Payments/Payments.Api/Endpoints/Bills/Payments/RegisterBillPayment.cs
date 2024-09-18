using Ardalis.ApiEndpoints;
using Ardalis.RouteAndBodyModelBinding;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Payments.Domain.Interfaces.Repositories;
using Payments.Domain.Interfaces.Services;
using Payments.Infrastructure.Persistence;
using SharedKernel.Events;
using Sitec.Delivery.EventBus.Abstractions;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Payments.Api.Endpoints.Bills;
public class RegisterBillPayment : EndpointBaseAsync
    .WithRequest<RegisterBillPaymentCommand>
    .WithActionResult<GetBillResult>
{
    private readonly IBankConfigRepository _bankConfigRepository;
    private readonly IQRCommunicationService _qrCommunicationService;
    private readonly IBillRepository _billRepository;
    private readonly IVoucherRepository _voucherRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly IEventBus _eventBus;

    public RegisterBillPayment(IBankConfigRepository bankConfigRepository,
        IQRCommunicationService qrCommunicationService,
        IBillRepository billRepository,
        IVoucherRepository voucherRepository,
        IMapper mapper,
        ILogger<RegisterBillPayment> logger,
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

    [HttpPost("/organizations/{OrganizationId}/bills/{BillId}/payments")]
    [Authorize(Policy = "UserM2MPolicy")]
    [SwaggerOperation(
        Summary = "Register a payment for a bill",
        Description = "Register a payment for a bill",
        OperationId = "Payment.Batch",
        Tags = new[] { "PaymentEndpoint" })
    ]
    public override async Task<ActionResult<GetBillResult>> HandleAsync([FromRoute] RegisterBillPaymentCommand request, CancellationToken cancellationToken = default)
    {
        var billFound = await _billRepository.GetBillByIdIncludePaymentsAndVoucherAsync(request.BillId, request.OrganizationId);

        if (request.RegisterBillPaymentCommandBody.VoucherId is not null)
        {
            var voucherFound = await _voucherRepository.GetAsync(request.RegisterBillPaymentCommandBody.VoucherId, cancellationToken);

            billFound.RegisterPayment(voucherFound);
        }

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

        await _billRepository.UpdateBill(billFound);

        var result = _mapper.Map<GetBillResult>(billFound);

        return Created("", result);
    }
}
