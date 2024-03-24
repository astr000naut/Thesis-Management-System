using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TenantManagement.API.Param;
using TenantManagement.BusinessLayer.DTO;
using TenantManagement.BusinessLayer.Interface;

namespace TenantManagement.API.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService) 
        {
            _userService = userService;
        }


        // how to get body from request

        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate([FromBody] LoginParam param)
        {
            LoginResponseDto response = await _userService.Login(param.Username, param.Password);
            return Ok(response);
        }

        [HttpGet]
        [Authorize(Roles ="admin")]
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

            LoginResponseDto response = await _userService.RefreshToken( refreshTokenParam.AccessToken, refreshTokenParam.RefreshToken);
            return Ok(response);
        }
    }
}
