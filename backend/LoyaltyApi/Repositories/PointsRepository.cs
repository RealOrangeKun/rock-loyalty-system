using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyApi.Data;
using LoyaltyApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyApi.Repositories
{
    public class PointsRepository(RockDbContext dbContext) : IPointsRepository
    {
        public async Task<Points> CreatePointsAsync(Points points)
        {
            await dbContext.Points.AddAsync(points);
            await dbContext.SaveChangesAsync();
            return points;
        }

        public async Task<List<Points>> GetTotalPointsAsync(int customerId, int restaurantId)
        {
            return await dbContext.Points.Where(p => p.CustomerId == customerId && p.RestaurantId == restaurantId).ToListAsync();

        }
        public async Task<Points?> GetPointsRecordAsync(int customerId, int restaurantId, int transactionId)
        {
            return await dbContext.Points.FirstOrDefaultAsync(p => p.CustomerId == customerId && p.RestaurantId == restaurantId && p.TransactionId == transactionId);
        }

        public async Task<Points> UpdatePointsAsync(Points points)
        {
            dbContext.Points.Attach(points);

            dbContext.Entry(points).Property(r => r.CreditPoints).IsModified = true;

            await dbContext.SaveChangesAsync();
            return points;
        }
    }
}