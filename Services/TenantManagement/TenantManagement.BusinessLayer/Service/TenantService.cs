using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenantManagement.BusinessLayer.DTO;
using TenantManagement.BusinessLayer.Interface;
using TenantManagement.DataLayer.Interface;
using TMS.BaseRepository;
using TMS.BaseService;
using TMS.DataLayer.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace TenantManagement.BusinessLayer.Service
{
    public class TenantService : BaseService<Tenant, TenantDto>, ITenantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantRepository _tenantRepository;
        private readonly IConfiguration _configuration;


        public TenantService(IConfiguration configuration, ITenantRepository tenantRepository, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(tenantRepository, mapper, unitOfWork, httpContextAccessor)
        {
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;

            var connectionString = configuration.GetConnectionString("SqlConnection");

            if (connectionString == null || connectionString.Length == 0)
            {
                throw new Exception("ConnectionString is not cofigured");
            }
            unitOfWork.SetConnectionString(connectionString);
        }

        public override void BeforeCreate()
        {
            base.BeforeCreate();
            var a = 1;
            Console.WriteLine(a);
        }
    }
}
