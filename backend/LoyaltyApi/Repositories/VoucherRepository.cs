using System.Data;
using System.Data.Common;
using System.Text;
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
            using HttpClient client = new();
            var body = new
            {
                DTL = new[]
                {
                    new
                        {
                            VOCHNO = 1,// This is constant do not change it
                            VOCHVAL = voucher.Value
                        }
                },
                CNO = voucher.CustomerId.ToString(),
                CCODE = "C"
            };
            string jsonBody = JsonSerializer.Serialize(voucher);
            StringContent content = new(jsonBody, Encoding.UTF8, "application/json");


            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = string.Equals(envName, "Development", StringComparison.OrdinalIgnoreCase);

            var apiKey = await apiUtility.GetApiKey(isDevelopment ? "600" : voucher.RestaurantId.ToString() ?? throw new ArgumentException("RestaurantId cannot be null"));

            client.DefaultRequestHeaders.Add("XApiKey", apiKey);

            var result = await client.PostAsync($"{apiOptions.Value.BaseUrl}/api/HISCMD/ADDVOC", content);
            string jsonResponse = await result.Content.ReadAsStringAsync();
            logger.LogCritical(jsonResponse);

            var codes = JsonSerializer.Deserialize<string[]>(jsonResponse);


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