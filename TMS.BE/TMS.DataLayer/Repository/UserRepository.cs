
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BaseRepository;
using TMS.DataLayer.Entity;
using TMS.DataLayer.Interface;

namespace TMS.DataLayer.Repository
{
    public class UserRepository : IUserRepository
    {
        protected readonly IUnitOfWork _unitOfWork;

        public UserRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            var query = "SELECT * FROM users WHERE Username = @Username";
            var parameters = new { UserName = username};
            var result = await _unitOfWork.Connection.QueryAsync<User>(sql: query, param: parameters, transaction: _unitOfWork.Transaction);
            return result.FirstOrDefault();
        }

        public async Task<bool> Update(User user)
        {
            var query = "UPDATE users SET Username = @Username, Password = @Password, FullName = @FullName, Role = @Role, RefreshToken = @RefreshToken, RefreshTokenExp = @RefreshTokenExp WHERE UserId = @UserId;";
            var parameters = new { 
                UserId = user.UserId,
                Username = user.Username, 
                Password = user.Password,
                FullName = user.FullName,
                Role = user.Role,
                RefreshToken = user.RefreshToken,
                RefreshTokenExp = user.RefreshTokenExp,
            };
            var result = await _unitOfWork.Connection.ExecuteAsync(sql: query, param: parameters, transaction: _unitOfWork.Transaction);
            return result > 0;
        }

        public async Task<bool> CreateAsync(User user)
        {
            var query = "INSERT INTO users (UserId, Username, Password, FullName, Role) " +
                "VALUES (@UserId, @Username, @Password, @FullName, @Role);";
            var parameters = new
            {
                UserId = user.UserId,
                Username = user.Username,
                Password = user.Password,
                FullName = user.FullName,
                Role = user.Role
            };
            var result = await _unitOfWork.Connection.ExecuteAsync(sql: query, param: parameters, transaction: _unitOfWork.Transaction);
            return result > 0;
        }

        
    }
}
