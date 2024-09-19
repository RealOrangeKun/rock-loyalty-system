using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.Repositories;
using LoyaltyPointsApi.RequestModels;
using LoyaltyPointsApi.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace LoyaltyPointsApi.Services
{
    public class LoyaltyPointsTransactionService(LoyaltyPointsTransactionRepositroy loyaltyPointsTransactionRepositroy) : ILoyaltyPointsTransactionService
    {
        public Task AddLoyaltyPointsTransaction(LoyaltyPointsTransactionRequestModel loyaltyPointsRequestModel)
        {
            LoyaltyPoints loyaltyPoints = new(){
                CustomerId = loyaltyPointsRequestModel.CustomerId,
                RestaurantId = loyaltyPointsRequestModel.RestaurantId,
                TransactionId = loyaltyPointsRequestModel.TransactionId,
                Points = loyaltyPointsRequestModel.Points,
                IsExpired = loyaltyPointsRequestModel.IsExpired,
                TransactionDate = loyaltyPointsRequestModel.TransactionDate,
                Restaurant = loyaltyPointsRequestModel.Restaurant,
                ReceiptId = loyaltyPointsRequestModel.ReceiptId,
            };

            return loyaltyPointsTransactionRepositroy.AddLoyaltyPointsTransaction(loyaltyPoints);
        }

        public Task<LoyaltyPoints?> GetLoyaltyPointsTransaction(LoyaltyPointsTransactionRequestModel loyaltyPointsRequestModel)
        {
            LoyaltyPoints loyaltyPoints = new(){
                TransactionId = loyaltyPointsRequestModel.TransactionId,
                CustomerId = loyaltyPointsRequestModel.CustomerId,
                RestaurantId = loyaltyPointsRequestModel.RestaurantId,
                ReceiptId = loyaltyPointsRequestModel.ReceiptId,
                Points = loyaltyPointsRequestModel.Points,
            };

            return loyaltyPointsTransactionRepositroy.GetLoyaltyPointsTransaction(loyaltyPoints);
        }

        public Task<List<LoyaltyPoints>> GetLoyaltyPointsTransactions(LoyaltyPointsTransactionRequestModel loyaltyPointsRequestModel)
        {
            LoyaltyPoints loyaltyPoints = new(){
                TransactionId = loyaltyPointsRequestModel.TransactionId,
                CustomerId = loyaltyPointsRequestModel.CustomerId,
                RestaurantId = loyaltyPointsRequestModel.RestaurantId,
                ReceiptId = loyaltyPointsRequestModel.ReceiptId,
                Points = loyaltyPointsRequestModel.Points,
            };

            return loyaltyPointsTransactionRepositroy.GetLoyaltyPointsTransactions(loyaltyPoints);
        }

        public async Task<int> GetTotalPoints(LoyaltyPointsTransactionRequestModel loyaltyPointsRequestModel)
        {
            LoyaltyPoints loyaltyPoints = new(){
                TransactionId = loyaltyPointsRequestModel.TransactionId,
                CustomerId = loyaltyPointsRequestModel.CustomerId,
                RestaurantId = loyaltyPointsRequestModel.RestaurantId,
                ReceiptId = loyaltyPointsRequestModel.ReceiptId,
                Points = loyaltyPointsRequestModel.Points,
        };
        List<LoyaltyPoints> loyaltyPointsList = await loyaltyPointsTransactionRepositroy.GetLoyaltyPointsTransactions(loyaltyPoints);
        int totalPoints = loyaltyPointsList.Sum(l => l.Points);
        return totalPoints;
    }


}
}