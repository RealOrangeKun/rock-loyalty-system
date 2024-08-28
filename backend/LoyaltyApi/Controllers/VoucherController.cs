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
                return StatusCode(201, voucher.ShortCode);
            }
            catch (PointsNotEnoughException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (MinimumPointsNotReachedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> GetVocher([FromQuery] int customerId, [FromQuery] int restaurantId , [FromQuery] string shortCode){
            try
            {
              var voucher = await voucherService.GetVoucherAsync(customerId, restaurantId, shortCode);
              var result = new{
                voucher.Value,
                voucher.IsUsed
              };
              return Ok(result);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500,ex.Message);
            }
        }
        [HttpGet]
        [Route("user")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> GetUserVouchers([FromQuery] int customerId, [FromQuery] int restaurantId)
        {
            try
            {
                var vouchers = await voucherService.GetUserVouchersAsync(customerId, restaurantId);
                return Ok(vouchers.Select(v => new { v.ShortCode, v.Value, v.IsUsed }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}