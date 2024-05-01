using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BaseRepository;
using TMS.BaseService;
using TMS.BusinessLayer.DTO;
using TMS.BusinessLayer.Interface;
using TMS.DataLayer.Entity;
using TMS.DataLayer.Interface;

namespace TMS.BusinessLayer.Service
{
    public class SettingService : BaseService<Setting, SettingDto>, ISettingService
    {
        private readonly ISettingRepository _settingRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public SettingService(ISettingRepository settingRepository, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(settingRepository, mapper, unitOfWork)
        {
            _settingRepository = settingRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;

            // get connection string from httpcontext items

            var connectionString = _httpContextAccessor.HttpContext.Items["ConnectionString"].ToString();

            if (connectionString == null)
            {
                throw new Exception("ConnectionString is not cofigured");
            }

            _unitOfWork.SetConnectionString(connectionString);
        }

    }

}
