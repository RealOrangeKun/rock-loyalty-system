using System.Security.Claims;
using LoyaltyApi.Data;
using LoyaltyApi.Exceptions;
using LoyaltyApi.Models;
using LoyaltyApi.Repositories;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Utilities;

namespace LoyaltyApi.Services;

public class CreditPointsTransactionService(
    ICreditPointsTransactionRepository transactionRepository,
    ICreditPointsTransactionDetailRepository transactionDetailRepository,
    IRestaurantRepository restaurantRepository,
    RockDbContext context,
    IHttpContextAccessor httpContext,
    CreditPointsUtility creditPointsUtility,
    ILogger<CreditPointsTransactionService> logger) : ICreditPointsTransactionService
{
    public async Task<CreditPointsTransaction?> GetTransactionByIdAsync(int transactionId)
    {
        logger.LogInformation("Getting transaction {TransactionId}", transactionId);
        return await transactionRepository.GetTransactionByIdAsync(transactionId);
    }

    public async Task<CreditPointsTransaction?> GetTransactionByReceiptIdAsync(int receiptId)
    {
        logger.LogInformation("Getting transaction for receipt {ReceiptId}", receiptId);
        return await transactionRepository.GetTransactionByReceiptIdAsync(receiptId);
    }

    public async Task<IEnumerable<CreditPointsTransaction>> GetTransactionsByCustomerAndRestaurantAsync(int? customerId,
        int? restaurantId)
    {
        logger.LogInformation("Getting transactions for customer {CustomerId} and restaurant {RestaurantId}", customerId, restaurantId);
        int customerIdJwt = customerId ??
                            int.Parse(httpContext.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                                      throw new ArgumentException("customerId not found"));
        logger.LogTrace("customerIdJwt: {customerIdJwt}", customerIdJwt);
        int restaurantIdJwt = restaurantId ??
                              int.Parse(httpContext.HttpContext?.User?.FindFirst("restaurantId")?.Value ??
                                        throw new ArgumentException("restaurantId not found"));
        logger.LogTrace("restaurantIdJwt: {restaurantIdJwt}", restaurantIdJwt);
        return await transactionRepository.GetTransactionsByCustomerAndRestaurantAsync(customerId ?? customerIdJwt,
            restaurantId ?? restaurantIdJwt);
    }

    public async Task AddTransactionAsync(CreateTransactionRequest transactionRequest)
    {
        logger.LogInformation("Adding transaction for customer {CustomerId} and restaurant {RestaurantId}", transactionRequest.CustomerId, transactionRequest.RestaurantId);
        var restaurant = await restaurantRepository.GetRestaurantById(transactionRequest.RestaurantId) ??
                         throw new ArgumentException("Invalid restaurant");
        var transaction = new CreditPointsTransaction
        {
            CustomerId = transactionRequest.CustomerId,
            RestaurantId = transactionRequest.RestaurantId,
            ReceiptId = transactionRequest.ReceiptId,
            TransactionType = transactionRequest.TransactionType,
            Points = creditPointsUtility.CalculateCreditPoints(transactionRequest.Amount,
                restaurant.CreditPointsSellingRate),
            TransactionDate = transactionRequest.TransactionDate ?? DateTime.Now
        };
        await transactionRepository.AddTransactionAsync(transaction);
    }

    /// <summary>
    /// Spends the specified number of credit points for a customer at a restaurant.
    /// 
    /// This function starts a database transaction, retrieves the customer's available transactions, 
    /// checks if there are enough points to spend, creates a new spend transaction, and distributes 
    /// the points across available transactions.
    /// 
    /// Parameters:
    ///   customerId (int): The ID of the customer spending the points.
    ///   restaurantId (int): The ID of the restaurant where the points are being spent.
    ///   points (int): The number of points to spend.
    /// 
    /// Returns:
    ///   Task: A task representing the asynchronous operation.
    /// </summary>
    public async Task SpendPointsAsync(int customerId, int restaurantId, int points)
    {
        logger.LogInformation("Spending {Points} points for customer {CustomerId} at restaurant {RestaurantId}", points, customerId, restaurantId);
        var restaurant = await restaurantRepository.GetRestaurantById(restaurantId) ??
                         throw new ArgumentException("Invalid restaurant");

        await using var dbTransaction = await context.Database.BeginTransactionAsync(); // Start a database transaction

        try
        {
            // Retrieve customer transactions and spend the points
            var transactions =
                await transactionRepository.GetTransactionsByCustomerAndRestaurantAsync(customerId, restaurantId);
            var remainingPoints = points;

            // Check if there are enough points before proceeding
            var totalAvailablePoints = transactions.Sum(t => t.Points);
            if (totalAvailablePoints < points)
            {
                throw new PointsNotEnoughException("Not enough points");
            }

            // Create a new spend transaction
            var spendTransaction = new CreditPointsTransaction
            {
                CustomerId = customerId,
                RestaurantId = restaurantId,
                Points = -points,
                TransactionType = TransactionType.Spend,
                TransactionDate = DateTime.UtcNow
            };

            await transactionRepository.AddTransactionAsync(spendTransaction);

            // Distribute the points to spend across available transactions
            foreach (var transaction in transactions
                         .Where(transaction =>
                             transaction.TransactionDate > DateTime.UtcNow.AddDays(-restaurant.CreditPointsLifeTime))
                         .OrderBy(t => t.TransactionDate))
            {
                if (remainingPoints <= 0) break;

                var availablePoints = transaction.Points;

                if (availablePoints <= 0) continue;

                var pointsToUse = Math.Min(availablePoints, remainingPoints);
                remainingPoints -= pointsToUse;

                var transactionDetail = new CreditPointsTransactionDetail
                {
                    TransactionId = spendTransaction.TransactionId,
                    EarnTransactionId = transaction.TransactionId,
                    PointsUsed = pointsToUse,
                };
                await transactionDetailRepository.AddTransactionDetailAsync(transactionDetail);
            }

            await context.SaveChangesAsync();
            await dbTransaction.CommitAsync();
        }
        catch
        {
            await dbTransaction.RollbackAsync(); // Rollback if any error occurs
            throw;
        }
    }

    public async Task<int> GetCustomerPointsAsync(int? customerId, int? restaurantId)
    {
        logger.LogInformation("Getting customer points for customer {CustomerId} and restaurant {RestaurantId}", customerId, restaurantId);
        int customerIdJwt = customerId ??
                            int.Parse(httpContext.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                                      throw new ArgumentException("customerId not found"));
        logger.LogTrace("customerIdJwt: {customerIdJwt}", customerIdJwt);
        int restaurantIdJwt = restaurantId ??
                              int.Parse(httpContext.HttpContext?.User?.FindFirst("restaurantId")?.Value ??
                                        throw new ArgumentException("restaurantId not found"));
        logger.LogTrace("restaurantIdJwt: {restaurantIdJwt}", restaurantIdJwt);
        return await transactionRepository.GetCustomerPointsAsync(customerId ?? customerIdJwt,
            restaurantId ?? restaurantIdJwt);
    }

    public async Task<int> ExpirePointsAsync()
    {
        logger.LogInformation("Expiring points");
        await using var dbTransaction = await context.Database.BeginTransactionAsync(); // Start a database transaction

        try
        {
            // Load all restaurant data into memory (ID and CreditPointsLifeTime)
            var restaurants = await restaurantRepository.GetAllRestaurantsAsync();
            var restaurantMap = restaurants.ToDictionary(r => r.RestaurantId, r => r.CreditPointsLifeTime);
            var currentDateTime = DateTime.UtcNow;
            // Fetch all transactions that have expired based on the restaurant's lifetime
            var expiredTransactions =
                await transactionRepository.GetExpiredTransactionsAsync(restaurantMap, currentDateTime);

            if (!expiredTransactions.Any())
            {
                return 0; // No transactions to expire
            }

            // Create lists to hold new expiration transactions and details
            var expirationTransactions = new List<CreditPointsTransaction>();
            var expirationDetails = new List<CreditPointsTransactionDetail>();

            // Process the expired transactions
            foreach (var transaction in expiredTransactions)
            {
                // Fetch total points spent from this earn transaction
                var pointsSpent =
                    await transactionDetailRepository.GetTotalPointsSpentForEarnTransaction(transaction.TransactionId);

                // Calculate remaining points that can be expired
                var remainingPoints = transaction.Points - pointsSpent;

                if (remainingPoints > 0)
                {
                    // Create an expire transaction
                    var expireTransaction = new CreditPointsTransaction
                    {
                        CustomerId = transaction.CustomerId,
                        RestaurantId = transaction.RestaurantId,
                        Points = -remainingPoints,
                        TransactionType = TransactionType.Expire,
                        TransactionDate = currentDateTime
                    };
                    expirationTransactions.Add(expireTransaction);

                    // Create a transaction detail for the expiration
                    var expireDetail = new CreditPointsTransactionDetail
                    {
                        TransactionId = expireTransaction.TransactionId, // Reference the new `expire` transaction
                        EarnTransactionId = transaction.TransactionId, // Reference the original `earn` transaction
                        PointsUsed = remainingPoints
                    };
                    expirationDetails.Add(expireDetail);
                    transaction.IsExpired = true; // Mark the original `earn` transaction as expired
                    await transactionRepository.UpdateTransactionAsync(transaction);
                }
            }

            // Add new expiration transactions and details to the database
            await transactionRepository.AddTransactionsAsync(expirationTransactions);
            await transactionDetailRepository.AddTransactionDetailsAsync(expirationDetails);

            // Commit the changes
            await context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return expirationTransactions.Count();
        }
        catch
        {
            await dbTransaction.RollbackAsync(); // Rollback transaction on error
            throw;
        }
    }
}