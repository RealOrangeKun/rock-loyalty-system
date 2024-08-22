
using LoyaltyApi.Models;

namespace LoyaltyApi.Repositories
{
    public interface IUserRepository
    {
        Task<object> CreateUserAsync(User user);

        Task<User> GetUserAsync(string? email, string? phoneNumber, int restaurantId);

    }
}