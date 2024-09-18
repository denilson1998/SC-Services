using System;
using SharedKernel.AbstractEntities;
using SharedKernel.Interfaces;

namespace Delivery.Domain.Webhooks;

public class Webhook : AuditableEntity, IMultiTenant, ISoftDelete
{
    public int OrganizationId { get; set; }
    public string Url { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletionDateTime { get; set; }
}
