using System.Security.Cryptography;
using System.Text;

namespace LoyaltyApi.Utilities
{
    public class VoucherUtility
    {
        public int CalculateVoucherValue(int points, double ratio) => (int)(points * ratio);

        public string ShortenVoucherCode(string voucherCode)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(voucherCode));

            StringBuilder hashString = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                hashString.Append(b.ToString("x2"));
            }

            return hashString.ToString().Substring(0, 5);
        }
    }
}