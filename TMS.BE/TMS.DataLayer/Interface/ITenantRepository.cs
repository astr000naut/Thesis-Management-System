using TMS.DataLayer.Entity;

namespace TMS.DataLayer.Interface
{
    public interface ITenantRepository
    {
        Task<Tenant> GetTenantByDomain(string domain);
        Task<Tenant> GetTenantById(string id);

    }
}
