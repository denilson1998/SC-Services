using SharedKernel.Interfaces;

namespace SharedKernel;

public class MultiTenantService : IMultiTenantService
{
    private int TenantId;

    public MultiTenantService()
    {
        TenantId = 0;
    }

    public int GetOrganizationId()
    {
        return TenantId;
    }

    public void OverrideOrganizationId(int organizationId)
    {
        TenantId = organizationId;
    }
}
