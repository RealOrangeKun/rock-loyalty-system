using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;

namespace LoyaltyApi.Services
{
    public interface IVoucherService
    {
        Task<IEnumerable<Voucher>> GetUserVouchersAsync(int customerId, int restaurantId);

        Task<Voucher> GetVoucherAsync(int voucherId, int restaurantId);

        Task<Voucher> CreateVoucherAsync(CreateVoucherRequest voucherRequest);
    }
}