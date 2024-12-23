using System.Security.Cryptography;
using System.Text;

namespace SIHOT.Wallet.API.Models
{
    public class BasePass
    {
        public static string GENERIC_TYPE_PASS = "GENERIC_PASS";
        public required string Hotel { get; set; }
        public string? PassType { get; set; }
        public required string HotelName { get; set; }
        public required string Address { get; set; }

        private string? _cachedSerialNumber;

        public string SerialNumber
        {
            get
            {
                if (_cachedSerialNumber == null)
                {
                    _cachedSerialNumber = GetHashInput();
                }
                return _cachedSerialNumber;
            }
        }

        protected virtual string GetHashInput()
        {
            return $"EQR";
        }

        public static string ComputeSHA256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
