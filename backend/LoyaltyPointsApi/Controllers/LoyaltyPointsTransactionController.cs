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
    [Route("api/transaction")]
    public class LoyaltyPointsTransactionController(ILoyaltyPointsTransactionService service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetCustomerTransactions([FromQuery] int transactionId)
        {
           try{
            var result = await service.GetLoyaltyPointsTransaction(transactionId);
            return Ok( new {
                Status = "Success",
                Message = "Transaction found",
                Data = result
            });
           } 
           catch(Exception ex){
            return StatusCode(500, new {
                Status = "Error",
                Message = ex.Message
           });
           }
        }

        [HttpGet]
        public async Task<ActionResult> GetCustomerTransactions([FromQuery] int customerId, [FromQuery] int restaurantId)
        {
            try
            {
                var result = await service.GetLoyaltyPointsTransactions(customerId, restaurantId);
                return Ok(new
                {
                    Status = "Success",
                    Message = "Transactions found",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }
        [HttpGet]
        public async Task<ActionResult> GetTotalPoints([FromBody] LoyaltyPointsTransactionRequestModel loyaltyPointsRequestModel)
        {
            try
            {
                var result = await service.GetTotalPoints(loyaltyPointsRequestModel);
                return Ok(new
                {
                    Status = "Success",
                    Message = "Total points found",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<ActionResult> AddLoyaltyPointsTransaction([FromBody] LoyaltyPointsTransactionRequestModel loyaltyPointsRequestModel)
        {
            try
            {
                var result = await service.AddLoyaltyPointsTransaction(loyaltyPointsRequestModel);
                return Ok(new
                {
                    Status = "Success",
                    Message = "Transaction added",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Transaction Failed",
                    Message = ex.Message
                });
            }
        }
    }
}