using LoyaltyApi.Models;
using LoyaltyApi.Repositories;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Services
{
    public class VoucherService (IVoucherRepository voucherRepository, VoucherUtility voucherUtility , IRestaurantRepository restaurantRepository): IVoucherService
    {
        public async Task<Voucher> CreateVoucherAsync([FromBody] CreateVoucherRequest voucherRequest)
        {

           int voucherValue = voucherUtility.CalculateVoucherValue(voucherRequest.Points,restaurantRepository.GetRestaurantInfo(voucherRequest.RestaurantId).Result.CreditPointsSellingRate); 
             Voucher voucher = new()
            {
                RestaurantId = voucherRequest.RestaurantId,
                CustomerId = voucherRequest.CustomerId,
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