using LoyaltyPointsApi.Models;
using X.PagedList;

namespace LoyaltyPointsApi.Repositories
{
    public interface ILoyaltyPointsTransactionRepository
    {
        public Task<LoyaltyPoints> AddLoyaltyPointsTransaction(LoyaltyPoints loyaltyPointsTransaction);

        public Task<List<LoyaltyPoints?>> GetLoyaltyPointsTransactions(LoyaltyPoints loyaltyPointsTransaction);

        public Task<LoyaltyPoints?> GetLoyaltyPointsTransaction(LoyaltyPoints loyaltyPointsTransaction);

        public Task<List<int>> GetCustomersByRestaurantAndPointsRange(int restaurantId, int? minimumPoints, int? maximumPoints);
        
        public Task<IPagedList<LoyaltyPoints>> GetPagedLoyaltyPointsTransactions(LoyaltyPoints loyaltyPointsTransaction,int pageNumber, int pageSize);

    }
}