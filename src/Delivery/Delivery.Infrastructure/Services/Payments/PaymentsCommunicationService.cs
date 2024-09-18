using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SharedKernel.Contracts;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Delivery.Domain.Interfaces.Services;
using System.Collections.Specialized;
using SharedKernel.Contracts.Payments;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Delivery.Infrastructure.Services.QRService;
public class PaymentsCommunicationService : IPaymentsCommunicationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;


    public PaymentsCommunicationService(HttpClient httpClient, IOptions<PaymentsServiceConfig> qrServiceConfig, ILogger<PaymentsCommunicationService> logger)
    {
        httpClient.BaseAddress = new Uri(qrServiceConfig.Value.BaseUrl);
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CreateBillResponse> CreateBillAsync(CreateBillDto command, int timeout = 100)
    {
        // Prepare the data
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        var json = JsonConvert.SerializeObject(command, settings);
        // Make the request to Stock project
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        _httpClient.Timeout = TimeSpan.FromSeconds(timeout);
        try
        {
            var response = await _httpClient.PostAsync($"/organizations/{command.OrganizationId}/bills", httpContent);
            if (response.StatusCode is not HttpStatusCode.Created)
            {
                return null;
            }
            var responseStringContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<CreateBillResponse>(responseStringContent);
        }
        catch (Exception e)
        {

            _logger.LogError("No se logró crear el bill : {mensaje}", e);
            return null;
        }


    }

    public async Task<PagedResponse<ListBillsResult>> GetBillsAsync(ListBillsQueryFilters command, int timeout = 100)
    {
        // // Prepare the data
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("since", command.Since.ToString());

        // Make the request to Stock project
        _httpClient.Timeout = TimeSpan.FromSeconds(timeout);
        try
        {
            var response = await _httpClient.GetAsync($"/bills?{queryString}");
            var responseStringContent = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<PagedResponse<ListBillsResult>>(responseStringContent);
        }
        catch (Exception e)
        {

            _logger.LogError("No se logró obtner los bill : {mensaje}", e);
            return null;
        }

    }
}
