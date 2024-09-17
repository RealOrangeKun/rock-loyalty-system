using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Data;
using LoyaltyPointsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyPointsApi.Repositories
{
    public class ThresholdRepository(LoyaltyDbContext dbContext) : IThresholdRepository
    {
        public Task AddThreshold(Threshold threshold)
        {
            dbContext.Thresholds.Add(threshold);
            return dbContext.SaveChangesAsync();
        }

        public async Task<Threshold?> GetRestaurantThreshold(Threshold threshold)
        {
            return await dbContext.Thresholds.FirstOrDefaultAsync(r => r.RestaurantId == threshold.RestaurantId && r.ThresholdName == threshold.ThresholdName);
            
        }

        public async Task<List<Threshold>> GetRestaurantThresholds(Threshold threshold)
        {
            return await dbContext.Thresholds.Where(r => r.RestaurantId == threshold.RestaurantId).ToListAsync();
            
        }

        public Task UpdateThreshold(Threshold threshold)
        {
            dbContext.Thresholds.Update(threshold);
            return dbContext.SaveChangesAsync();
        }
    }
}