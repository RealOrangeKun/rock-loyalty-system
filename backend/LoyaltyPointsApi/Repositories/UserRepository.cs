using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Data;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.Utilities;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyPointsApi.Repositories
{
    public class UserRepository(ApiUtility apiUtility,
    ILogger<LoyaltyPointsTransactionRepository> logger) : IUserRepository
    {

        public async Task<User?> GetUser(User user)
        {
            logger.LogInformation("Getting User: {customerId} for restaurant: {restaurantId}", user.CustomerId, user.RestaurantId);
            var apiKey = await apiUtility.GetApiKey(user.RestaurantId.ToString());
            return await apiUtility.GetUserAsync(user, apiKey);
        }
    }


}