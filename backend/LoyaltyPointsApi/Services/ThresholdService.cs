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
        public async Task AddThreshold(ThresholdRequestModel thresholdRequest)
        {
            Threshold newThreshold = new(){
                RestaurantId = thresholdRequest.RestaurantId,
                ThresholdName = thresholdRequest.ThresholdName,
                MinimumPoints = thresholdRequest.MinimumPoints
            };
            await thresholdRepository.AddThreshold(newThreshold);
        }

        public async Task<Threshold?> GetRestaurantThreshold(int restaurantId, int thresholdId)
        {
            Threshold threshold = new(){
                RestaurantId = restaurantId,
                ThresholdId = thresholdId,
            };

            
            return await thresholdRepository.GetRestaurantThreshold(threshold);
        }

        public async Task<List<Threshold>?> GetRestaurantThresholds(int restaurantId)
        {
            Threshold threshold = new (){
                RestaurantId = restaurantId
            };

            return await thresholdRepository.GetRestaurantThresholds(threshold);
        }

        public async Task<Threshold?> UpdateThreshold(ThresholdRequestModel thresholdRequest , int restaurantId, int thresholdId)
        {
            Threshold threshold = new(){
                RestaurantId = restaurantId,
                ThresholdId = thresholdId

            };
            Threshold updatedThreshold = await thresholdRepository.GetRestaurantThreshold(threshold);
            updatedThreshold.MinimumPoints = thresholdRequest.MinimumPoints;
            updatedThreshold.ThresholdName = thresholdRequest.ThresholdName;
            await thresholdRepository.UpdateThreshold(updatedThreshold);
            return updatedThreshold;
        }
    }
}