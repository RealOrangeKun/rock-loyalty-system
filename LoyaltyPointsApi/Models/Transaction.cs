using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyPointsApi.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        public int Points  { get; set; } 

        public required int CustomerId { get; set; }

        public required int RestaurantId { get; set; }

        public DateTime TransactionDate { get; set; }
               
    }
}