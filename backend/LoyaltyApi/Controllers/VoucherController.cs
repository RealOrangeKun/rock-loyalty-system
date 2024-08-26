using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using LoyaltyApi.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/voucher")]
    public class VoucherController(IVoucherService voucherService) : ControllerBase
    {
        private readonly IVoucherService voucherService = voucherService;

        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreateVoucher([FromBody] CreateVoucherRequest voucherRequest)
        {
            if (voucherRequest == null) return BadRequest("Voucher request is null");

           Voucher voucher =  await voucherService.CreateVoucherAsync(voucherRequest);
            return Ok(voucher.Code);

        }
    }
}