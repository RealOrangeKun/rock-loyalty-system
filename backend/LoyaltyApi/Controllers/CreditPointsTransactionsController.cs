using LoyaltyApi.Models;
using Microsoft.AspNetCore.Mvc;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authorization;


namespace LoyaltyApi.Controllers;

/// <summary>
///  Controller for managing credit points transactions. 
/// </summary>
[Route("api")]
[ApiController]
public class CreditPointsTransactionController(
    ICreditPointsTransactionService transactionService,
    ILogger<CreditPointsTransactionController> logger) : ControllerBase
{
    /// <summary>
    /// Retrieves a credit points transaction by its ID.
    /// </summary>
    /// <param name="transactionId">The ID of the transaction.</param>
    /// <returns>Action result containing the transaction.</returns>
    /// <response code="200">The transaction was retrieved successfully.</response>
    /// <response code="404">No transaction was found with the specified ID.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/admin/credit-points-transactions/1
    /// 
    /// Sample response:
    /// 
    ///     200 OK
    ///     {
    ///         "success": true,
    ///         "data": {
    ///             "transaction":{
    ///                     "transactionId": 1,
    ///                     "receiptId": 1,
    ///                     "restaurantId": 1,
    ///                     "customerId": 1,
    ///                     "transactionType": "0",
    ///                     "transactionDate": "2022-01-01T00:00:00.000Z",
    ///                     "isExpired": false,
    ///                     "points": 100
    ///            }
    ///         },
    ///         "message": "Transaction retrieved successfully"
    ///     }
    ///
    ///  **Admin Only Endpoint**
    /// Authorization header with JWT Bearer token is required.
    /// </remarks>
    [HttpGet]
    [Route("admin/credit-points-transactions/{transactionId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetTransactionById(int transactionId)
    {
        try
        {
            logger.LogInformation("Get transaction by id request for {TransactionId}", transactionId);
            var transaction = await transactionService.GetTransactionByIdAsync(transactionId);
            if (transaction == null)
            {
                return NotFound(new { success = false, message = "Transaction not found" });
            }
            var responseTransaction = new {
                transactionId = transaction.TransactionId,
                customerId = transaction.CustomerId,
                receiptId = transaction.ReceiptId,
                transactionType = transaction.TransactionType,
                transactionDate = transaction.TransactionDate,
                isExpired = transaction.IsExpired,
                points = transaction.Points
            };
            return Ok(new { success = true, data = new { transaction = responseTransaction }, message = "Transaction found" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting transaction by id");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Retrieves a credit points transaction by its receipt ID.
    /// </summary>
    /// <param name="receiptId">The ID of the receipt.</param>
    /// <returns>Action result containing the transaction.</returns>
    /// <response code="200">The transaction was retrieved successfully.</response>
    /// <response code="404">No transaction was found with the specified receipt ID.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/credit-points-transactions/receipts/1
    /// 
    /// Sample response:
    /// 
    ///     200 OK
    ///     {
    ///         "success": true,
    ///         "data": {
    ///             "transaction":{
    ///                     "transactionId": 1,
    ///                     "receiptId": 1,
    ///                     "restaurantId": 1,
    ///                     "customerId": 1,
    ///                     "transactionType": "0",
    ///                     "transactionDate": "2022-01-01T00:00:00.000Z",
    ///                     "isExpired": false,
    ///                     "points": 100
    ///              }
    ///         },
    ///         "message": "Transaction retrieved successfully"
    ///     }
    ///
    ///  **Admin Only Endpoint**
    /// Authorization header with JWT Bearer token is required.
    /// </remarks>
    [HttpGet]
    [Route("admin/credit-points-transactions/receipt/{receiptId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetTransactionByReceiptId(int receiptId)
    {
        try
        {
            logger.LogInformation("Get transaction by receipt id request for {ReceiptId}", receiptId);
            var transaction = await transactionService.GetTransactionByReceiptIdAsync(receiptId);
            if (transaction == null)
            {
                return NotFound(new { success = false, message = "Transaction not found" });
            }
            
            var responseTransaction = new {
                transactionId = transaction.TransactionId,
                customerId = transaction.CustomerId,
                receiptId = transaction.ReceiptId,
                transactionType = transaction.TransactionType,
                transactionDate = transaction.TransactionDate,
                isExpired = transaction.IsExpired,
                points = transaction.Points
            };
            return Ok(new { success = true, data = new { transaction = responseTransaction }, message = "Transaction found" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting transaction by receipt id");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    // Add transaction
    /// <summary>
    /// Creates a new credit points transaction.
    /// </summary>
    /// <param name="transactionRequest">The request model containing transaction details.</param>
    /// <returns>Action result indicating the outcome of the operation.</returns>
    /// <response code="201">The transaction was created successfully.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/admin/credit-points-transactions
    ///     {
    ///         "restaurantId": 1,
    ///         "customerId": 1,
    ///         "amount": 100.00,
    ///         "transactionType": "0",
    ///         "transactionDate": "2022-01-01T00:00:00",
    ///         "receiptId": "1234567890"
    ///     }
    /// 
    /// Sample response:
    /// 
    ///     201 Created
    ///     {
    ///         success: true,
    ///         message: "Credit points transaction created"
    ///     }
    ///
    ///  **Admin Only Endpoint**
    /// Authorization header with JWT Bearer token is required.
    /// </remarks>
    [HttpPost]
    [Route("admin/credit-points-transactions")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddTransaction(CreateTransactionRequest transactionRequest)
    {
        try
        {
            logger.LogInformation("Add transaction request for restaurant {RestaurantId}",
                transactionRequest.RestaurantId);
            await transactionService.AddTransactionAsync(transactionRequest);
            return StatusCode(StatusCodes.Status201Created,
                new { success = true, message = "Credit points transaction created" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding transaction");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

// Get all transactions made by customerId and restaurantId
    /// <summary>
    /// Retrieves all credit points transactions made by a customer and restaurant.
    /// </summary>
    /// <param name="customerId">The ID of the customer.</param>
    /// <param name="restaurantId">The ID of the restaurant.</param>
    /// <returns>Action result containing a list of transactions.</returns>
    /// <response code="200">The transactions were retrieved successfully.</response>
    /// <response code="404">No transactions were found for the specified customer and restaurant.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/customers/1/restaurants/1/credit-points-transactions
    /// 
    /// Sample response:
    /// 
    ///     {
    ///         "success": true,
    ///         "data":{
    ///             "transactions": [
    ///             {
    ///                 "transactionId": 1,
    ///                 "receiptId": 1,
    ///                 "restaurantId": 1,
    ///                 "customerId": 1,
    ///                 "transactionType": "0",
    ///                 "transactionDate": "2022-01-01T00:00:00.000Z",
    ///                 "isExpired": false,
    ///                 "points": 100
    ///             }
    ///         ]},
    ///         "message": "Transactions retrieved successfully"
    ///     }
    ///
    /// Authorization header with JWT Bearer token is required.
    /// </remarks>
    [HttpGet]
    [Route("customers/{customerId}/restaurants/{restaurantId}/credit-points-transactions")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> GetTransactionsByCustomer(int customerId, int restaurantId)
    {
        try
        {
            logger.LogInformation("Get transactions for customer {CustomerId} and restaurant {RestaurantId}",
                customerId, restaurantId);
            var transactions =
                await transactionService.GetTransactionsByCustomerAndRestaurantAsync(customerId, restaurantId);
            transactions = transactions.Select(t => new CreditPointsTransaction
            {
                TransactionId = t.TransactionId,
                RestaurantId = t.RestaurantId,
                ReceiptId = t.ReceiptId,
                CustomerId = t.CustomerId,
                TransactionType = t.TransactionType,
                TransactionDate = t.TransactionDate,
                IsExpired = t.IsExpired,
                Points = t.Points,
            });
            return Ok(new
                { success = true, data = new { transactions }, message = "Transactions retrieved successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting transactions for customer and restaurant");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }
}