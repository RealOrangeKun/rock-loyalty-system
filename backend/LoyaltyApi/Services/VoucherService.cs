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
    ICreditPointsTransactionRepository creditPointsTransactionRepository,
    ILogger<VoucherService> logger) : IVoucherService
    {
        public async Task<Voucher> CreateVoucherAsync(CreateVoucherRequest voucherRequest, int customerId, int restaurantId)
        {
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

        public async Task<IEnumerable<Voucher>> GetUserVouchersAsync(int customerId, int restaurantId)
        {

            return await voucherRepository.GetUserVouchersAsync(customerId, restaurantId);
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