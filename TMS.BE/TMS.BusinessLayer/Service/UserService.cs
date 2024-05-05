using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TMS.BaseRepository;
using TMS.BaseService;
using TMS.BusinessLayer.DTO;
using TMS.BusinessLayer.Interface;
using TMS.DataLayer.Interface;

namespace TMS.BusinessLayer.Service
{
    public class UserService : IUserService
    {
        private readonly ITenantService _tenantService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public UserService(ITenantService tenantService, IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _tenantService = tenantService;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;


            // get connection string from httpcontext items

            var connectionString = _httpContextAccessor.HttpContext.Items["ConnectionString"].ToString(); 

            if (connectionString == null)
            {
                throw new Exception("ConnectionString is not cofigured");
            }

            unitOfWork.SetConnectionString(connectionString);
        }

        public async Task<ServiceResponse<bool>> ChangePasswordAsync(string oldPass, string newPass, string confirmPass)
        {
            var result = new ServiceResponse<bool>();   
            var userId = _httpContextAccessor.HttpContext.User.FindFirst("UserId").Value;
            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
            if (user == null)
            {
                result.ErrorCode = "USER_NOT_FOUND";
                result.Message = "Có lỗi xảy ra. Vui lòng thử lại.";
                result.Data = false;
            } else
            {
                // check old password equal to current password
                if (!BCrypt.Net.BCrypt.Verify(oldPass, user.Password))
                {
                    result.ErrorCode = "INVALID_PASSWORD";
                    result.Message = "Mật khẩu cũ không đúng";
                    result.Data = false;
                }
                else
                {
                    if (newPass != confirmPass)
                    {
                        result.ErrorCode = "INVALID_INFO";
                        result.Message = "Mật khẩu mới không trùng khớp";
                        result.Data = false;
                    }
                    else
                    {
                        user.Password = BCrypt.Net.BCrypt.HashPassword(newPass);
                        await _unitOfWork.OpenAsync();
                        await _userRepository.Update(user);
                        await _unitOfWork.CommitAsync();
                        result.Data = true;
                    }
                }   
            }

            return result;
        }

        public async Task<ServiceResponse<LoginResponseDto>> Login(string username, string password)
        {
            try
            {
                var response = new ServiceResponse<LoginResponseDto>();
                var user = await _userRepository.GetUserByUsername(username);

                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim("FullName", user.FullName),
                        new Claim("UserId", user.UserId.ToString())
                    };

                    var accessToken = GenerateToken(claims);
                    var refreshToken = GenerateRefreshToken();

                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExp = DateTime.Now.AddDays(double.Parse(_configuration["JWT:RefreshTokenExpiryInDays"] ?? "7"));

                    await _unitOfWork.OpenAsync();
                    await _userRepository.Update(user);
                    await _unitOfWork.CommitAsync();

                    response.Data = new LoginResponseDto
                    {
                        User = _mapper.Map<UserDto>(user)
                    };

                    var httpResponse = _httpContextAccessor.HttpContext.Response;

                    // set access token to cookie
                    httpResponse.Cookies.Append("x-token", accessToken, new CookieOptions
                    {
                        HttpOnly = false,
                        Expires = DateTime.Now.AddHours(24)
                    });

                    // set refresh token to cookie with path /refresh-token
                    httpResponse.Cookies.Append("x-refresh-token", refreshToken, new CookieOptions
                    {
                        HttpOnly = false,
                        Expires = DateTime.Now.AddDays(double.Parse(_configuration["JWT:RefreshTokenExpiryInDays"] ?? "2") + 1),
                        Path = "api/users/refresh-token"
                    });
                }
                else
                {
                    response.ErrorCode = "INVALID_INFO";
                    response.Message = "Sai tên đăng nhập hoặc mật khẩu";
                }

                return response;
            }
                catch
            {
                throw;
            }
            finally
            {
                await _unitOfWork.CloseAsync();
            }

        }

        private string GenerateToken(List<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["JWT:TokenExpiryInMinutes"] ?? "15")),
                signingCredentials: signinCredentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

            return tokenString;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = _configuration["JWT:ValidAudience"],
                ValidateIssuer = true,
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

        }

        public async Task<ServiceResponse<bool>> RefreshToken(string accessToken, string refreshToken)
        {
            try
            {
                await _unitOfWork.OpenAsync();
                var principal = GetPrincipalFromExpiredToken(accessToken);
                if (principal == null)
                {
                    throw new Exception("Invalid access token or refresh token");
                }

                string username = principal.Identity.Name;
                var user = await _userRepository.GetUserByUsername(username);

                if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExp < DateTime.Now)
                {
                    throw new Exception("Invalid access token or refresh token");
                }

                var newAccessToken = GenerateToken(principal.Claims.ToList());
                var newRefreshToken = GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                await _userRepository.Update(user);

                await _unitOfWork.CommitAsync();

                var response = new ServiceResponse<bool>
                {
                    Data = true
                };


                var httpResponse = _httpContextAccessor.HttpContext.Response;

                // set access token to cookie
                httpResponse.Cookies.Append("x-token", newAccessToken, new CookieOptions
                {
                    HttpOnly = false,
                    Expires = DateTime.Now.AddHours(24)
                });

                // set refresh token to cookie with path /refresh-token
                httpResponse.Cookies.Append("x-refresh-token", newRefreshToken, new CookieOptions
                {
                    HttpOnly = false,
                    Expires = DateTime.Now.AddDays(double.Parse(_configuration["JWT:RefreshTokenExpiryInDays"] ?? "2") + 1),
                    Path = "api/users/refresh-token"
                });

                return response;

            }
            catch
            {
                throw;
            }
            finally
            {
                await _unitOfWork.CloseAsync();
            }

        }
    }
}
