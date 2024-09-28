using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.Repositories;
using LoyaltyPointsApi.RequestModels;
using Microsoft.EntityFrameworkCore.Storage;

namespace LoyaltyPointsApi.Services
{
    public class ThresholdService(IThresholdRepository thresholdRepository) : IThresholdService
    {
        public async Task<Threshold> AddThreshold(ThresholdRequestModel thresholdRequest)
        {
            Threshold newThreshold = new()
            {
                RestaurantId = thresholdRequest.RestaurantId,
                ThresholdName = thresholdRequest.ThresholdName,
                MinimumPoints = thresholdRequest.MinimumPoints
            };
            await thresholdRepository.AddThreshold(newThreshold);
            return newThreshold;
        }

        public async Task DelteThreshold(int thresholdId)
        {
            Threshold threshold = new()
            {
                ThresholdId = thresholdId
            };
            await thresholdRepository.DeleteThreshold(threshold);
        }

        public async Task<Threshold?> GetRestaurantThreshold(int restaurantId, int thresholdId)
        {
            Threshold threshold = new()
            {
                RestaurantId = restaurantId,
                ThresholdId = thresholdId,
            };


            return await thresholdRepository.GetRestaurantThreshold(threshold);
        }

        public async Task<List<Threshold>?> GetRestaurantThresholds(int restaurantId)
        {
            Threshold threshold = new()
            {
                RestaurantId = restaurantId
            };

            return await thresholdRepository.GetRestaurantThresholds(threshold);
        }

        public async Task<Threshold?> UpdateThreshold(ThresholdRequestModel thresholdRequest, int restaurantId,
            int thresholdId)
        {
            Threshold threshold = new()
            {
                RestaurantId = restaurantId,
                ThresholdId = thresholdId
            };
            Threshold updatedThreshold = await thresholdRepository.GetRestaurantThreshold(threshold);
            updatedThreshold.MinimumPoints = thresholdRequest.MinimumPoints;
            updatedThreshold.ThresholdName = thresholdRequest.ThresholdName;
            await thresholdRepository.UpdateThreshold(updatedThreshold);
            return updatedThreshold;
        }

        public async Task<List<int?>> GetThresholdBoundaries(int thresholdId, int restaurantId)
        {
            List<int?> boundaries = [];

            List<Threshold> restaurantThresholds = await GetRestaurantThresholds(restaurantId);

            List<Threshold> sortedThresholds = restaurantThresholds.OrderBy(t => t.MinimumPoints).ToList();

            int thresholdIndex = sortedThresholds.Find(t => t.ThresholdId == thresholdId).ThresholdId;

            boundaries.Add(sortedThresholds[thresholdIndex].MinimumPoints);

            if (thresholdIndex == sortedThresholds.Count - 1) boundaries.Add(null);

            boundaries.Add(sortedThresholds[thresholdIndex + 1].MinimumPoints - 1);

            return boundaries;
        }
    }
}