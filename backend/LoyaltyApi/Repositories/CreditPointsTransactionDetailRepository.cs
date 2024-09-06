using LoyaltyApi.Data;
using LoyaltyApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyApi.Repositories;

public class CreditPointsTransactionDetailRepository(RockDbContext dbContext) : ICreditPointsTransactionDetailRepository
{
    public async Task<CreditPointsTransactionDetail?> GetTransactionDetailByIdAsync(int transactionDetailId)
    {
        return await dbContext.CreditPointsTransactionsDetails
            .FirstOrDefaultAsync(t => t.DetailId == transactionDetailId);
    }

    public async Task<CreditPointsTransactionDetail?> GetTransactionDetailByTransactionIdAsync(int transactionId)
    {
        return await dbContext.CreditPointsTransactionsDetails
            .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
    }

    public async Task AddTransactionDetailAsync(CreditPointsTransactionDetail transactionDetail)
    {
        await dbContext.CreditPointsTransactionsDetails.AddAsync(transactionDetail);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddTransactionDetailsAsync(List<CreditPointsTransactionDetail> details)
    {
        await dbContext.CreditPointsTransactionsDetails.AddRangeAsync(details);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateTransactionDetailAsync(CreditPointsTransactionDetail transactionDetail)
    {
        dbContext.CreditPointsTransactionsDetails.Update(transactionDetail);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteTransactionDetailAsync(int transactionDetailId)
    {
        var transactionDetail = await GetTransactionDetailByIdAsync(transactionDetailId);

        if (transactionDetail is not null)
        {
            dbContext.CreditPointsTransactionsDetails.Remove(transactionDetail);
            await dbContext.SaveChangesAsync();
        }
    }
    public async Task<int> GetTotalPointsSpentForEarnTransaction(int earnTransactionId)
    {
        // Sum up all points used from the specified earn transaction
        return await dbContext.CreditPointsTransactionsDetails
            .Where(detail => detail.EarnTransactionId == earnTransactionId)
            .SumAsync(detail => detail.PointsUsed);
    }
}