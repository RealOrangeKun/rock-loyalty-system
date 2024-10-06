using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyPointsApi.Models
{
    public class LoyaltyPoints
    {
        public int TransactionId { get; set; }

        public int ReceiptId { get; set; }

        public int CustomerId { get; set; }

        public int RestaurantId { get; set; }

        public int Points { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public DateTime ExpiryDate { get; set; }

        public bool IsExpired { get; set; } = false;

        public RestaurantSettings Restaurant { get; set; }
    }
}