using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyApi.Models;

namespace LoyaltyApi.Repositories
{
    public interface IPointsRepository
    {
        Task <List<Points>> GetTotalPointsAsync(int customerId, int restaurantId);
        Task <Points> GetPointsRecordAsync(int customerId, int restaurantId, int transactionId);

        Task <Points> CreatePointsAsync(Points points);
        Task <Points> UpdatePointsAsync(Points points);   
    }
}