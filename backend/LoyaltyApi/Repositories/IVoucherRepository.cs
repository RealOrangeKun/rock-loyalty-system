using LoyaltyApi.Models;

namespace LoyaltyApi.Repositories
{
    public interface IVoucherRepository
    {
        Task<Voucher> CreateVoucherAsync(Voucher voucher);

        Task<dynamic?> GetVoucherAsync(Voucher voucher);

        Task<IEnumerable<dynamic>> GetUserVouchersAsync(Voucher voucher);

    }
}