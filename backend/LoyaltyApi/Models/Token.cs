using System.ComponentModel.DataAnnotations;

namespace LoyaltyApi.Models
{
    public class Token
    {
        [Key]
        public required string TokenValue { get; set; }

        public required TokenType TokenType { get; set; }

        public required int CustomerId { get; set; }

        public required int RestaurantId { get; set; }

        public Token() { }
    }
}