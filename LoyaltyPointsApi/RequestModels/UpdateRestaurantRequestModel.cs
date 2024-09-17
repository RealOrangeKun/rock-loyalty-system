using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyPointsApi.Models
{
    public class UpdateRestaurantRequestModel
    {
        public required double PointsRate { get; set; }

        public required int ThresholdsNumber { get; set; }

        public required  int PointsLifeTime { get; set; }
    }
}