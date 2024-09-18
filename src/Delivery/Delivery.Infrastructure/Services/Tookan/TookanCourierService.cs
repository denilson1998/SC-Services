using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Delivery.Domain.Interfaces;
using Delivery.Domain.Services.Contracts;
using Delivery.Infrastructure.Services.Tookan;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Delivery.Domain.Interfaces.Services;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using Delivery.Infrastructure.Services.WebHook;

namespace Delivery.Infrastructure.Services.Logistics;

public class TookanCourierService : ICourierCommunicationService
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<TookanCourierConfig> _tookanCourierConfig;
    private readonly ILogger _logger;
    public string _tookanDateResponse = "0000-00-00 00:00:00";

    public TookanCourierService(HttpClient httpClient, IOptions<TookanCourierConfig> tookanCourierConfig, ILogger<TookanCourierService> logger)
    {
        httpClient.BaseAddress = new Uri(tookanCourierConfig.Value.BaseUrl);
        _httpClient = httpClient;
        _tookanCourierConfig = tookanCourierConfig;
        _logger = logger;
    }

    public async Task<List<ListCourierAgentsResponse>> GetCourierAgentsAsync()
    {
        var contract = new TookanGetAgentsRequest()
        {
            api_key = _tookanCourierConfig.Value.ApiKey
        };
        var json = JsonConvert.SerializeObject(contract);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        var rawResponse = await _httpClient.PostAsync("/v2/get_all_fleets", httpContent);

        if (rawResponse.StatusCode is not HttpStatusCode.OK)
        {
            _logger.LogError("No se logró obtener los Courier Agents: {mensaje}", rawResponse.Content.ToString());
            return null;
        }

        var rawResponseStringContent = rawResponse.Content.ReadAsStringAsync().Result;

        var response = JsonConvert.DeserializeObject<TookanGenericListResponse<TookanGetAgentsResponse>>(rawResponseStringContent);

        return response.data.ConvertAll(d =>
        {
            return new ListCourierAgentsResponse()
            {
                FullName = d.name,
                ExternalCourierAgentId = d.fleet_id,
                PhoneNumber = d.phone,
                IsActive = d.is_active,
                HasGpsAccuracy = d.has_gps_accuracy,
                // TransportType = d.transport_type,
                TransportDescription = d.transport_desc,
                LicensePlate = d.license,
                Tags = d.tags,
                Email = d.email,
                CellphoneBatteryLevel = d.battery_level,
                Latitude = d.latitude,
                Longitude = d.longitude,
                PictureUri = d.fleet_image
            };
        });
    }

    public async Task<List<ListCourierTasksResponse>> GetCourierTasksAsync(IEnumerable<string> jobIds)
    {
        var contract = new TookanGetTaskDetailsCommand()
        {
            api_key = _tookanCourierConfig.Value.ApiKey,
            job_ids = jobIds
        };
        var json = JsonConvert.SerializeObject(contract);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        var rawResponse = await _httpClient.PostAsync("/v2/get_job_details", httpContent);
        var rawResponseStringContent = rawResponse.Content.ReadAsStringAsync().Result;
        Console.WriteLine(rawResponseStringContent);
        // TODO log error
        rawResponse.EnsureSuccessStatusCode();

        var response = JsonConvert.DeserializeObject<TookanGenericListResponse<TookanGetTaskDetailsResponse>>(rawResponseStringContent);
        return response.data.ConvertAll(d =>
        {
            return new ListCourierTasksResponse()
            {
                ExternalTaskId = d.job_id.ToString(),
                FleetId = d.fleet_id,
                AcceptedAt = (_tookanDateResponse == d.acknowledged_datetime) ? null : Convert.ToDateTime(d.acknowledged_datetime),
                AssignedAt = d.fleet_id is not null ? DateTime.Now : null,
                StartedAt = (_tookanDateResponse == d.started_datetime) ? null : Convert.ToDateTime(d.started_datetime),
                ArrivedAt = (_tookanDateResponse == d.arrived_datetime) ? null : Convert.ToDateTime(d.arrived_datetime),
                CompletedAt = (_tookanDateResponse == d.completed_datetime) ? null : Convert.ToDateTime(d.completed_datetime),
                SucceededAt = d.task_history.Find(t => t.description == "Successful at")?.creation_datetime,
                FailedAt = d.task_history.Find(t => t.description == "Failed at")?.creation_datetime,
                CanceledAt = d.task_history.Find(t => t.description == "Canceled at")?.creation_datetime,
                //d.order_id
            };
        });
    }

    public async Task<CreateExternalPickupAndDeliveryResponse> CreatePickupAndDeliveryAsync(CreateExternalPickupAndDeliveryCommand command)
    {
        var contract = new TookanCreatePickupAndDeliveryTaskContract
        {
            // Pickup Address
            job_pickup_phone = command.OriginAddress.PhoneNumber,
            job_pickup_name = command.OriginAddress.ClientName,
            job_pickup_latitude = command.OriginAddress.Latitude,
            job_pickup_longitude = command.OriginAddress.Longitude,
            job_pickup_address = command.OriginAddress.Reference,
            // job_pickup_email = command.OriginAddress.Email

            // Delivery Address
            customer_phone = command.DeliveryAddress.PhoneNumber,
            customer_username = command.DeliveryAddress.ClientName,
            latitude = command.DeliveryAddress.Latitude,
            longitude = command.DeliveryAddress.Longitude,
            customer_address = command.DeliveryAddress.Reference,

            // Definir
            // Son obligatorios los dos datetime
            job_pickup_datetime = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"), // tiene que ser mayor a datetime now
            job_delivery_datetime = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"),

            // Fixed for DeliveryAndPickup
            has_pickup = TookanCourierConstants.HasPickup,
            has_delivery = TookanCourierConstants.HasDelivery,
            layout_type = TookanCourierConstants.PickupAndDeliveryLayoutType,
            tracking_link = TookanCourierConstants.HasTrackingLink,

            // Fixed
            api_key = _tookanCourierConfig.Value.ApiKey,
            timezone = TookanCourierConstants.Timezone,

            // TODO considerar
            auto_assignment = true ? "1" : "0",
            job_description = "",
            // fleet_id // agent id
            // notify
            // geofence
            // ignore_customer_lat_long	integer	This is used to add address sent in request body otherwise tookan fetch customer by phone number saved in tookan database and use that customer adddress .
            // tags	string	This is a string containing comma separated tags as filters for agents in auto assignment.
            // ride_type	number	0 for Non-Pool Task, 1 for Pool Task.
        };

        var json = JsonConvert.SerializeObject(contract);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        var rawResponse = await _httpClient.PostAsync("/v2/create_task", httpContent);
        var rawResponseStringContent = rawResponse.Content.ReadAsStringAsync().Result;
        var response = JsonConvert.DeserializeObject<TookanGenericResponse<TookanCreatePickupAndDeliveryTaskResponse>>(rawResponseStringContent);

        if (response.message != "The task has been created.")
        {
            throw new Exception($"Exception while creating a tookan task: {response.message}");
        }
        return new CreateExternalPickupAndDeliveryResponse()
        {
            ExternalTaskId = response.data.job_id,
            PickupTrackingLink = response.data.pickup_tracking_link,
            DeliveryTrackingLink = response.data.delivery_tracing_link
        };
    }
}
