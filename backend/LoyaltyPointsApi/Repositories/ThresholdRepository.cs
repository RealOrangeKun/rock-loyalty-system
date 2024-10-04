using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Data;
using LoyaltyPointsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyPointsApi.Repositories
{
    public class ThresholdRepository(LoyaltyDbContext dbContext,
    ILogger<ThresholdRepository> logger) : IThresholdRepository
    {
        public async Task<Threshold> AddThreshold(Threshold threshold)
        {
            logger.LogInformation("Adding Threshold: {threshold} for restaurant: {restaurantId}", threshold.ThresholdId, threshold.RestaurantId);
            await dbContext.Thresholds.AddAsync(threshold);
            await dbContext.SaveChangesAsync();
            return threshold;
        }

        public async Task DeleteThreshold(Threshold threshold)
        {
            logger.LogInformation("Deleting Threshold: {threshold} for restaurant: {restaurantId}", threshold.ThresholdId, threshold.RestaurantId);
            dbContext.Thresholds.Remove(threshold);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Threshold?> GetRestaurantThreshold(Threshold threshold)
        {
            logger.LogInformation("Getting Threshold: {threshold} for restaurant: {restaurantId}", threshold.ThresholdId, threshold.RestaurantId);
            return await dbContext.Thresholds
                .Include(t => t.Promotions)
                .FirstOrDefaultAsync(r =>
                    r.RestaurantId == threshold.RestaurantId && r.ThresholdId == threshold.ThresholdId);
        }

        public async Task<List<Threshold>> GetRestaurantThresholds(Threshold threshold)
        {
            logger.LogInformation("Getting Thresholds for restaurant: {restaurantId}", threshold.RestaurantId);
            return await dbContext.Thresholds
                .Include(t => t.Promotions)
                .Where(r => r.RestaurantId == threshold.RestaurantId)
                .ToListAsync()!;
        }

        public async Task<Threshold> UpdateThreshold(Threshold threshold)
        {
            logger.LogInformation("Updating Threshold: {threshold} for restaurant: {restaurantId}", threshold.ThresholdId, threshold.RestaurantId);
            dbContext.Thresholds.Update(threshold);
            await dbContext.SaveChangesAsync();
            return threshold;
        }
    }
}