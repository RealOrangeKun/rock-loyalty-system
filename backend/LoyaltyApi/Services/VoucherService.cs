using LoyaltyApi.Models;
using LoyaltyApi.Repositories;

namespace LoyaltyApi.Services
{
    public class VoucherService (IVoucherRepository voucherRepository): IVoucherService
    {
        public async Task<Voucher> CreateVoucherAsync(Voucher voucher)
        {
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