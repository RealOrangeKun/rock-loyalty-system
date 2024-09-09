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
            logger.LogTrace("user: {user}", user);
            int customerId = int.Parse(user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("customerId not found"));
            logger.LogTrace("customerId: {customerId}", customerId);
            int restaurantId = int.Parse(user.FindFirst("restaurantId")?.Value ?? throw new ArgumentException("restaurantId not found"));
            logger.LogTrace("restaurantId: {restaurantId}", restaurantId);
            var availablePoints = await creditPointsTransactionRepository.GetCustomerPointsAsync(customerId, restaurantId);
            logger.LogTrace("availablePoints: {availablePoints}", availablePoints);
            if (availablePoints < voucherRequest.Points) throw new PointsNotEnoughException("Not enough points");
            Restaurant? restaurant = await restaurantRepository.GetRestaurantById(restaurantId) ?? throw new ArgumentException("restaurant not found");
            int voucherValue = voucherUtility.CalculateVoucherValue(voucherRequest.Points, restaurant.CreditPointsSellingRate);

            logger.LogTrace("voucherValue: {voucherValue}", voucherValue);
            if (voucherValue == 0) throw new MinimumPointsNotReachedException("Point used too low");
            Voucher voucher = new()
            {
                RestaurantId = restaurantId,
                CustomerId = customerId,
                Value = voucherValue,
                DateOfCreation = DateTime.Now
            };
            return await voucherRepository.CreateVoucherAsync(voucher, restaurant);
        }

        public async Task<IEnumerable<Voucher>> GetUserVouchersAsync(int? customerId, int? restaurantId)
        {
            int customerIdJwt = customerId ?? int.Parse(httpContext.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("customerId not found"));
            logger.LogTrace("customerIdJwt: {customerIdJwt}", customerIdJwt);
            int restaurantIdJwt = restaurantId ?? int.Parse(httpContext.HttpContext?.User?.FindFirst("restaurantId")?.Value ?? throw new ArgumentException("restaurantId not found"));
            logger.LogTrace("restaurantIdJwt: {restaurantIdJwt}", restaurantIdJwt);
            return await voucherRepository.GetUserVouchersAsync(customerId ?? customerIdJwt, restaurantId ?? restaurantIdJwt);
        }

        public async Task<Voucher> GetVoucherAsync(int? customerId, int? restaurantId, string shortCode)
        {
            int customerIdJwt = customerId ?? int.Parse(httpContext.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ArgumentException("customerId not found"));
            logger.LogTrace("customerIdJwt: {customerIdJwt}", customerIdJwt);
            int restaurantIdJwt = restaurantId ?? int.Parse(httpContext.HttpContext?.User?.FindFirst("restaurantId")?.Value ?? throw new ArgumentException("restaurantId not found"));
            logger.LogTrace("restaurantIdJwt: {restaurantIdJwt}", restaurantIdJwt);
            Voucher voucher = new()
            {
                RestaurantId = restaurantId ?? restaurantIdJwt,
                ShortCode = shortCode,
                CustomerId = customerId ?? customerIdJwt
            };
            return await voucherRepository.GetVoucherAsync(voucher) ?? throw new Exception("Voucher not found");
        }
    }
}