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
    public class VoucherRepository(RockDbContext dbContext, IOptions<API> apiOptions, ApiUtility apiUtility, ILogger<VoucherRepository> logger) : IVoucherRepository
    {
        public async Task<Voucher> CreateVoucherAsync(Voucher voucher)
        {
            var apiKey = await apiUtility.GetApiKey(voucher.RestaurantId.ToString());

            var result = await apiUtility.GenerateVoucher(voucher, apiKey);

            var codes = JsonSerializer.Deserialize<string[]>(result);

            voucher.Code = codes?.First() ?? throw new DataException("Voucher not created");

            await dbContext.Vouchers.AddAsync(voucher);

            await dbContext.SaveChangesAsync();

            return voucher;
        }

        public async Task<IEnumerable<Voucher>> GetUserVouchersAsync(int customerId, int restaurantId)
        {
            return await dbContext.Vouchers.Where(v => v.CustomerId == customerId && v.RestaurantId == restaurantId).ToListAsync();

        }

        public async Task<Voucher> GetVoucherAsync(int voucherId, int restaurantId)
        {
            return await dbContext.Vouchers.FirstOrDefaultAsync(v => v.Id == voucherId && v.RestaurantId == restaurantId) ?? throw new DataException("Voucher not found");
        }
    }
}