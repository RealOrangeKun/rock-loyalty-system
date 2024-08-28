using LoyaltyApi.Exceptions;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class VoucherController(IVoucherService voucherService,
    ICreditPointsTransactionService pointsTransactionService,
    ILogger<VoucherController> logger) : ControllerBase
    {

        [HttpPost]
        [Route("vouchers")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> CreateVoucher([FromBody] CreateVoucherRequest voucherRequest)
        {
            try
            {
                Voucher voucher = await voucherService.CreateVoucherAsync(voucherRequest);
                await pointsTransactionService.SpendPointsAsync(voucher.CustomerId, voucher.RestaurantId, voucherRequest.Points);
                return StatusCode(StatusCodes.Status201Created, voucher.ShortCode);
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
                logger.LogCritical(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet]
        [Route("vouchers")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> GetVocher([FromQuery] string? shortCode)
        {
            try
            {
                if (shortCode is null)
                {
                    var vochers = await voucherService.GetUserVouchersAsync(null, null);
                    return Ok(vochers.Select(v => new
                    {
                        v.ShortCode,
                        v.IsUsed,
                        v.Value
                    }));
                }
                var voucher = await voucherService.GetVoucherAsync(null, null, shortCode);
                var result = new
                {
                    voucher.Value,
                    voucher.IsUsed
                };
                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet]
        [Route("admin/vouchers")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetVoucherLongCode([FromQuery] string shortCode, [FromQuery] int customerId, [FromQuery] int restaurantId)
        {
            Voucher voucher = await voucherService.GetVoucherAsync(customerId, restaurantId, shortCode);
            return Ok(voucher.LongCode);
        }
    }
}