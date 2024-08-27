using LoyaltyApi.Models;

namespace LoyaltyApi.Repositories
{
    public interface IVoucherRepository
    {
        Task<Voucher> CreateVoucherAsync(Voucher voucher);

        Task<Voucher> GetVoucherAsync(Voucher voucher);

        Task<IEnumerable<Voucher>> GetUserVouchersAsync(Voucher voucher);

    }
}