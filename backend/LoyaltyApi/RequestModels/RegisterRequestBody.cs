using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyApi.RequestModels
{
    public class RegisterRequestBody
    {
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public required string Password { get; set; }
        public required int RestaurantId { get; set; }
        public required string Name { get; set; }
    }
}