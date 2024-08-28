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
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> GetTransactions()
        {
            var transactions = await pointsTransactionService.GetTransactionsByCustomerAndRestaurantAsync(customerId, restaurantId);
            return Ok(transactions);
        }
    }
}