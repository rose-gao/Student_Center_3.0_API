using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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

        public async Task<string> AddUser(UserDTO userDTO)
        {
            if (userDTO == null)
            {
                return "Student information cannot be empty.";
            }

            // CHECK THAT userNum IS VALID NUMBER OF DIGITS
            if (userDTO.userNum <= 0 || userDTO.userNum.ToString().Length != 9)
            {
                return "Student Number must be 9 digits long.";
            }

            // CHECK VALIDITY OF NAMES
            if (!userDTO.firstName.All(char.IsLetter))
            {
                return "First name must only contain letters and be less than 60 characters.";
            }

            if (!string.IsNullOrWhiteSpace(userDTO.middleName) && !userDTO.middleName.All(char.IsLetter))
            {
                return "Middle name must only contain letters and be less than 60 characters.";
            }

            if (!userDTO.lastName.All(char.IsLetter))
            {
                return "Last name must only contain letters and be less than 60 characters.";
            }

            // CHECK SOCIAL INSURANCE NUMBER (SSN)
            if (!userDTO.socialInsuranceNum.All(char.IsDigit))
            {
                return "SSN must be 9 digits long.";
            }

            // CHECK BIRTHDAY FORMAT AND VALIDITY
            if (!DateTime.TryParseExact(userDTO.birthday, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var birthday) ||
                birthday > DateTime.Now)
            {
                return "Birthday must be in dd/MM/yyyy format and cannot be in the future.";
            }

            // CHECK EMAIL FORMAT
            if (!Regex.IsMatch(userDTO.email, @"^[a-zA-Z0-9._%+-]+@uwo\.ca$"))
            {
                return "Invalid email format. Must be a UWO email address.";
            }

            // CHECK PHONE NUMBER FORMAT
            if (!userDTO.phoneNum.All(char.IsDigit))
            {
                return "Phone number must be exactly 10 digits.";
            }

            // CHECK PROVINCE
            var validProvinces = new HashSet<string> { "ON", "BC", "AB", "MB", "SK", "QC", "NB", "NS", "PE", "NL", "YT", "NT", "NU" };
            if (!validProvinces.Contains(userDTO.province))
            {
                return "Invalid province code.";
            }

            // CHECK POSTAL CODE
            if (!Regex.IsMatch(userDTO.postalCode, @"^[A-Za-z]\d[A-Za-z] \d[A-Za-z]\d$"))
            {
                return "Invalid postal code format.";
            }

            // MAKE API CALL TO ADD USER
            var response = await _httpClient.PostAsJsonAsync("api/User", userDTO);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error adding user: {content}");
            }

            return "OK";
        }

    }
}
