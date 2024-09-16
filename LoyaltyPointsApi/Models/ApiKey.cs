using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyPointsApi.Models
{
    public class ApiKey
    {
        public required string Key { get; set; }

        public required int RestaurantId { get; set; }


    }
}