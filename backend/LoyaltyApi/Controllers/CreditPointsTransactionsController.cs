using Microsoft.AspNetCore.Mvc;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;


namespace LoyaltyApi.Controllers;

[Route("api")]
[ApiController]
public class CreditPointsTransactionController(ICreditPointsTransactionService transactionService) : ControllerBase
{
    // Get transaction by transaction id
    [HttpGet]
    [Route("admin/credit-points-transactions/{transactionId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetTransactionById(int transactionId)
    {
        var transaction = await transactionService.GetTransactionByIdAsync(transactionId);
        if (transaction == null)
        {
            return NotFound();
        }

        return Ok(transaction);
    }

    // Get transaction by receipt id
    [HttpGet]
    [Route("admin/credit-points-transactions/receipt/{receiptId}")]
    [Authorize(Roles = "Admin")]
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
    [HttpPost]
    [Route("admin/credit-points-transactions")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddTransaction(CreateTransactionRequest transactionRequest)
    {
        await transactionService.AddTransactionAsync(transactionRequest);
        return StatusCode(StatusCodes.Status201Created, "Credit points transaction created");
    }

    // Get all transactions made by customerId and restaurantId
    [HttpGet]
    [Route("customers/{customerId}/restaurants/{restaurantId}/credit-points-transactions")]
    [Authorize(Roles = "Admin, User")]
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
}