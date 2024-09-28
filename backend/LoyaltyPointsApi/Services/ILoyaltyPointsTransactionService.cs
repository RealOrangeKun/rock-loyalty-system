using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.RequestModels;

namespace LoyaltyPointsApi.Services
{
    public interface ILoyaltyPointsTransactionService
    {
        public Task<LoyaltyPoints?> GetLoyaltyPointsTransaction(int transactionId);

        public Task<LoyaltyPoints> AddLoyaltyPointsTransaction(LoyaltyPointsTransactionRequestModel loyaltyPointsRequestModel);

        public Task<List<LoyaltyPoints>> GetLoyaltyPointsTransactions(int customerId , int restaurantId);

        public Task<int> GetTotalPoints(int customerId , int restaurantId);

        public Task<List<User>> Dika(int restaurantId , int minPoints , int maxPoints);
    }
}