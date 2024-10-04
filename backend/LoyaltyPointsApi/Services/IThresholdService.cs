using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.RequestModels;

namespace LoyaltyPointsApi.Services
{
    public interface IThresholdService
    {
        public Task<List<Threshold>?> GetRestaurantThresholds(int restaurantId);

        public Task<Threshold?> GetRestaurantThreshold(int restaurantId, int thresholdId);

        public Task<Threshold> AddThreshold(ThresholdRequestModel threshold);

        public Task<Threshold?> UpdateThreshold(ThresholdRequestModel threshold, int restaurantId, int thresholdId);

        public Task DeleteThreshold(int thresholdId);

        public Task<List<int?>> GetThresholdBoundaries(int thresholdId, int restaurantId);

    }


}