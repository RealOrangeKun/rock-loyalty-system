using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyPointsApi.Models
{
    public class UpdateRestaurantRequestModel
    {
        public  double PointsRate { get; set; }

        public  int ThresholdsNumber { get; set; }

        public   int PointsLifeTime { get; set; }
    }
}