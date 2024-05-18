using Microsoft.AspNetCore.Mvc;
using TMS.BusinessLayer.DTO;
using TMS.BusinessLayer.Interface;
using TMS.BusinessLayer.Param;
using TMS.BusinessLayer.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.Worker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeachersController(ITeacherService teacherService)
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

        [HttpGet("sample_upload_file")]
        public async Task<IActionResult> SampleUploadFile()
        {
            byte[] bytes = await _teacherService.GetSampleUploadFile();

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "teacher_upload.xlsx");

        }

        [Route("export")]
        [HttpPost]
        public async Task<IActionResult> ExportReceipData(ExportParam exportExcelParam)
        {
            byte[] excelFileBytes = await _teacherService.ExportExcelAsync(exportExcelParam);
            return File(excelFileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", exportExcelParam.FileName);
        }

    }
}
