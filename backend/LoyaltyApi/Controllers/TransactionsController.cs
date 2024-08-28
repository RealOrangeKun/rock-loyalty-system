using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController(ICreditPointsTransactionService pointsTransactionService) : ControllerBase
    {
        [HttpPost]
        [Route("")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateTransaction(CreateTransactionRequest createTransactionRequest)
        {
            try
            {
                await pointsTransactionService.AddTransactionAsync(createTransactionRequest);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}