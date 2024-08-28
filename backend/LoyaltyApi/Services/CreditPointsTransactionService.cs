using System.Security.Claims;
using LoyaltyApi.Data;
using LoyaltyApi.Models;
using LoyaltyApi.Repositories;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Utilities;

// namespace LoyaltyApi.Services;

public class CreditPointsTransactionService(
    ICreditPointsTransactionRepository transactionRepository,
    ICreditPointsTransactionDetailRepository transactionDetailRepository,
    IRestaurantRepository restaurantRepository,
    RockDbContext context,
    IHttpContextAccessor httpContext,
    CreditPointsUtility creditPointsUtility) : ICreditPointsTransactionService
{
    public async Task<CreditPointsTransaction?> GetTransactionByIdAsync(int transactionId)
    {
        return await transactionRepository.GetTransactionByIdAsync(transactionId);
    }

//     public async Task<CreditPointsTransaction?> GetTransactionByReceiptIdAsync(int receiptId)
//     {
//         return await transactionRepository.GetTransactionByReceiptIdAsync(receiptId);
//     }

//     public async Task<IEnumerable<CreditPointsTransaction>> GetTransactionsByCustomerAndRestaurantAsync(int customerId,
//         int restaurantId)
//     {
//         return await transactionRepository.GetTransactionsByCustomerAndRestaurantAsync(customerId, restaurantId);
//     }

    public async Task AddTransactionAsync(CreateTransactionRequest transactionRequest)
    {
        var restaurant = await restaurantRepository.GetRestaurantInfo(transactionRequest.RestaurantId) ?? throw new Exception("Invalid restaurant");
        var transaction = new CreditPointsTransaction
        {
            CustomerId = transactionRequest.CustomerId,
            RestaurantId = transactionRequest.RestaurantId,
            ReceiptId = transactionRequest.ReceiptId,
            TransactionType = transactionRequest.TransactionType,
            Points = creditPointsUtility.CalculateCreditPoints(transactionRequest.Amount, restaurant.CreditPointsSellingRate),
            TransactionDate = transactionRequest.TransactionDate ?? DateTime.Now
        };
        await transactionRepository.AddTransactionAsync(transaction);
    }

//     /// <summary>
//     /// Spends the specified number of credit points for a customer at a restaurant.
//     /// 
//     /// This function starts a database transaction, retrieves the customer's available transactions, 
//     /// checks if there are enough points to spend, creates a new spend transaction, and distributes 
//     /// the points across available transactions.
//     /// 
//     /// Parameters:
//     ///   customerId (int): The ID of the customer spending the points.
//     ///   restaurantId (int): The ID of the restaurant where the points are being spent.
//     ///   points (int): The number of points to spend.
//     /// 
//     /// Returns:
//     ///   Task: A task representing the asynchronous operation.
//     /// </summary>
//     public async Task SpendPointsAsync(int customerId, int restaurantId, int points)
//     {
//         await using var dbTransaction = await context.Database.BeginTransactionAsync(); // Start a database transaction

//         try
//         {
//             // Retrieve customer transactions and spend the points
//             var transactions =
//                 await transactionRepository.GetTransactionsByCustomerAndRestaurantAsync(customerId, restaurantId);
//             var remainingPoints = points;

//             // Check if there are enough points before proceeding
//             var totalAvailablePoints = transactions.Sum(t => t.Points);
//             if (totalAvailablePoints < points)
//             {
//                 throw new Exception("Not enough points");
//             }

//             // Create a new spend transaction
//             var spendTransaction = new CreditPointsTransaction
//             {
//                 CustomerId = customerId,
//                 RestaurantId = restaurantId,
//                 Points = -points,
//                 TransactionType = TransactionType.Spend,
//                 TransactionDate = DateTime.UtcNow
//             };

//             await transactionRepository.AddTransactionAsync(spendTransaction);
//             var restaurant = await restaurantRepository.GetRestaurantInfo(restaurantId);

//             // Distribute the points to spend across available transactions
//             foreach (var transaction in transactions
//                          .Where(transaction =>
//                              transaction.TransactionDate > DateTime.UtcNow.AddDays(-restaurant.CreditPointsLifeTime))
//                          .OrderBy(t => t.TransactionDate))
//             {
//                 if (remainingPoints <= 0) break;

//                 var availablePoints = transaction.Points;

                if (availablePoints <= 0) continue;

                var pointsToUse = Math.Min(availablePoints, remainingPoints);
                remainingPoints -= pointsToUse;

//                 var transactionDetail = new CreditPointsTransactionDetail
//                 {
//                     TransactionId = spendTransaction.TransactionId,
//                     EarnTransactionId = transaction.TransactionId,
//                     PointsUsed = pointsToUse,
//                 };
//                 await transactionDetailRepository.AddTransactionDetailAsync(transactionDetail);
//             }

//             await context.SaveChangesAsync();
//             await dbTransaction.CommitAsync();
//         }
//         catch
//         {
//             await dbTransaction.RollbackAsync(); // Rollback if any error occurs
//             throw;
//         }
//     }

    public async Task<int> GetCustomerPointsAsync(int? customerId, int? restaurantId)
    {
        int customerIdJwt = customerId ?? int.Parse(httpContext.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("customerId not found"));
        int restaurantIdJwt = restaurantId ?? int.Parse(httpContext.HttpContext?.User?.FindFirst("restaurantId")?.Value ?? throw new ArgumentException("restaurantId not found"));
        return await transactionRepository.GetCustomerPointsAsync(customerId ?? customerIdJwt, restaurantId ?? restaurantIdJwt);
    }

//     public Task<int> ExpirePointsAsync(int customerId, int restaurantId)
//     {
//         throw new NotImplementedException();
//     }
// }