using LoyaltyApi.Exceptions;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/vouchers")]
    public class VoucherController(IVoucherService voucherService, ICreditPointsTransactionService pointsTransactionService) : ControllerBase
    {
        private readonly IVoucherService voucherService = voucherService;

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> CreateVoucher([FromBody] CreateVoucherRequest voucherRequest)
        {
            try
            {
                Voucher voucher = await voucherService.CreateVoucherAsync(voucherRequest);
                await pointsTransactionService.SpendPointsAsync(voucher.CustomerId, voucher.RestaurantId, voucherRequest.Points);
                return Ok(voucher.ShortCode);
            }
            catch (PointsNotEnoughException ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetVoucherLongCode([FromQuery] string shortCode, [FromQuery] int customerId, [FromQuery] int restaurantId)
        {
            try
            {
                Voucher voucher = await voucherService.GetVoucherAsync(customerId, restaurantId, shortCode);
                return Ok(voucher.LongCode);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}