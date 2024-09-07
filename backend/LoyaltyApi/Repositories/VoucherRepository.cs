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
    public class VoucherRepository(RockDbContext dbContext,
    ApiUtility apiUtility,
    VoucherUtility voucherUtility,
    ILogger<VoucherRepository> logger) : IVoucherRepository
    {
        public async Task<Voucher> CreateVoucherAsync(Voucher voucher)
        {
            var apiKey = await apiUtility.GetApiKey(voucher.RestaurantId.ToString());

            string result = await apiUtility.GenerateVoucher(voucher, apiKey);

            voucher.LongCode = result;

            voucher.ShortCode = voucherUtility.ShortenVoucherCode(result);

            await dbContext.Vouchers.AddAsync(voucher);

            await dbContext.SaveChangesAsync();

            logger.LogInformation("Voucher {ShortCode} created successfully", voucher.ShortCode);

            return voucher;
        }

        public async Task<IEnumerable<Voucher>> GetUserVouchersAsync(int customerId, int restaurantId)
        {
            logger.LogInformation("Getting vouchers for customer {CustomerId} and restaurant {RestaurantId}", customerId, restaurantId);
            return await dbContext.Vouchers.Where(v => v.CustomerId == customerId && v.RestaurantId == restaurantId).ToListAsync();
        }

        public async Task<Voucher?> GetVoucherAsync(Voucher voucher)
        {
            logger.LogInformation("Getting voucher {ShortCode} for customer {CustomerId} and restaurant {RestaurantId}", voucher.ShortCode, voucher.CustomerId, voucher.RestaurantId);
            return await dbContext.Vouchers.Where(v => v.CustomerId == voucher.CustomerId && v.RestaurantId == v.RestaurantId && v.ShortCode == voucher.ShortCode).FirstOrDefaultAsync();
        }
    }
}