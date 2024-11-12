using System.Security.Cryptography;
using System.Text;

namespace Aff12B.Ext.API.Infastructure.Extentions
{
    public static class HashExtentions
    {
        public static string Sha256(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

    }
}
