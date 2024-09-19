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
    public class LoyaltyPointsTransactionService(LoyaltyPointsTransactionRepository loyaltyPointsTransactionRepositroy) : ILoyaltyPointsTransactionService
    {
        public async Task<LoyaltyPoints> AddLoyaltyPointsTransaction(LoyaltyPointsTransactionRequestModel loyaltyPointsRequestModel)
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

            return await loyaltyPointsTransactionRepositroy.AddLoyaltyPointsTransaction(loyaltyPoints);
        }

        public async Task<LoyaltyPoints?> GetLoyaltyPointsTransaction(int transactionId)
        {
            LoyaltyPoints loyaltyPoints = new(){
                TransactionId = transactionId,
            };

            return await loyaltyPointsTransactionRepositroy.GetLoyaltyPointsTransaction(loyaltyPoints);
        }

        public async Task<List<LoyaltyPoints>> GetLoyaltyPointsTransactions(int customerId , int restaurantId)
        {
            LoyaltyPoints loyaltyPoints = new(){
                CustomerId = customerId,
                RestaurantId = restaurantId,
            };

            return await loyaltyPointsTransactionRepositroy.GetLoyaltyPointsTransactions(loyaltyPoints);
        }

        public async Task<int> GetTotalPoints(int customerId , int restaurantId)
        {
            LoyaltyPoints loyaltyPoints = new(){
                CustomerId = customerId,
                RestaurantId =restaurantId,
                
        };
        List<LoyaltyPoints> loyaltyPointsList = await loyaltyPointsTransactionRepositroy.GetLoyaltyPointsTransactions(loyaltyPoints);
        int totalPoints = loyaltyPointsList.Sum(l => l.Points);
        return totalPoints;
    }


}
}