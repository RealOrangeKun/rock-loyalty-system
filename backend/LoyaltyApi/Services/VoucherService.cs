using LoyaltyApi.Models;

namespace LoyaltyApi.Services
{
    public class VoucherService : IVoucherService
    {
        public Task<Voucher> CreateVoucherAsync(Voucher voucher)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Voucher>> GetUserVouchersAsync(int customerId, int restaurantId)
        {
            throw new NotImplementedException();
        }

        public Task<Voucher> GetVoucherAsync(int voucherId, int restaurantId)
        {
            throw new NotImplementedException();
        }
    }
}