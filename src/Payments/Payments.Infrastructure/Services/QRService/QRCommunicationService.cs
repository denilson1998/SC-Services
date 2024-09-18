using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Payments.Domain.Enums;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Payments.Domain.Entities;
using Payments.Domain.Interfaces.Services;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Extensions.Logging;

namespace Payments.Infrastructure.Services.QRService;
public class QRCommunicationService : IQRCommunicationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    public QRCommunicationService(HttpClient httpClient, IOptions<QRServiceConfig> qrServiceConfig, ILogger<QRCommunicationService> logger)
    {
        httpClient.BaseAddress = new Uri(qrServiceConfig.Value.BaseUrl);
        _httpClient = httpClient;
        _logger = logger;

    }

    public async Task<TransferQrResponse> GenerateQrStringAsync(TransferQrDto command, int timeout = 100)
    {
        // Prepare the data
        var data = new GenerateQrStringRequest
        {
            ClientCode = command.ClientCode,
            Currency = command.Currency,
            Amount = command.Amount,
            Description = command.Description,
            SingleUse = command.SingleUse,
            SystemModules = command.SystemModules,
            AccountNumber = command.AccountNumber,
            ExpirationDate = command.ExpirationDate
        };

        var settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        var json = JsonConvert.SerializeObject(data, settings);
        // Make the request to Stock project
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        _httpClient.Timeout = TimeSpan.FromSeconds(timeout);
        try
        {
            var response = await _httpClient.PostAsync("/api/v1/qr", httpContent);
            if (response.StatusCode is not HttpStatusCode.Created)
            {
                _logger.LogError("No se logró Generar el QR: {mensaje}", response.Content.ToString());
                return null;
            }
            response.EnsureSuccessStatusCode();
            var responseStringContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TransferQrResponse>(responseStringContent);
        }
        catch (Exception unhandledException)
        {

            _logger.LogError("No se logró Generar el QR: {mensaje}", unhandledException);
            return null;
        }

    }

    public async Task<ListBankQrPaymentsPagedResult> GetBankQrPayments(ListBankQrPaymentsRequest request, int timeout = 100)
    {
        // // Prepare the data
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("StartDate", request.StartDate.ToString());
        queryString.Add("PerPage", request.PerPage.ToString());

        // Make the request to Stock project
        _httpClient.Timeout = TimeSpan.FromSeconds(timeout);
        var response = await _httpClient.GetAsync("api/v1/payment");
        var responseStringContent = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<ListBankQrPaymentsPagedResult>(responseStringContent);
    }

    // private static TransferQrDto GetQrDtoFromPayment(BankQr bankQr)
    // {
    //     return new TransferQrDto
    //     {
    //         ClientCode = bankQr.ClientId,
    //         Currency = bankQr.Currency,
    //         Amount = bankQr.Amount,
    //         Description = bankQr.Description,
    //         ExpirationDate = bankQr.ExpirationDate,
    //         SingleUse = false,
    //         SystemModules = (SystemModules)bankQr.BankAccountType,
    //         AccountNumber = bankQr.BankAccountNumber,
    //         Metadata = $"{{ \"OrganizationId\": {bankQr.OrganizationId}}}"
    //     };
    // }
}
