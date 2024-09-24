using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;

namespace LoyaltyPointsApi.RequestModels
{
    public class LoyaltyPointsTransactionRequestModel
    {
        public int TransactionId { get; set; }

        public int ReceiptId { get; set; }

        public required int CustomerId { get; set; }

        public required int RestaurantId { get; set; }

        public required int Points { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public DateTime ExpiryDate { get; set; }

        public bool IsExpired { get; set; } = false;

        public RestaurantSettings Restaurant { get; set; }
    }
}