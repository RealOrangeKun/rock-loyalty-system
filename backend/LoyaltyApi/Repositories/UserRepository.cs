using LoyaltyApi.Models;
using LoyaltyApi.Utilities;
using Microsoft.AspNetCore.Identity;

namespace LoyaltyApi.Repositories
{
    public class UserRepository(ApiUtility apiUtility,
    ILogger<UserRepository> logger) : IUserRepository
    {
        public async Task<User?> CreateUserAsync(User user)
        {
            var apiKey = await apiUtility.GetApiKey(user.RestaurantId.ToString());
            logger.LogInformation("Creating user {Email} for restaurant {RestaurantId}", user.Email, user.RestaurantId);
            return await apiUtility.CreateUserAsync(user, apiKey);
        }

        public async Task<User?> GetUserAsync(User user)
        {
            var apiKey = await apiUtility.GetApiKey(user.RestaurantId.ToString());
            logger.LogInformation("Getting user {Email} for restaurant {RestaurantId}", user.Email ?? user.Id.ToString() ?? "", user.RestaurantId);
            return await apiUtility.GetUserAsync(user, apiKey);

        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var apiKey = await apiUtility.GetApiKey(user.RestaurantId.ToString());
            logger.LogInformation("Updating user {Email} for restaurant {RestaurantId}", user.Email, user.RestaurantId);
            return await apiUtility.UpdateUserAsync(user, apiKey);
        }
    }
}