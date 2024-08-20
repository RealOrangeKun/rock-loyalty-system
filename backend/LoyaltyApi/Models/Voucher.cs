using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LoyaltyApi.Models
{
    public class Voucher
    {
        [Key]
        public int? Id { get; set; }
        [ForeignKey("Restaurant")]
        public required int RestaurantId { get; set; }
        public required int CustomerId { get; set; }

        public string? Code { get; set; }
        public required int Value { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsUsed { get; set; }
    }
}