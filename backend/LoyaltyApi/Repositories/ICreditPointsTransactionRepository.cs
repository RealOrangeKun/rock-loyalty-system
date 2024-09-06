using LoyaltyApi.Models;

namespace LoyaltyApi.Repositories;

public interface ICreditPointsTransactionRepository
{
    public Task<CreditPointsTransaction?> GetTransactionByIdAsync(int transactionId);

    public Task<CreditPointsTransaction?> GetTransactionByReceiptIdAsync(int receiptId);

    public Task<IEnumerable<CreditPointsTransaction>> GetTransactionsByCustomerAndRestaurantAsync(int customerId,
        int restaurantId);

    public Task AddTransactionAsync(CreditPointsTransaction transaction);

    public Task AddTransactionsAsync(List<CreditPointsTransaction> transactions);

    public Task UpdateTransactionAsync(CreditPointsTransaction transaction);

    public Task DeleteTransactionAsync(int transactionId);

    public Task<int> GetCustomerPointsAsync(int customerId, int restaurantId);

    public Task<IEnumerable<CreditPointsTransaction>> GetExpiredTransactionsAsync(Dictionary<int, int> restaurantMap,
        DateTime currentDate);
}