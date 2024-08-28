using System.Data;
using System.Text.Json;
using LoyaltyApi.Config;
using LoyaltyApi.Data;
using LoyaltyApi.Models;
using LoyaltyApi.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LoyaltyApi.Repositories
{
    public class VoucherRepository(RockDbContext dbContext, ApiUtility apiUtility, VoucherUtility voucherUtility) : IVoucherRepository
    {
        public async Task<Voucher> CreateVoucherAsync(Voucher voucher)
        {
            var apiKey = await apiUtility.GetApiKey(voucher.RestaurantId.ToString());

            string result = await apiUtility.GenerateVoucher(voucher, apiKey);

            voucher.LongCode = result;

            voucher.ShortCode = voucherUtility.ShortenVoucherCode(result);

            await dbContext.Vouchers.AddAsync(voucher);

            await dbContext.SaveChangesAsync();

            return voucher;
        }

        public async Task<IEnumerable<dynamic>> GetUserVouchersAsync(Voucher voucher)
        {
            return await dbContext.Vouchers.Where(v => v.CustomerId == voucher.CustomerId && v.RestaurantId == voucher.CustomerId).Select(v => new { v.ShortCode, v.Value, v.IsUsed }).ToListAsync();
        }

        public async Task<Voucher?> GetVoucherAsync(Voucher voucher)
        {
            return await dbContext.Vouchers.Where(v => v.CustomerId == voucher.CustomerId && v.RestaurantId == v.RestaurantId && v.ShortCode == voucher.ShortCode).FirstOrDefaultAsync();
        }
    }
}