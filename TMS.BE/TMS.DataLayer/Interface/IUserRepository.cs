using TMS.DataLayer.Entity;

namespace TMS.DataLayer.Interface
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUsername(string username);
        Task<bool> Update(User user);
        Task<bool> CreateAsync(User user);
    }
}
