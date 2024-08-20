using LoyaltyApi.Models;

namespace LoyaltyApi.Repositories
{
    public interface IVoucherRepository
    {
        Task <Voucher> CreateVoucherAsync (Voucher voucher);

        Task<Voucher> GetVoucherAsync(int voucherId , int restaurantId);

        Task<IEnumerable<Voucher>> GetUserVouchersAsync(int customerId, int restaurantId);

    }
}