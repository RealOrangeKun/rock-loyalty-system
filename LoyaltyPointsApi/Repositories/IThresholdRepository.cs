using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;
using Microsoft.AspNetCore.SignalR;

namespace LoyaltyPointsApi.Repositories
{
    public interface IThresholdRepository
    {
        Task<List<Threshold>> GetRestaurantThresholds(Threshold threshold);

        Task<Threshold> GetRestaurantThreshold(Threshold threshold);

        Task AddThreshold(Threshold threshold);

        Task UpdateThreshold(Threshold threshold);
    }
}