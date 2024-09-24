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
    public class UserRepository(ApiUtility apiUtility) : IUserRepository
    {

        public async Task<User?> GetUser(User user)
        {
            var apiKey = await apiUtility.GetApiKey(user.RestaurantId.ToString());
            return await apiUtility.GetUserAsync(user, apiKey);
        }
    }


}