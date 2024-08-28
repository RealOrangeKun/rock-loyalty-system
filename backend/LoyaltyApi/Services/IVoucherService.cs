using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;

namespace LoyaltyApi.Services
{
    public interface IVoucherService
    {
        Task<IEnumerable<Voucher>> GetUserVouchersAsync(int? customerId, int? restaurantId);

        Task<Voucher> GetVoucherAsync(int? customerId, int? restaurantId, string shortCode);



        Task<Voucher> CreateVoucherAsync(CreateVoucherRequest voucherRequest);
    }
}