using LoyaltyApi.Data;
using LoyaltyApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyApi.Repositories;

public class CreditPointsTransactionRepository(RockDbContext dbContext) : ICreditPointsTransactionRepository
{
    public async Task<CreditPointsTransaction?> GetTransactionByIdAsync(int transactionId)
    {
        return await dbContext.CreditPointsTransactions
            .Include(t => t.CreditPointsTransactionDetails) 
            .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
    }

    public async Task<CreditPointsTransaction?> GetTransactionByReceiptIdAsync(int receiptId)
    {
        return await dbContext.CreditPointsTransactions
            .Include(t => t.CreditPointsTransactionDetails) 
            .FirstOrDefaultAsync(t => t.ReceiptId == receiptId);
    }

    public async Task<IEnumerable<CreditPointsTransaction>> GetTransactionsByCustomerAndRestaurantAsync(int customerId, int restaurantId)
    {
        return await dbContext.CreditPointsTransactions
            .Where(t => t.CustomerId == customerId && t.RestaurantId == restaurantId)
            .ToListAsync();
    }

    public async Task AddTransactionAsync(CreditPointsTransaction transaction)
    {
        await dbContext.CreditPointsTransactions.AddAsync(transaction);
        await dbContext.SaveChangesAsync();
    }
    public async Task AddTransactionsAsync(List<CreditPointsTransaction> transactions)
    {
        await dbContext.CreditPointsTransactions.AddRangeAsync(transactions);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateTransactionAsync(CreditPointsTransaction transaction)
    {
        dbContext.CreditPointsTransactions.Update(transaction);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteTransactionAsync(int transactionId)
    {
        var transaction = await GetTransactionByIdAsync(transactionId);
        if (transaction is not null)
        {
            dbContext.CreditPointsTransactions.Remove(transaction);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<int> GetCustomerPointsAsync(int customerId, int restaurantId)
    {
        return await dbContext.CreditPointsTransactions
            .Where(t => t.CustomerId == customerId && t.RestaurantId == restaurantId)
            .SumAsync(t => t.Points);
    }
    
    public async Task<IEnumerable<CreditPointsTransaction>> GetExpiredTransactionsAsync(Dictionary<int, int> restaurantMap, DateTime currentDate)
    {
        // Use SQL query to get all transactions that have expired based on the restaurant's lifetime
        var query = from transaction in dbContext.CreditPointsTransactions
            join restaurant in dbContext.Restaurants
                on transaction.RestaurantId equals restaurant.RestaurantId
            where transaction.TransactionType == TransactionType.Earn &&
                  transaction.Points > 0 &&
                  transaction.TransactionDate < currentDate.AddDays(-restaurant.CreditPointsLifeTime)
            select transaction;

        return await query.ToListAsync();
    }
}