using Microsoft.AspNetCore.Mvc;
using TMS.API.Param;
using TMS.BusinessLayer.DTO;
using TMS.BusinessLayer.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : BaseController<StudentDto>
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService): base(studentService)
        {
            _studentService = studentService;
        }

    }
}
