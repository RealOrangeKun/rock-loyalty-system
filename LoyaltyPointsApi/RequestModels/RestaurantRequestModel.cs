using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyPointsApi.RequestModels
{
    public class RestaurantRequestModel
    {
        public required int RestaurantId { get; set; }
        public required double PointsRate { get; set; }

        public required int ThresholdsNumber { get; set; }

        public required int PointsLifeTime { get; set; }
    }
}