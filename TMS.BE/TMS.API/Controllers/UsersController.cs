using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMS.API.Param;
using TMS.BaseService;
using TMS.BusinessLayer.DTO;
using TMS.BusinessLayer.Interface;

namespace TMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }


        // how to get body from request

        [HttpPost("authenticate")]
        public async Task<ServiceResponse<LoginResponseDto>> Authenticate([FromBody] LoginParam param)
        {
            var response = await _userService.Login(param.Username, param.Password);
            return response;
        }

        [HttpPost("change-password")]
        public async Task<ServiceResponse<bool>> ChangePassword([FromBody] ChangePasswordParam param)
        {
            var response = await _userService.ChangePasswordAsync(param.OldPass, param.NewPass, param.ConfirmPass);
            return response;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Test()
        {
            return Ok("Test");
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenParam refreshTokenParam)
        {
            if (refreshTokenParam is null)
            {
                return BadRequest("Invalid client request");
            }

            LoginResponseDto response = await _userService.RefreshToken(refreshTokenParam.AccessToken, refreshTokenParam.RefreshToken);
            return Ok(response);
        }
    }
}
