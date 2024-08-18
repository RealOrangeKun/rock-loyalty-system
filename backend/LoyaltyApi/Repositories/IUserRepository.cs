
using LoyaltyApi.Models;

namespace LoyaltyApi.Repositories
{
    public interface IUserRepository
    {
        Task<object> CreateUserAsync(User user);

        Task GetUserAsync(int userId, int restaurantId);

    }
}