using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.RequestModels;
using X.PagedList;

namespace LoyaltyPointsApi.Services
{
    public interface ILoyaltyPointsTransactionService
    {
        public Task<LoyaltyPoints?> GetLoyaltyPointsTransaction(int transactionId);

        public Task<LoyaltyPoints> AddLoyaltyPointsTransaction(LoyaltyPointsTransactionRequestModel loyaltyPointsRequestModel);

        public Task<IPagedList<LoyaltyPoints>> GetLoyaltyPointsTransactions(int customerId , int restaurantId,int pageNumber, int pageSize);

        public Task<int> GetTotalPoints(int customerId , int restaurantId);


    }
}