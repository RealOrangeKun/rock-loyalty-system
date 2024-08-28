using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;

namespace LoyaltyApi.Services
{
    public interface ICreditPointsTransactionService
    {
        public Task<CreditPointsTransaction?> GetTransactionByIdAsync(int transactionId);

        public Task<CreditPointsTransaction?> GetTransactionByReceiptIdAsync(int receiptId);

        public Task<IEnumerable<CreditPointsTransaction>> GetTransactionsByCustomerAndRestaurantAsync(int? customerId,
            int? restaurantId);

        public Task AddTransactionAsync(CreateTransactionRequest transactionRequest);

        public Task SpendPointsAsync(int customerId, int restaurantId, int points);

        public Task<int> GetCustomerPointsAsync(int? customerId, int? restaurantId);

        public Task<int> ExpirePointsAsync(int customerId, int restaurantId);
    }
}