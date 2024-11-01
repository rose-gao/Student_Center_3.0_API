using System.Security.Cryptography;
using System.Text;

namespace Student_Center_3._0_Database.Utils
{
    internal class EncryptionUtils
    {
        internal static string Encrypt(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}

/*
 * using System.Security.Cryptography;
using System.Text;

namespace Student_Center_3._0_Database.Utils
{
    internal class EncryptionUtils
    {
        private readonly ILogger<EncryptionUtils> _logger;
        private readonly IConfiguration _configuration;

        public EncryptionUtils(ILogger<EncryptionUtils> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        internal string EncryptSHA256(string s)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        internal string EncryptAES(string s)
        {
            var key = Convert.FromBase64String(_configuration["AESConfig:Key"]);
            var iv = Convert.FromBase64String(_configuration["AESConfig:IV"]);

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(s);
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }

            }
        }
    }
}
*/