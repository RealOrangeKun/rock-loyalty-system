using Microsoft.AspNetCore.Mvc;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using Microsoft.IdentityModel.Tokens;


namespace LoyaltyApi.Controllers;

[Route("api")]
[ApiController]
public class CreditPointsTransactionController(ICreditPointsTransactionService transactionService) : ControllerBase
{
    // Get transaction by transaction id
    [HttpGet("credit-points-transactions/{transactionId}")]
    public async Task<IActionResult> GetTransaction(int transactionId)
    {
        var transaction = await transactionService.GetTransactionByIdAsync(transactionId);
        if (transaction == null)
        {
            return NotFound();
        }

        return Ok(transaction);
    }

    // Get transaction by receipt id
    [HttpGet("credit-points-transactions/receipt/{receiptId}")]
    public async Task<IActionResult> GetTransactionByReceiptId(int receiptId)
    {
        var transaction = await transactionService.GetTransactionByReceiptIdAsync(receiptId);
        if (transaction == null)
        {
            return NotFound();
        }

        return Ok(transaction);
    }

    // Add transaction
    [HttpPost("credit-points-transactions")]
    public async Task<IActionResult> AddTransaction(CreateTransactionRequest transactionRequest)
    {
        await transactionService.AddTransactionAsync(transactionRequest);
        return StatusCode(StatusCodes.Status201Created, "Credit points transaction created");
    }

    // Get all transactions made by customerId and restaurantId
    [HttpGet("customers/{customerId}/restaurants/{restaurantId}/credit-points-transactions")]
    public async Task<IActionResult> GetTransactionsByCustomer(int customerId, int restaurantId)
    {
        var transactions =
            await transactionService.GetTransactionsByCustomerAndRestaurantAsync(customerId, restaurantId);
        if (transactions.IsNullOrEmpty())
        {
            return NotFound();
        }

        return Ok(transactions);
    }

    [HttpGet("customers/{customerId}/restaurants/{restaurantId}/total-credit-points")]
    public async Task<IActionResult> GetTotalPoints(int customerId, int restaurantId)
    {
        var totalPoints = await transactionService.GetCustomerPointsAsync(customerId, restaurantId);
        return Ok(totalPoints);
    }
}