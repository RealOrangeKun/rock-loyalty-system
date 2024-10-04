using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using LoyaltyPointsApi.RequestModels;
using LoyaltyPointsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyPointsApi.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class LoyaltyPointsTransactionController(ILoyaltyPointsTransactionService service,
    ILogger<LoyaltyPointsTransactionController> logger) : ControllerBase
    {
        [HttpGet]
        [Route("{transactionId}")]
        public async Task<ActionResult> GetCustomerTransaction([FromRoute] int transactionId)
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

        [HttpGet]
        [Route("customers/{customerId}/restaurants/{restaurantId}")]
        public async Task<ActionResult> GetCustomerTransactions([FromRoute] int customerId, [FromRoute] int restaurantId)
        {
            logger.LogInformation("Request to get transactions: {customerId} for restaurant: {restaurantId}", customerId, restaurantId);
            try
            {
                var result = await service.GetLoyaltyPointsTransactions(customerId, restaurantId);
                return Ok(new
                {
                    success = true,
                    message = "Transactions found",
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

        [HttpPost]
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