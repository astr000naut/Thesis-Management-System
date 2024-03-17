using Microsoft.AspNetCore.Mvc;
using TMS.BaseService;


namespace TenantManagement.API.Controllers
{

    [ApiController]
    public abstract class BaseController<TEntityDto> : ControllerBase
    { 
        protected readonly IBaseService<TEntityDto> _baseService;

        public BaseController(IBaseService<TEntityDto> baseService)
        {
            _baseService = baseService;
        }

        /// <summary>
        /// API Tạo mới một đối tượng
        /// </summary>
        /// <param name="tEntityInputDto"></param>
        /// <returns>Id của đối tượng được tạo</returns>
        /// Author: DNT(24/05/2023)
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] TEntityDto tEntityInputDto)
        {
            var newId =  await _baseService.CreateAsync(tEntityInputDto);
            return Created("", newId);
        }

        /// <summary>
        /// Lấy một đối tượng theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Thông tin của đối tượng</returns>
        /// Author: DNT(24/05/2023)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var entity = await _baseService.GetByIdAsync(id);
            return Ok(entity);
        }

        /// <summary>
        /// Filter danh sách đối tượng
        /// </summary>
        /// <param name="entityFilterDto"></param>
        /// <returns>filtered List</returns>
        /// Author: DNT(29/05/2023)
        [Route("Filter")]
        [HttpGet]
        public Task<IActionResult> FilterAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Cập nhật một đối tượng
        /// </summary>
        /// <param name="id">Id của đối tượng</param>
        /// <param name="tEntityInputDto">EntityUpdateDto của đối tượng</param>
        /// <returns>Giá trị boolean đã cập nhật hay chưa</returns>
        /// Author: DNT(24/05/2023)
        [HttpPut("{id}")]
        public Task<IActionResult> PutAsync(Guid id, [FromBody] TEntityDto tEntityDto)
        {
            throw new NotImplementedException();
        }



        [Route("DeleteMultiple")]
        [HttpPost]
        public Task<IActionResult> DeleteMultipleAsync([FromBody] List<Guid> entityIdList)
        {
            throw new NotImplementedException();
        }

    }
}
