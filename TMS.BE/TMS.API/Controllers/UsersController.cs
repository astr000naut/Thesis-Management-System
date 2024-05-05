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
        [Authorize(Roles = "ADMIN")]
        public ActionResult Test()
        {
            return Ok("Test");
        }

        [HttpGet]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            // get access token from x-token cookies
            var accessToken = Request.Cookies["x-token"];
            // get refresh token from x-refresh-token cookies
            var refreshToken = Request.Cookies["x-refresh-token"];

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("Invalid client request");
            }

            var response = await _userService.RefreshToken(accessToken, refreshToken);
            return Ok(response);
        }
    }
}
