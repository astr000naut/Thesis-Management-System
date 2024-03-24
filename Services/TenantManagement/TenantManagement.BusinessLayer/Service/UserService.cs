using AutoMapper;
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
using TenantManagement.BusinessLayer.DTO;
using TenantManagement.BusinessLayer.Interface;
using TenantManagement.DataLayer.Interface;
using TMS.BaseRepository;

namespace TenantManagement.BusinessLayer.Service
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IConfiguration configuration, IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper) { 
            _configuration = configuration;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;

            var connectionString = configuration.GetConnectionString("SqlConnection");

            if (connectionString == null || connectionString.Length == 0)
            {
                throw new Exception("ConnectionString is not cofigured");
            }

            unitOfWork.SetConnectionString(connectionString);
        }
        public async  Task<LoginResponseDto> Login(string username, string password)
        {
            try
            {
                var user = await _userRepository.GetUserByUsername(username);
                if (user == null)
                {
                    throw new Exception("Sai tên đăng nhập hoặc mật khẩu");
                }

                if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Role)
                    };

                    var accessToken = GenerateToken(claims);
                    var refreshToken = GenerateRefreshToken();

                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExp = DateTime.Now.AddDays(double.Parse(_configuration["JWT:RefreshTokenExpiryInDays"] ?? "7"));

                    await _unitOfWork.OpenAsync();
                    await _userRepository.Update(user);
                    await _unitOfWork.CommitAsync();

                    return new LoginResponseDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        User = _mapper.Map<UserDto>(user)
                    };
                }
                else
                {
                    throw new Exception("Sai tên đăng nhập hoặc mật khẩu");
                }
            } catch
            {
                throw;
            } finally
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
                ValidateLifetime = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

        }

        public async Task<LoginResponseDto> RefreshToken(string accessToken, string refreshToken)
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

                return new LoginResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    User = _mapper.Map<UserDto>(user)
                };

            } catch
            {
                throw;
            } finally
            {
                await _unitOfWork.CloseAsync();
            }

        }
    }
}
