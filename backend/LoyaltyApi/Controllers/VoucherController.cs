using LoyaltyApi.Filters;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/voucher")]
    public class VoucherController(IVoucherService voucherService, ILogger<VoucherController> logger) : ControllerBase
    {
        private readonly IVoucherService voucherService = voucherService;

        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<ActionResult> CreateVoucher([FromBody] CreateVoucherRequest voucherRequest)
        {
            try
            {
                Voucher voucher = await voucherService.CreateVoucherAsync(voucherRequest);
                return Ok(voucher.Code);
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex.Message);
                return StatusCode(500, ex.Message);
            }



        }
    }
}