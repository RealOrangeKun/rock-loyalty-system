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
    public class LoyaltyPointsTransactionService(ILoyaltyPointsTransactionRepository loyaltyPointsTransactionRepository , IRestaurantService restaurantService , IThresholdService thresholdService) : ILoyaltyPointsTransactionService
    {
        public async Task<LoyaltyPoints> AddLoyaltyPointsTransaction(LoyaltyPointsTransactionRequestModel loyaltyPointsRequestModel)
        {
            var restaurant = await restaurantService.GetRestaurant(loyaltyPointsRequestModel.RestaurantId) ?? throw new Exception("Restaurant not found");
            LoyaltyPoints loyaltyPoints = new(){
                CustomerId = loyaltyPointsRequestModel.CustomerId,
                RestaurantId = loyaltyPointsRequestModel.RestaurantId,
                TransactionDate = loyaltyPointsRequestModel.TransactionDate,
                ExpiryDate = loyaltyPointsRequestModel.TransactionDate.AddDays(restaurant.PointsLifeTime),
                Restaurant = loyaltyPointsRequestModel.Restaurant,
                ReceiptId = loyaltyPointsRequestModel.ReceiptId,
            };

            return await loyaltyPointsTransactionRepository.AddLoyaltyPointsTransaction(loyaltyPoints);
        }

        public async Task<List<User>> Dika(int restaurantId, int minPoints, int maxPoints)
        {
            string query = $"SUM(Points) >= {minPoints} AND SUM(Points) <= {maxPoints}";
            var result = 
        }

        public async Task<LoyaltyPoints?> GetLoyaltyPointsTransaction(int transactionId)
        {
            LoyaltyPoints loyaltyPoints = new(){
                TransactionId = transactionId,
            };

            return await loyaltyPointsTransactionRepository.GetLoyaltyPointsTransaction(loyaltyPoints);
        }

        public async Task<List<LoyaltyPoints>> GetLoyaltyPointsTransactions(int customerId , int restaurantId)
        {
            LoyaltyPoints loyaltyPoints = new(){
                CustomerId = customerId,
                RestaurantId = restaurantId,
            };

            return await loyaltyPointsTransactionRepository.GetLoyaltyPointsTransactions(loyaltyPoints);
        }

        public async Task<int> GetTotalPoints(int customerId , int restaurantId)
        {
            LoyaltyPoints loyaltyPoints = new(){
                CustomerId = customerId,
                RestaurantId =restaurantId,
                
        };
        List<LoyaltyPoints> loyaltyPointsList = await loyaltyPointsTransactionRepository.GetLoyaltyPointsTransactions(loyaltyPoints);
        int totalPoints = loyaltyPointsList.Sum(l => l.Points);
        return totalPoints;
    }


}
}