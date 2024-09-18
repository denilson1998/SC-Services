using SharedKernel.AbstractEntities;

namespace Delivery.Domain.Entities
{
    public class User : AuditableEntity
    {
        public int OrganizationId { get; set; }
        public int ClientId { get; set; }
    }
}