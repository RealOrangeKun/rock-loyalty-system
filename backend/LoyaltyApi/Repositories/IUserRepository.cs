
using LoyaltyApi.Models;

namespace LoyaltyApi.Repositories
{
    public interface IUserRepository
    {
        Task<User?> CreateUserAsync(User user);

        Task<User?> GetUserAsync(User user);

    }
}