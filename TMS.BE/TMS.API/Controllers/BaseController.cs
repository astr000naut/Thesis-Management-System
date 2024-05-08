using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMS.BaseRepository.Param;
using TMS.BaseService;


namespace TMS.API.Controllers
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
            var result =  await _baseService.CreateAsync(tEntityInputDto);
            return Ok(result);
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
            var entity = await _baseService.GetByIdAsync(id.ToString());
            return Ok(entity);
        }

        [HttpGet("new")]
        [Authorize]
        public async Task<IActionResult> GetNew()
        {
            var entity = await _baseService.GetNew();
            return Ok(entity);
        }

        /// <summary>
        /// Filter danh sách đối tượng
        /// </summary>
        /// <param name="entityFilterDto"></param>
        /// <returns>filtered List</returns>
        /// Author: DNT(29/05/2023)
        [Route("filter")]
        [HttpPost]
        public async Task<IActionResult> FilterAsync([FromBody] FilterParam filterParam)
        {
            var (data, total) = await _baseService.FilterAsync(filterParam);
            var response = new
            {
                data = data,
                total = total,
            };
            return Ok(response);
        }

        /// <summary>
        /// Cập nhật một đối tượng
        /// </summary>
        /// <param name="id">Id của đối tượng</param>
        /// <param name="tEntityInputDto">EntityUpdateDto của đối tượng</param>
        /// <returns>Giá trị boolean đã cập nhật hay chưa</returns>
        /// Author: DNT(24/05/2023)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync([FromBody] TEntityDto tEntityDto)
        {
            var updated = await _baseService.UpdateAsync(tEntityDto);
            return Ok(updated);
        }



        [Route("delete")]
        [HttpPost]
        public async Task<IActionResult> DeleteMultipleAsync([FromBody] List<string> entityIdList)
        {
            int deleted;
            if (entityIdList.Count == 1)
            {
                deleted = await _baseService.DeleteAsync(entityIdList[0]);
            } else
            {
                deleted = await _baseService.DeleteMultipleAsync(entityIdList);
            }
            return Ok(deleted);
        }



    }
}
