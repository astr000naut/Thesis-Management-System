using AutoMapper;

namespace TMS.Worker.Core.Profiles
{
    public class TenantProfile : Profile
    {
        public TenantProfile()
        {
            CreateMap<Tenant, TenantDto>();
            CreateMap<Tenant, TenantLiteDto>();
            CreateMap<TenantDto, Tenant>();

        }
    }
}
