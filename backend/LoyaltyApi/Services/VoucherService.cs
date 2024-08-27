using System.Security.Claims;
using LoyaltyApi.Models;
using LoyaltyApi.Repositories;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Utilities;

namespace LoyaltyApi.Services
{
    public class VoucherService(IVoucherRepository voucherRepository, VoucherUtility voucherUtility, IRestaurantRepository restaurantRepository, IHttpContextAccessor httpContext, ILogger<VoucherService> logger) : IVoucherService
    {
        public async Task<Voucher> CreateVoucherAsync(CreateVoucherRequest voucherRequest)
        {
            var user = httpContext.HttpContext?.User;
            int customerId = int.Parse(user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("customerId not found"));
            int restaurantId = int.Parse(user.FindFirst("restaurantId")?.Value ?? throw new ArgumentException("restaurantId not found"));
            double ratio = (await restaurantRepository.GetRestaurantInfo(restaurantId)).CreditPointsSellingRate;
            int voucherValue = voucherUtility.CalculateVoucherValue(voucherRequest.Points, ratio);
            Voucher voucher = new()
            {
                RestaurantId = restaurantId,
                CustomerId = customerId,
                Value = voucherValue
            };
            return await voucherRepository.CreateVoucherAsync(voucher);


        }

        public async Task<IEnumerable<Voucher>> GetUserVouchersAsync(int customerId, int restaurantId)
        {
            Voucher voucher = new()
            {
                CustomerId = customerId,
                RestaurantId = restaurantId
            };
            return await voucherRepository.GetUserVouchersAsync(voucher);
        }

        public async Task<Voucher> GetVoucherAsync(int customerId, int restaurantId, string shortCode)
        {
            Voucher voucher = new()
            {
                RestaurantId = restaurantId,
                ShortCode = shortCode,
                CustomerId = customerId
            };
            return await voucherRepository.GetVoucherAsync(voucher);
        }
    }
}