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
        Task<List<Threshold>?> GetRestaurantThresholds(int restaurantId);

        Task<Threshold?> GetRestaurantThreshold(int restaurantId, string thresholdName);

        Task AddThreshold(ThresholdRequestModel threshold , int restaurantId);

        Task<Threshold?> UpdateThreshold(ThresholdRequestModel threshold , int restaurantId, string thresholdName);
    }

   
}