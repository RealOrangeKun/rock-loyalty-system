using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;

namespace LoyaltyPointsApi.Repositories
{
    public interface ILoyaltyPointsTransactionRepository
    {
        public Task AddLoyaltyPointsTransaction(LoyaltyPoints loyaltyPointsTransaction);

        public Task<List<LoyaltyPoints>> GetLoyaltyPointsTransactions(LoyaltyPoints loyaltyPointsTransaction);

        public Task<LoyaltyPoints> GetLoyaltyPointsTransaction(LoyaltyPoints loyaltyPointsTransaction);
    }
}