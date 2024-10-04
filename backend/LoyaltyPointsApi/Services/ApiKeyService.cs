using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.Repositories;
using LoyaltyPointsApi.Utilities;

namespace LoyaltyPointsApi.Services
{
    public class ApiKeyService(IApiKeyRepository repository,
    ILogger<ApiKeyService> logger) : IApiKeyService
    {
        public async Task<ApiKey> CreateApiKey(int restaurantId)
        {
            logger.LogInformation("Creating ApiKey for restaurantId: {restaurantId}", restaurantId);
            var key = ApiKeyUtility.GenerateApiKey();
            logger.LogTrace("Generated ApiKey: {key}", key);
            ApiKey apiKey = new()
            {
                Key = key,
                RestaurantId = restaurantId
            };

            return await repository.GetApiKeyByRestaurantId(apiKey) ?? await repository.CreateApiKey(apiKey);
        }

        public async Task<ApiKey?> GetApiKey(string key)
        {
            logger.LogInformation("Getting ApiKey for key: {key}", key);
            ApiKey apiKey = new()
            {
                Key = key,
            };

            return await repository.GetApiKeyByKey(apiKey);
        }
    }
}