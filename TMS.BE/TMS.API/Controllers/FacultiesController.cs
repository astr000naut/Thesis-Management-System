using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMS.API.Param;
using TMS.BusinessLayer.DTO;
using TMS.BusinessLayer.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class FacultiesController : BaseController<FacultyDto>
    {
        private readonly IFacultyService _facultyService;

        public FacultiesController(IFacultyService facultyService): base(facultyService)
        {
            _facultyService = facultyService;
        }

    }
}
