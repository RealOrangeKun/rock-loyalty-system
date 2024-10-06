using LoyaltyPointsApi.Data;
using LoyaltyPointsApi.Models;
using Microsoft.EntityFrameworkCore;
using X.PagedList;



namespace LoyaltyPointsApi.Repositories
{
    public class LoyaltyPointsTransactionRepository(
        LoyaltyDbContext dbContext,
        ILogger<LoyaltyPointsTransactionRepository> logger) : ILoyaltyPointsTransactionRepository
    {
        public async Task<LoyaltyPoints> AddLoyaltyPointsTransaction(LoyaltyPoints loyaltyPointsTransaction)
        {
            logger.LogInformation("Adding LoyaltyPointsTransaction: {loyaltyPointsTransaction}",
                loyaltyPointsTransaction);
            await dbContext.LoyaltyPoints.AddAsync(loyaltyPointsTransaction);
            await dbContext.SaveChangesAsync();
            return loyaltyPointsTransaction;
        }

        public async Task<LoyaltyPoints?> GetLoyaltyPointsTransaction(LoyaltyPoints loyaltyPointsTransaction)
        {
            logger.LogInformation("Getting LoyaltyPointsTransaction: {loyaltyPointsTransaction}",
                loyaltyPointsTransaction);
            if (loyaltyPointsTransaction.ReceiptId != 0)
                return await dbContext.LoyaltyPoints.FirstOrDefaultAsync(r =>
                    r.ReceiptId == loyaltyPointsTransaction.ReceiptId);
            return await dbContext.LoyaltyPoints.FirstOrDefaultAsync(r =>
                r.TransactionId == loyaltyPointsTransaction.TransactionId);
        }

        public async Task<List<LoyaltyPoints>> GetLoyaltyPointsTransactions(LoyaltyPoints loyaltyPointsTransaction)
        {
            logger.LogInformation("Getting LoyaltyPointsTransactions: {loyaltyPointsTransaction}",
                loyaltyPointsTransaction);
            return await dbContext.LoyaltyPoints.Where(r =>
                r.CustomerId == loyaltyPointsTransaction.CustomerId &&
                r.RestaurantId == loyaltyPointsTransaction.RestaurantId).ToListAsync();
        }

        public async Task<IPagedList<LoyaltyPoints>> GetPagedLoyaltyPointsTransactions(
            LoyaltyPoints loyaltyPointsTransaction, int pageNumber, int pageSize)
        {
            logger.LogInformation("Getting LoyaltyPointsTransactions: {loyaltyPointsTransaction}",
                loyaltyPointsTransaction);
            var transactions = await dbContext.LoyaltyPoints.Where(r =>
                r.CustomerId == loyaltyPointsTransaction.CustomerId &&
                r.RestaurantId == loyaltyPointsTransaction.RestaurantId).ToListAsync();
            
            return transactions.ToPagedList(pageNumber, pageSize);
        }

        public async Task<List<int>> GetCustomersByRestaurantAndPointsRange(int restaurantId, int? minPoints,
            int? maxPoints)
        {
            if (maxPoints != null)
            {
                var userIds = await dbContext.LoyaltyPoints
                    .Where(lp => lp.RestaurantId == restaurantId)
                    .GroupBy(lp => lp.CustomerId)
                    .Where(g => g.Sum(lp => lp.Points) >= minPoints && g.Sum(lp => lp.Points) <= maxPoints)
                    .Select(g => g.Key)
                    .ToListAsync();
                return userIds;
            }
            else
            {
                var userIds = dbContext.LoyaltyPoints
                    .Where(lp => lp.RestaurantId == restaurantId)
                    .GroupBy(lp => lp.CustomerId)
                    .Where(g => g.Sum(lp => lp.Points) >= minPoints)
                    .Select(g => g.Key)
                    .ToList();
                return userIds;
            }
        }
    }
}