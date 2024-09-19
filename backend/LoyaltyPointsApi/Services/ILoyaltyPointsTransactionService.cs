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
        public Task<LoyaltyPoints?> GetLoyaltyPointsTransaction(LoyaltyPointsTransactionRequestModel loyaltyPointsRequestModel);

        public Task AddLoyaltyPointsTransaction(LoyaltyPointsTransactionRequestModel loyaltyPointsRequestModel);

        public Task<List<LoyaltyPoints>> GetLoyaltyPointsTransactions(LoyaltyPointsTransactionRequestModel loyaltyPointsRequestModel);

        public Task<int> GetTotalPoints(LoyaltyPointsTransactionRequestModel loyaltyPointsRequestModel);
    }
}