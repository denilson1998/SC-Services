// using Ardalis.ApiEndpoints;
// using AutoMapper;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Payments.Domain;
// using Payments.Domain.Interfaces.Repositories;
// using Payments.Domain.Interfaces.Services;
// using Payments.Infrastructure.Persistence;
// using Payments.Infrastructure.Services.QRService;
// using Swashbuckle.AspNetCore.Annotations;
// using System;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;

// namespace Payments.Api.Endpoints.Payments;
// public class GetBankQr : EndpointBaseAsync
//     .WithRequest<GetBankQrCommand>
//     .WithActionResult<GetBankQrResult>
// {
//     private readonly IMapper _mapper;
//     private readonly IQRCommunicationService _qrCommunicationService;
//     private readonly ApplicationDbContext _dbContext;

//     public GetBankQr(
//         IMapper mapper,
//         IQRCommunicationService qrCommunicationService,
//         ApplicationDbContext dbContext)
//     {
//         _qrCommunicationService = qrCommunicationService;
//         _mapper = mapper;
//         _dbContext = dbContext;
//     }

//     [HttpPost("/organizations/{OrganizationId}/bankQrs/{bankQrId}")]
//     [Authorize(Policy = "MustBeOnTheOrganization")]
//     [SwaggerOperation(
//         Summary = "Get a QR Detail",
//         Description = "Get a QR Detail",
//         OperationId = "BankQrs.Get",
//         Tags = new[] { "BankQrsEndpoint" })
//     ]
//     public override async Task<ActionResult<GetBankQrResult>> HandleAsync([FromRoute] GetBankQrCommand request, CancellationToken cancellationToken = default)
//     {
//         var bankQrFound = await _dbContext
//             .BankQrs
//             .Where(b => b.Id == request.BankQrId)
//             .FirstOrDefaultAsync(cancellationToken);

//         if (bankQrFound is null)
//         {
//             return NotFound("BankQr no fue encontrado");
//         }

//         if (bankQrFound.EncryptedQrString is null)
//         {
//             bankQrFound.Currency = bankQrFound.Currency;
//             bankQrFound.Currency = bankQrFound.Currency;
//             bankQrFound.ExpirationDate = DateTime.UtcNow.AddMonths(1);

//             var qrResponse = await _qrCommunicationService.GenerateQrStringAsync(bankQrFound);
//             if (qrResponse is null)
//             {
//                 return StatusCode(500, "No se pudo generar el QR para la transacción.");
//             }
//             bankQrFound.EncryptedQrString = qrResponse.Encrypt;
//             bankQrFound.QrId = qrResponse.Id;
//             bankQrFound.ClientName = qrResponse.AccountHolder;
//         }

//         await _dbContext.SaveChangesAsync(cancellationToken);

//         var result = _mapper.Map<GetBankQrResult>(bankQrFound);

//         return Ok(result);
//     }
// }
