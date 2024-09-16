using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.Repositories;
using LoyaltyPointsApi.Utilities;

namespace LoyaltyPointsApi.Services
{
    public class ApiKeyService(IApiKeyRepository repository) : IApiKeyService
    {
        public async Task<ApiKey> CreateApiKey(int restaurantId)
        {
            var key = ApiKeyUtility.GenerateApiKey();
            ApiKey apiKey = new()
            {
                Key = key,
                RestaurantId = restaurantId
            };

            return await repository.GetApiKeyByRestaurantId(apiKey) ?? await repository.CreateApiKey(apiKey);
        }

        public async Task<ApiKey?> GetApiKey(string key)
        {
            ApiKey apiKey = new()
            {
                Key = key,
            };

            return await repository.GetApiKeyByKey(apiKey);
        }
    }
}