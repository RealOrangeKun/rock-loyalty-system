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
        public async Task AddThreshold(Threshold threshold)
        {
            await dbContext.Thresholds.AddAsync(threshold);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Threshold?> GetRestaurantThreshold(Threshold threshold)
        {
            return await dbContext.Thresholds
                .Include(t => t.Promotions)
                .FirstOrDefaultAsync(r =>
                    r.RestaurantId == threshold.RestaurantId && r.ThresholdId == threshold.ThresholdId);
        }

        public async Task<List<Threshold>> GetRestaurantThresholds(Threshold threshold)
        {
            return await dbContext.Thresholds
                .Include(t => t.Promotions)
                .Where(r => r.RestaurantId == threshold.RestaurantId)
                .ToListAsync();
        }

        public Task UpdateThreshold(Threshold threshold)
        {
            dbContext.Thresholds.Update(threshold);
            return dbContext.SaveChangesAsync();
        }
    }
}