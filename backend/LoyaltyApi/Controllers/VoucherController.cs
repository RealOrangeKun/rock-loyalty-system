using LoyaltyApi.Filters;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using LoyaltyApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/voucher")]
    public class VoucherController(IVoucherService voucherService) : ControllerBase
    {
        private readonly IVoucherService voucherService = voucherService;

        [HttpPost]
        [Route("")]
        [ServiceFilter(typeof(ExtractDataFromTokenFilter))]
        public async Task<ActionResult> CreateVoucher([FromBody] CreateVoucherRequest voucherRequest)
        {
            if (voucherRequest == null) return BadRequest("Voucher request is null");

            Voucher voucher = await voucherService.CreateVoucherAsync(voucherRequest);
            return Ok(voucher.ShortCode);

        }
    }
}