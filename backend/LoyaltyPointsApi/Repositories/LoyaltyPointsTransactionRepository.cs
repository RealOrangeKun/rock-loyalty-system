using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Data;
using LoyaltyPointsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyPointsApi.Repositories
{
    public class LoyaltyPointsTransactionRepository(LoyaltyDbContext dbContext) : ILoyaltyPointsTransactionRepository
    {
        public async Task<LoyaltyPoints> AddLoyaltyPointsTransaction(LoyaltyPoints loyaltyPointsTransaction)
        {
             await dbContext.LoyaltyPoints.AddAsync(loyaltyPointsTransaction);
             await dbContext.SaveChangesAsync();
             return loyaltyPointsTransaction;
        }

        public async Task<LoyaltyPoints?> GetLoyaltyPointsTransaction(LoyaltyPoints loyaltyPointsTransaction)
        {
           return await dbContext.LoyaltyPoints.FirstOrDefaultAsync(r => r.TransactionId == loyaltyPointsTransaction.TransactionId);

        }

        public async Task<List<LoyaltyPoints>> GetLoyaltyPointsTransactions(LoyaltyPoints loyaltyPointsTransaction)
        {
            return await dbContext.LoyaltyPoints.Where(r => r.CustomerId== loyaltyPointsTransaction.CustomerId && r.RestaurantId == loyaltyPointsTransaction.RestaurantId).ToListAsync();
        }
    }
}