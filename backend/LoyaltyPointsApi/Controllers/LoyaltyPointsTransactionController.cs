using LoyaltyPointsApi.RequestModels;
using LoyaltyPointsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyPointsApi.Controllers
{
    /// <summary>
    ///     Controller for transactions in the loyalty points system
    /// </summary>
    [ApiController]
    [Route("api/transactions")]
    public class LoyaltyPointsTransactionController(ILoyaltyPointsTransactionService service,
    ILogger<LoyaltyPointsTransactionController> logger) : ControllerBase
    {
        /// <summary>
        /// Get the transaction for a customer with a specific transaction id
        /// </summary>
        /// <param name="transactionId">The transaction id</param>
        /// <returns>The transaction found</returns>
        /// <response code="200">Transaction found</response>
        /// <response code="500">Internal server error</response>
        /// <example>
        ///  GET /api/transactions/1 HTTP/1.1
        ///  Host: localhost:5000
        ///  Content-Type: application/json
        /// </example>
        /// <example>
        ///  HTTP/1.1 200 OK
        ///  Content-Type: application/json
        ///  
        ///  {
        ///      "success": true,
        ///      "message": "Transaction found",
        ///      "data": {
        ///          "TransactionId": 1,
        ///          "CustomerId": 1,
        ///          "ReceiptId": 1,
        ///          "RestaurantId": 1,
        ///          "TransactionDate": "2022-11-01T14:30:00",
        ///          "ExpiryDate": "2022-11-01T14:30:00",
        ///          "Points": 100
        ///      }
        ///  }
        /// </example>
        [HttpGet]
        [Route("{transactionId}")]
        public async Task<ActionResult> GetCustomerTransactionByTransactionId([FromRoute] int transactionId)
        {
            logger.LogInformation("Request to get transaction: {transactionId}", transactionId);
            try
            {
                var result = await service.GetLoyaltyPointsTransaction(transactionId);
                return Ok(new
                {
                    success = true,
                    message = "Transaction found",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Get the transaction for a customer with a specific receipt id
        /// </summary>
        /// <param name="receiptId">The receipt id</param>
        /// <returns>The transaction found</returns>
        /// <response code="200">Transaction found</response>
        /// <response code="500">Internal server error</response>
        /// <example>
        ///  GET /api/transactions/receipts/1 HTTP/1.1
        ///  Host: localhost:5000
        ///  Content-Type: application/json
        /// </example>
        /// <example>
        ///  HTTP/1.1 200 OK
        ///  Content-Type: application/json
        ///  
        ///  {
        ///      "success": true,
        ///      "message": "Transaction found",
        ///      "data": {
        ///          "TransactionId": 1,
        ///          "CustomerId": 1,
        ///          "ReceiptId": 1,
        ///          "RestaurantId": 1,
        ///          "TransactionDate": "2022-11-01T14:30:00",
        ///          "ExpiryDate": "2022-11-01T14:30:00",
        ///          "Points": 100
        ///      }
        ///  }
        /// </example>
        [HttpGet]
        [Route("receipts/{receiptId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetCustomerTransactionByReceiptId([FromRoute] int receiptId)
        {
            logger.LogInformation("Request to get transaction: {receiptId}", receiptId);
            try
            {
                var result = await service.GetLoyaltyPointsTransaction(receiptId);
                return Ok(new
                {
                    success = true,
                    message = "Transaction found",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Get the transactions for a customer at a specific restaurant
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="restaurantId">The restaurant id</param>
        /// <returns>The transactions found</returns>
        /// <response code="200">Transactions found</response>
        /// <response code="500">Internal server error</response>
        /// <example>
        ///  GET /api/transactions/customers/1/restaurants/1 HTTP/1.1
        ///  Host: localhost:5000
        ///  Content-Type: application/json
        /// </example>
        /// <example>
        ///  HTTP/1.1 200 OK
        ///  Content-Type: application/json
        ///  
        /// {
        ///     "success": true,
        ///     "message": "Transactions found",
        ///     "data": [
        ///     {
        ///         "transactionId": 1,
        ///         "receiptId": 100,
        ///         "customerId": 1,
        ///         "restaurantId": 1,
        ///         "points": 0,
        ///         "transactionDate": "2024-10-06T13:41:44.019",
        ///         "expiryDate": "2027-07-03T13:41:44.019",
        ///         "isExpired": false,
        ///         "restaurant": null
        ///     },
        ///     {
        ///         "transactionId": 2,
        ///         "receiptId": 101,
        ///         "customerId": 1,
        ///         "restaurantId": 1,
        ///         "points": 0,
        ///         "transactionDate": "2024-10-06T13:41:45.019",
        ///         "expiryDate": "2027-07-03T13:41:45.019",
        ///         "isExpired": false,
        ///         "restaurant": null
        ///     }
        ///     ],
        ///     "metadata": {
        ///         "pageNumber": 1,
        ///         "pageSize": 2,
        ///         "totalItemCount": 3,
        ///         "pageCount": 2,
        ///         "hasPreviousPage": false,
        ///         "hasNextPage": true
        ///     }
        /// }
        /// </example>
        [HttpGet]
        [Route("customers/{customerId}/restaurants/{restaurantId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetCustomerTransactions([FromRoute] int customerId, [FromRoute] int restaurantId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            logger.LogInformation("Request to get transactions: {customerId} for restaurant: {restaurantId}", customerId, restaurantId);
            try
            {
                var pagedTransactions = await service.GetLoyaltyPointsTransactions(customerId, restaurantId, pageNumber, pageSize);
                return Ok(new
                {
                    success = true,
                    message = "Transactions found",
                    data = pagedTransactions.ToList(),
                    metadata = new
                    {
                        PageNumber = pagedTransactions.PageNumber,
                        PageSize = pagedTransactions.PageSize,
                        TotalItemCount = pagedTransactions.TotalItemCount,
                        PageCount = pagedTransactions.PageCount,
                        HasPreviousPage = pagedTransactions.HasPreviousPage,
                        HasNextPage = pagedTransactions.HasNextPage
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Add a new transaction
        /// </summary>
        /// <param name="loyaltyPointsRequestModel">The transaction to add</param>
        /// <returns>The transaction added</returns>
        /// <response code="201">Transaction added</response>
        /// <response code="500">Internal server error</response>
        /// <example>
        ///  POST /api/transactions HTTP/1.1
        ///  Content-Type: application/json
        ///  {
        ///      "customerId": 1,
        ///      "restaurantId": 1,
        ///      "ReceiptId": 1,          
        ///      "transactionDate": "2022-11-01T14:30:00",
        ///      "expiryDate": "2022-11-01T14:30:00",
        ///      "points": 100
        ///  }
        /// </example>
        /// <example>
        ///  HTTP/1.1 201 Created
        ///  Content-Type: application/json
        ///  
        ///  {
        ///      "success": true,
        ///      "message": "Transaction added",
        ///      "data": {
        ///          "TransactionId": 1,
        ///          "CustomerId": 1,
        ///          "ReceiptId": 1,          
        ///          "RestaurantId": 1,
        ///          "TransactionDate": "2022-11-01T14:30:00",
        ///          "ExpiryDate": "2022-11-01T14:30:00",
        ///          "Points": 100
        ///      }
        ///  }
        /// </example>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddLoyaltyPointsTransaction([FromBody] LoyaltyPointsTransactionRequestModel loyaltyPointsRequestModel)
        {
            logger.LogInformation("Adding LoyaltyPointsTransaction: {customerId} for restaurant: {restaurantId}", loyaltyPointsRequestModel.CustomerId, loyaltyPointsRequestModel.RestaurantId);
            try
            {
                var result = await service.AddLoyaltyPointsTransaction(loyaltyPointsRequestModel);
                return Ok(new
                {
                    success = true,
                    message = "Transaction added",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    Message = ex.Message
                });
            }
        }
    }
}