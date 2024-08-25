using LoyaltyApi.Models;
using LoyaltyApi.Utilities;

namespace LoyaltyApi.Repositories
{
    public class UserRepository(ApiUtility apiUtility) : IUserRepository
    {
        public async Task<User?> CreateUserAsync(User user)
        {
            var apiKey = await apiUtility.GetApiKey(user.RestaurantId.ToString());
            return await apiUtility.CreateUserAsync(user, apiKey);
        }

        public async Task<User?> GetUserAsync(User user)
        {
            var apiKey = await apiUtility.GetApiKey(user.RestaurantId.ToString());

            return await apiUtility.GetUserAsync(user, apiKey);

        }
    }
}