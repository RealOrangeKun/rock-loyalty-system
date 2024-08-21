namespace LoyaltyApi.Utilities
{
    public class VoucherUtility
    {
        public int CalculateVoucherValue(int points, double ratio) => (int)(points * ratio);
    }
}