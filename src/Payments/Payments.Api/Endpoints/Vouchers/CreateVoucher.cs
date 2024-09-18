using Ardalis.ApiEndpoints;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Payments.Domain.Entities;
using Payments.Domain.Interfaces.Repositories;
using Payments.Domain.Interfaces.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Payments.Api.Endpoints.Vouchers;

public class CreateVoucher : EndpointBaseAsync
    .WithRequest<CreateVoucherCommand>
    .WithActionResult<CreateVoucherResult>
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IQRCommunicationService _qrCommunicationService;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public CreateVoucher(IVoucherRepository voucherRepository,
           IQRCommunicationService qrCommunicationService,
           IMapper mapper,
           ILogger<CreateVoucher> logger)
    {
        _voucherRepository = voucherRepository;
        _qrCommunicationService = qrCommunicationService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost("/organizations/{OrganizationId}/vouchers")]
    [Authorize(Policy = "UserM2MPolicy")]
    [SwaggerOperation(
        Summary = "Creates payments for an order",
        Description = "Creates payments for an order",
        OperationId = "Payment.Batch",
        Tags = new[] { "PaymentEndpoint" })
    ]
    public override async Task<ActionResult<CreateVoucherResult>> HandleAsync([FromRoute] CreateVoucherCommand request, CancellationToken cancellationToken = default)
    {
        var voucher = new Voucher(
            request.OrganizationId,
            request.CreateVoucherBody.Value,
            request.CreateVoucherBody.Currency,
            request.CreateVoucherBody.IsPercentage,
            request.CreateVoucherBody.Quantity
        );
        
        voucher.SetValidDates(request.CreateVoucherBody.ValidSince, request.CreateVoucherBody.ValidUntil);
        
        var voucherResult = await _voucherRepository.CreateAsync(voucher, cancellationToken);
        
        var result = _mapper.Map<CreateVoucherResult>(voucherResult);

        return Created("", result);
    }
}