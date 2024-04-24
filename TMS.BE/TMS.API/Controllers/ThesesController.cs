using Microsoft.AspNetCore.Mvc;
using TMS.API.Param;
using TMS.BusinessLayer.DTO;
using TMS.BusinessLayer.Interface;
using TMS.BusinessLayer.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThesesController : BaseController<ThesisDto>
    {
        private readonly IThesisService _thesisService;

        public ThesesController(IThesisService thesisService): base(thesisService)
        {
            _thesisService = thesisService;
        }


    }
}
