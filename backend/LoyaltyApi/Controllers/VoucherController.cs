using LoyaltyApi.Models;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/voucher")]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService voucherService;
        public VoucherController(IVoucherService voucherService)
        {
            this.voucherService = voucherService;
        }
        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreateVoucher([FromBody] Voucher voucher)
        {
            var createdVoucher = await voucherService.CreateVoucherAsync(voucher);
            return Ok(createdVoucher.Code);

        }
    }
}