using Microsoft.AspNetCore.Mvc;
using System.Security.AccessControl;
using TMS.BusinessLayer.DTO;
using TMS.BusinessLayer.Interface;
using TMS.BusinessLayer.Param;
using TMS.BusinessLayer.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.Worker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController: ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file is null)
            {
                return BadRequest("Invalid client request");
            }
            var response = await _studentService.HanleUploadFileAsync(file);
            return Ok(response);
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            return Ok("Hello World!");
        }

        [HttpGet("sample_upload_file")]
        public async Task<IActionResult> SampleUploadFile()
        {
            byte[] bytes = await _studentService.GetSampleUploadFile();

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "student_upload.xlsx"); 

        }

        [Route("export")]
        [HttpPost]
        public async Task<IActionResult> ExportReceipData(ExportParam exportExcelParam)
        {
            byte[] excelFileBytes = await _studentService.ExportExcelAsync(exportExcelParam);
            return File(excelFileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", exportExcelParam.FileName);
        }

    }
}
