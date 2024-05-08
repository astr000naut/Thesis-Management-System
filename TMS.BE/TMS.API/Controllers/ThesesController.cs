using Microsoft.AspNetCore.Mvc;
using TMS.API.Param;
using TMS.BusinessLayer.DTO;
using TMS.BusinessLayer.Interface;
using TMS.BusinessLayer.Service;
using TMS.DataLayer.Param;

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

        [HttpGet("guiding-list/{teacherId}")]
        public async Task<IActionResult> GetGuidingList(Guid teacherId)
        {
            var guidingList = await _thesisService.GetGuidingList(teacherId.ToString());
            return Ok(guidingList);
        }

        [HttpPost("list-thesis-guided")]
        public async Task<IActionResult> GetListThesisGuided([FromBody] TeacherThesisFilterParam param)
        {
            var result = await _thesisService.GetListThesisGuided(param);
            var response = new
            {
                data = result.Item1,
                total = result.Item2,
            };
            return Ok(response);
        }


    }
}
