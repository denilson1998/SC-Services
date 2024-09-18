using System;
using SharedKernel.AbstractEntities;

namespace Payments.Domain.Entities;

public class Worker: AuditableEntity
{
    public string Name { get; set; }
    public DateTime LastRunAt { get; set; }
    public DateTime? DataSyncedSince { get; set; }
}
