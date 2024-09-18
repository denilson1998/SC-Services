using System;

namespace Delivery.Domain.Services.Contracts;

public class ListCourierTasksResponse
{
    public string ExternalTaskId { get; set; }
    public int? FleetId { get; set; }
    public DateTime? AssignedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? ArrivedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? SucceededAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public DateTime? CanceledAt { get; set; }

}