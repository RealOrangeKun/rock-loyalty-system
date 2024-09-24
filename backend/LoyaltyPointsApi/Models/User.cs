using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyPointsApi.Models
{
    public class User
    {
        public int CustomerId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int RestaurantId { get; set; }

        public int LoyaltyPoints { get; set; }
    }
}