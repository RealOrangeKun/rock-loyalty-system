﻿using LoyaltyApi.Data;
using LoyaltyApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyApi.Repositories;

public class CreditPointsTransactionDetailRepository(RockDbContext dbContext,
ILogger<CreditPointsTransactionDetailRepository> logger) : ICreditPointsTransactionDetailRepository
{
    public async Task<CreditPointsTransactionDetail?> GetTransactionDetailByIdAsync(int transactionDetailId)
    {
        logger.LogInformation("Getting transaction detail {TransactionDetailId}", transactionDetailId);
        return await dbContext.CreditPointsTransactionsDetails
            .FirstOrDefaultAsync(t => t.DetailId == transactionDetailId);
    }

    public async Task<CreditPointsTransactionDetail?> GetTransactionDetailByTransactionIdAsync(int transactionId)
    {
        logger.LogInformation("Getting transaction detail for transaction {TransactionId}", transactionId);
        return await dbContext.CreditPointsTransactionsDetails
            .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
    }

    public async Task AddTransactionDetailAsync(CreditPointsTransactionDetail transactionDetail)
    {
        await dbContext.CreditPointsTransactionsDetails.AddAsync(transactionDetail);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Transaction detail {DetailId} created successfully", transactionDetail.DetailId);
    }

    public async Task AddTransactionDetailsAsync(List<CreditPointsTransactionDetail> details)
    {
        await dbContext.CreditPointsTransactionsDetails.AddRangeAsync(details);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Transaction details created successfully");
    }

    public async Task UpdateTransactionDetailAsync(CreditPointsTransactionDetail transactionDetail)
    {
        dbContext.CreditPointsTransactionsDetails.Update(transactionDetail);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Transaction detail {DetailId} updated successfully", transactionDetail.DetailId);
    }

    public async Task DeleteTransactionDetailAsync(int transactionDetailId)
    {
        var transactionDetail = await GetTransactionDetailByIdAsync(transactionDetailId);

        if (transactionDetail is not null)
        {
            dbContext.CreditPointsTransactionsDetails.Remove(transactionDetail);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Transaction detail {DetailId} deleted successfully", transactionDetail.DetailId);
        }

    }
    public async Task<int> GetTotalPointsSpentForEarnTransaction(int earnTransactionId)
    {
        logger.LogInformation("Getting total points spent for earn transaction {EarnTransactionId}", earnTransactionId);
        // Sum up all points used from the specified earn transaction
        return await dbContext.CreditPointsTransactionsDetails
            .Where(detail => detail.EarnTransactionId == earnTransactionId)
            .SumAsync(detail => detail.PointsUsed);

    }
}