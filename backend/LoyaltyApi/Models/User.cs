using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public required string Name { get; set; }
        public int RestaurantId { get; set; }

    }
}