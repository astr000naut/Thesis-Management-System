using Microsoft.AspNetCore.Mvc;
using TMS.API.Param;
using TMS.BusinessLayer.DTO;
using TMS.BusinessLayer.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : BaseController<SettingDto>
    {
        private readonly ISettingService _settingService;

        public SettingsController(ISettingService settingService): base(settingService)
        {
            _settingService = settingService;
        }

    }
}
