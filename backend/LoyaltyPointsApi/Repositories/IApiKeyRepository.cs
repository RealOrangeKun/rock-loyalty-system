using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;

namespace LoyaltyPointsApi.Repositories
{
    public interface IApiKeyRepository
    {
        public Task<ApiKey?> GetApiKeyByKey(ApiKey key);
        public Task<ApiKey?> GetApiKeyByRestaurantId(ApiKey key);

        public Task<ApiKey> CreateApiKey(ApiKey key);

    }
}