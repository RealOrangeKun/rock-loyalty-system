using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Data;
using LoyaltyPointsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyPointsApi.Repositories
{
    public class ApiKeyRepository(LoyaltyDbContext dbContext) :  IApiKeyRepository
    {
        public async Task<ApiKey> CreateApiKey(ApiKey key)
        {
            await dbContext.ApiKeys.AddAsync(key);
            await dbContext.SaveChangesAsync();
            return key;
        }

        public async Task<ApiKey?> GetApiKeyByKey(ApiKey key)
        {
            return await dbContext.ApiKeys.Where(k => k.Key == key.Key).FirstOrDefaultAsync();
        }

        public Task<ApiKey?> GetApiKeyByRestaurantId(ApiKey key)
        {
            return dbContext.ApiKeys.Where(k => k.RestaurantId == key.RestaurantId).FirstOrDefaultAsync();
        }
    }
}