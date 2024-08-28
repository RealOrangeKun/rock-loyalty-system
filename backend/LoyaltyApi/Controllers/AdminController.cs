using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController(IVoucherService voucherService, ICreditPointsTransactionService pointsTransactionService) : ControllerBase
    {
        [HttpGet]
        [Route("vouchers")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetVoucherLongCode([FromQuery] string shortCode, [FromQuery] int customerId, [FromQuery] int restaurantId)
        {

            Voucher voucher = await voucherService.GetVoucherAsync(customerId, restaurantId, shortCode);
            return Ok(voucher.LongCode);
        }
        [HttpPost]
        [Route("transactions")]
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