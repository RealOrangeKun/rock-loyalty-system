using LoyaltyApi.Models;

namespace LoyaltyApi.Repositories;

public interface ICreditPointsTransactionRepository
{
    Task<CreditPointsTransaction?> GetTransactionByIdAsync(int transactionId);
    
    Task<CreditPointsTransaction?> GetTransactionByReceiptIdAsync(int receiptId);
    
    Task<IEnumerable<CreditPointsTransaction>> GetTransactionsByCustomerAndRestaurantAsync(int customerId, int restaurantId);
    
    Task AddTransactionAsync(CreditPointsTransaction transaction);
    
    Task UpdateTransactionAsync(CreditPointsTransaction transaction);
    
    Task DeleteTransactionAsync(int transactionId);
    
    Task<int> GetCustomerPointsAsync(int customerId, int restaurantId);
}