using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyApi.RequestModels
{
    public class LoginRequestBody
    {
        public string? Email { get; set; }
        public required string Password { get; set; }
        public required string RestaurantId { get; set; }

        public string? PhoneNumber { get; set; }
    }
}