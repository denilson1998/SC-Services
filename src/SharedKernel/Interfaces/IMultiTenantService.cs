namespace SharedKernel.Interfaces;

public interface IMultiTenantService
{
    public int GetOrganizationId();

    public void OverrideOrganizationId(int organizationId);
}
