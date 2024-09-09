using LoyaltyApi.Models;

namespace LoyaltyApi.Repositories
{
    public interface IVoucherRepository
    {
        Task<Voucher> CreateVoucherAsync(Voucher voucher, Restaurant restaurant);

        Task<Voucher?> GetVoucherAsync(Voucher voucher);

        Task<IEnumerable<Voucher>> GetUserVouchersAsync(int customerId, int restaurantId);

    }
}