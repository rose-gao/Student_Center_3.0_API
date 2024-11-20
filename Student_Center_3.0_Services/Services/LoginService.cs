using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Student_Center_3._0_Services.DTOs;


namespace Student_Center_3._0_Services.Services
{
    public class LoginService
    {
        private readonly HttpClient _httpClient;

        public LoginService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> ValidateUserCredentials(string userId, string password)
        {
            var encryptedPassword = EncryptPassword(password);

            // Call the database API to fetch the login record
            var response = await _httpClient.GetAsync($"api/Login/{userId}");
            if (!response.IsSuccessStatusCode)
            {
                return -1; // User not found or other issue
            }

            // Deserialize the response to get the login record
            var loginRecord = await response.Content.ReadFromJsonAsync<LoginDTO>();
            if (loginRecord == null)
            {
                return -1; // No login record found
            }

            // Check if the encrypted password matches the one in the database
            if (loginRecord.password == encryptedPassword)
            {
                return loginRecord.userNum;
            }
            else
            {
                return -1; // Password mismatch
            }
        }

        private string EncryptPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
