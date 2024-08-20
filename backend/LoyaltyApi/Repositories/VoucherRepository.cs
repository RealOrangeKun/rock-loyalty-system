using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyApi.Data;
using LoyaltyApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyApi.Repositories
{
    public class VoucherRepository(RockDbContext dbContext ) : IVoucherRepository
    {
        public async Task<Voucher> CreateVoucherAsync(Voucher voucher)
        {

            await dbContext.Vouchers.AddAsync(voucher);
            
            await dbContext.SaveChangesAsync();

            return voucher;
        }

        public async Task<IEnumerable<Voucher>> GetUserVouchersAsync(int customerId, int restaurantId)
        {
           return await dbContext.Vouchers.Where(v => v.CustomerId == customerId && v.RestaurantId == restaurantId).ToListAsync();

        }

        public async Task<Voucher> GetVoucherAsync(int voucherId, int restaurantId)
        {
            return await dbContext.Vouchers.FirstOrDefaultAsync(v => v.Id == voucherId && v.RestaurantId == restaurantId);
        }
    }
}