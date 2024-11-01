using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Student_Center_3._0_Services.DTOs;

namespace Student_Center_3._0_Services.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserDTO> GetUserInformation(int userNum)
        {
            var response = await _httpClient.GetAsync($"api/User/{userNum}");

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }

            var user = JsonSerializer.Deserialize<UserDTO>(content);

            if (user != null)
            {
                user.socialInsuranceNum = $"*****{user.socialInsuranceNum[^4..]}";
            }

            return user;
        }
    }
}
