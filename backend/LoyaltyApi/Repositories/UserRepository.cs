using LoyaltyApi.Models;
using LoyaltyApi.Utilities;
using Microsoft.AspNetCore.Identity;

namespace LoyaltyApi.Repositories
{
    public class UserRepository(ApiUtility apiUtility , IPasswordHasher<User> passwordHasher) : IUserRepository
    {
        public async Task<User?> CreateUserAsync(User user)
        {
            user.Password = passwordHasher.HashPassword(user, user.Password);
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