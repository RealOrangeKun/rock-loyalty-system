using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyPointsApi.Models
{
    public class User
    {
        public int CustomerId { get; set; }
        public int RestaurantId { get; set; }

        public string Rank { get; set; }

        public int LoyaltyPoints { get; set; }
    }
}