using Delivery.Domain.Services.Webhook;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.Infrastructure.Services.WebHook
{
    public class WebhooksComunicationService : IWebhooksComunicationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public WebhooksComunicationService(HttpClient httpClient, ILogger<WebhooksComunicationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<dynamic> Send(WebhookCommand webhook, string url, CancellationToken cancellationToken)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var webhookBody = JsonConvert.DeserializeObject(webhook.Body);

            var json = JsonConvert.SerializeObject(webhookBody, settings);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            _httpClient.BaseAddress = new Uri(url);
            _httpClient.Timeout = TimeSpan.FromSeconds(100);
            try
            {
                var response = await _httpClient.PostAsync(url, httpContent, cancellationToken);
                if (response.StatusCode is not HttpStatusCode.OK)
                {
                    _logger.LogError("Webhook not found: {mensaje}", response.Content.ToString());
                    return null;
                }
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (Exception unhandledException)
            {
                _logger.LogError("Webhook not found: {mensaje}", unhandledException);
                return null;
            }
        }
    }
}