using System.Security.Claims;
using LoyaltyApi.Models;
using LoyaltyApi.Repositories;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Utilities;

namespace LoyaltyApi.Services
{
    public class VoucherService(IVoucherRepository voucherRepository, VoucherUtility voucherUtility, IRestaurantRepository restaurantRepository, IHttpContextAccessor httpContext) : IVoucherService
    {
        public async Task<Voucher> CreateVoucherAsync(CreateVoucherRequest voucherRequest)
        {
            int customerId = int.Parse(httpContext.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? httpContext.HttpContext.User?.FindFirstValue("sub") ?? throw new ArgumentException("Customer not found"));
            int restaurantId = int.Parse(httpContext.HttpContext.Items["restaurantId"]?.ToString() ?? throw new ArgumentException("Restaurant not found"));
            int voucherValue = voucherUtility.CalculateVoucherValue(voucherRequest.Points, restaurantRepository.GetRestaurantInfo(restaurantId).Result.CreditPointsSellingRate);
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
            return await voucherRepository.GetUserVouchersAsync(customerId, restaurantId);
        }

        public async Task<Voucher> GetVoucherAsync(int voucherId, int restaurantId)
        {
            return await voucherRepository.GetVoucherAsync(voucherId, restaurantId);
        }
    }
}