using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyApi.Models;

namespace LoyaltyApi.Services
{
    public interface IPointsService
    {
        Task <int> GetTotalPointsAsync(int customerId, int restaurantId);

        Task<Points> GetPointsRecordAsync(int customerId, int restaurantId, int transactionId);

        Task<Points> CreatePointsAsync(Points points);
    }
}