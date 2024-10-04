using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Data;
using LoyaltyPointsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyPointsApi.Repositories
{
    public class ApiKeyRepository(LoyaltyDbContext dbContext,
    ILogger<ApiKeyRepository> logger) : IApiKeyRepository
    {
        public async Task<ApiKey> CreateApiKey(ApiKey key)
        {
            logger.LogInformation("Creating ApiKey for restaurantId: {restaurantId}", key.RestaurantId);
            await dbContext.ApiKeys.AddAsync(key);
            await dbContext.SaveChangesAsync();
            return key;
        }

        public async Task<ApiKey?> GetApiKeyByKey(ApiKey key)
        {
            logger.LogInformation("Getting ApiKey for key: {key}", key.Key);
            return await dbContext.ApiKeys.Where(k => k.Key == key.Key).FirstOrDefaultAsync();
        }

        public async Task<ApiKey?> GetApiKeyByRestaurantId(ApiKey key)
        {
            logger.LogInformation("Getting ApiKey for restaurantId: {restaurantId}", key.RestaurantId);
            return await dbContext.ApiKeys.Where(k => k.RestaurantId == key.RestaurantId).FirstOrDefaultAsync();
        }
    }
}