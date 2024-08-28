using System.Security.Claims;
using LoyaltyApi.Exceptions;
using LoyaltyApi.Models;
using LoyaltyApi.Repositories;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Utilities;

namespace LoyaltyApi.Services
{
    public class VoucherService(IVoucherRepository voucherRepository,
    VoucherUtility voucherUtility,
    IRestaurantRepository restaurantRepository,
    IHttpContextAccessor httpContext,
    ICreditPointsTransactionRepository creditPointsTransactionRepository,
    ILogger<VoucherService> logger) : IVoucherService
    {
        public async Task<Voucher> CreateVoucherAsync(CreateVoucherRequest voucherRequest)
        {
            var user = httpContext.HttpContext?.User;
            int customerId = int.Parse(user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("customerId not found"));
            int restaurantId = int.Parse(user.FindFirst("restaurantId")?.Value ?? throw new ArgumentException("restaurantId not found"));
            var availablePoints = await creditPointsTransactionRepository.GetCustomerPointsAsync(customerId, restaurantId);
            if (availablePoints < voucherRequest.Points) throw new PointsNotEnoughException("Not enough points");
            double ratio = (await restaurantRepository.GetRestaurantInfo(restaurantId) ?? throw new ArgumentException("restaurant not found")).CreditPointsSellingRate;
            int voucherValue = voucherUtility.CalculateVoucherValue(voucherRequest.Points, ratio);
            if (voucherValue == 0) throw new MinimumPointsNotReachedException("Point used too low");
            Voucher voucher = new()
            {
                RestaurantId = restaurantId,
                CustomerId = customerId,
                Value = voucherValue
            };
            return await voucherRepository.CreateVoucherAsync(voucher);
        }

        public async Task<IEnumerable<dynamic>> GetUserVouchersAsync(int customerId, int restaurantId)
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
            return await voucherRepository.GetVoucherAsync(voucher) ?? throw new Exception("Voucher not found");
        }
    }
}