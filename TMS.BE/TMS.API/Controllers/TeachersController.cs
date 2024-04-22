using Microsoft.AspNetCore.Mvc;
using TMS.API.Param;
using TMS.BusinessLayer.DTO;
using TMS.BusinessLayer.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : BaseController<TeacherDto>
    {
        private readonly ITeacherService _teacherService;

        public TeachersController(ITeacherService teacherService): base(teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file is null)
            {
                return BadRequest("Invalid client request");
            }
            var response = await _teacherService.HanleUploadFileAsync(file);
            return Ok(response);
        }

    }
}
