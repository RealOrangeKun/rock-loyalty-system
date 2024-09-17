using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;

namespace LoyaltyPointsApi.Services
{
    public interface IApiKeyService
    {
        public Task<ApiKey> CreateApiKey(int restaurantId);

        public Task<ApiKey?> GetApiKey(string key);
    }
}