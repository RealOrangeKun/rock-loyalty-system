using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Data;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.Repositories;
using LoyaltyPointsApi.RequestModels;
using LoyaltyPointsApi.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace LoyaltyPointsApi.Services
{
    public class LoyaltyPointsTransactionService(ILoyaltyPointsTransactionRepository loyaltyPointsTransactionRepository,
    IRestaurantService restaurantService,
    IThresholdService thresholdService,
    LoyaltyDbContext dbContext,
    ILogger<LoyaltyPointsTransactionService> logger) : ILoyaltyPointsTransactionService
    {
        public async Task<LoyaltyPoints> AddLoyaltyPointsTransaction(LoyaltyPointsTransactionRequestModel loyaltyPointsRequestModel)
        {
            logger.LogInformation("Adding LoyaltyPointsTransaction: {customerId} for restaurant: {restaurantId}", loyaltyPointsRequestModel.CustomerId, loyaltyPointsRequestModel.RestaurantId);
            var restaurant = await restaurantService.GetRestaurant(loyaltyPointsRequestModel.RestaurantId) ?? throw new Exception("Restaurant not found");
            logger.LogTrace("Restaurant found: {restaurant}", restaurant);
            LoyaltyPoints loyaltyPoints = new()
            {
                CustomerId = loyaltyPointsRequestModel.CustomerId,
                RestaurantId = loyaltyPointsRequestModel.RestaurantId,
                TransactionDate = loyaltyPointsRequestModel.TransactionDate,
                ExpiryDate = loyaltyPointsRequestModel.TransactionDate.AddDays(restaurant.PointsLifeTime),
                Restaurant = loyaltyPointsRequestModel.Restaurant,
                ReceiptId = loyaltyPointsRequestModel.ReceiptId,
            };

            return await loyaltyPointsTransactionRepository.AddLoyaltyPointsTransaction(loyaltyPoints);
        }



        public async Task<LoyaltyPoints?> GetLoyaltyPointsTransaction(int transactionId)
        {
            logger.LogInformation("Getting LoyaltyPointsTransaction: {transactionId}", transactionId);
            LoyaltyPoints loyaltyPoints = new()
            {
                TransactionId = transactionId,
            };

            return await loyaltyPointsTransactionRepository.GetLoyaltyPointsTransaction(loyaltyPoints);
        }

        public async Task<List<LoyaltyPoints>> GetLoyaltyPointsTransactions(int customerId, int restaurantId)
        {
            logger.LogInformation("Getting LoyaltyPointsTransactions: {customerId} for restaurant: {restaurantId}", customerId, restaurantId);
            LoyaltyPoints loyaltyPoints = new()
            {
                CustomerId = customerId,
                RestaurantId = restaurantId,
            };

            return await loyaltyPointsTransactionRepository.GetLoyaltyPointsTransactions(loyaltyPoints);
        }

        public async Task<int> GetTotalPoints(int customerId, int restaurantId)
        {
            logger.LogInformation("Getting total points: {customerId} for restaurant: {restaurantId}", customerId, restaurantId);
            LoyaltyPoints loyaltyPoints = new()
            {
                CustomerId = customerId,
                RestaurantId = restaurantId,

            };
            List<LoyaltyPoints> loyaltyPointsList = await loyaltyPointsTransactionRepository.GetLoyaltyPointsTransactions(loyaltyPoints);
            int totalPoints = loyaltyPointsList.Sum(l => l.Points);
            return totalPoints;
        }
    }
}