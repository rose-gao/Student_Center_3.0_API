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

        public async Task<string> AddUser(UserLoginDTO userLoginDTO)
        {
            if (userLoginDTO == null)
            {
                return "User information cannot be empty.";
            }

            // CHECK THAT userNum IS VALID NUMBER OF DIGITS
            if (userLoginDTO.userNum <= 0 || userLoginDTO.userNum.ToString().Length != 9)
            {
                return "User Number must be 9 digits long.";
            }

            // CHECK VALIDITY OF NAMES
            if (!userLoginDTO.firstName.All(char.IsLetter))
            {
                return "First name must only contain letters and be less than 60 characters.";
            }

            if (!string.IsNullOrWhiteSpace(userLoginDTO.middleName) && !userLoginDTO.middleName.All(char.IsLetter))
            {
                return "Middle name must only contain letters and be less than 60 characters.";
            }

            if (!userLoginDTO.lastName.All(char.IsLetter))
            {
                return "Last name must only contain letters and be less than 60 characters.";
            }

            // CHECK SOCIAL INSURANCE NUMBER (SSN)
            if (!userLoginDTO.socialInsuranceNum.All(char.IsDigit))
            {
                return "SSN must be 9 digits long.";
            }

            // CHECK BIRTHDAY FORMAT AND VALIDITY
            if (!DateTime.TryParseExact(userLoginDTO.birthday, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var birthday) ||
                birthday > DateTime.Now)
            {
                return "Birthday must be in dd/MM/yyyy format and cannot be in the future.";
            }

            // CHECK EMAIL FORMAT
            if (!Regex.IsMatch(userLoginDTO.email, @"^[a-zA-Z0-9._%+-]+@uwo\.ca$"))
            {
                return "Invalid email format. Must be a UWO email address.";
            }

            // CHECK PHONE NUMBER FORMAT
            if (!userLoginDTO.phoneNum.All(char.IsDigit))
            {
                return "Phone number must be exactly 10 digits.";
            }

            // CHECK PROVINCE
            var validProvinces = new HashSet<string> { "ON", "BC", "AB", "MB", "SK", "QC", "NB", "NS", "PE", "NL", "YT", "NT", "NU" };
            if (!validProvinces.Contains(userLoginDTO.province))
            {
                return "Invalid province code.";
            }

            // CHECK POSTAL CODE
            if (!Regex.IsMatch(userLoginDTO.postalCode, @"^[A-Za-z]\d[A-Za-z] \d[A-Za-z]\d$"))
            {
                return "Invalid postal code format.";
            }

            UserDTO userDTO = new UserDTO
            {
                userNum = userLoginDTO.userNum,
                firstName = userLoginDTO.firstName,
                middleName = userLoginDTO.middleName,
                lastName = userLoginDTO.lastName,
                birthday = userLoginDTO.birthday,
                socialInsuranceNum = userLoginDTO.socialInsuranceNum,
                email = userLoginDTO.email,
                phoneNum = userLoginDTO.phoneNum,
                streetAddress = userLoginDTO.streetAddress,
                city = userLoginDTO.city,
                province = userLoginDTO.province,
                postalCode = userLoginDTO.postalCode,
                isAdmin = userLoginDTO.isAdmin

            };

            LoginDTO loginDTO = new LoginDTO
            {
                userId = userLoginDTO.userId,
                password = userLoginDTO.password,
                userNum = userLoginDTO.userNum

            };

            // MAKE API CALL TO ADD USER
            var userResponse = await _httpClient.PostAsJsonAsync("api/User", userDTO);
            var userContent = await userResponse.Content.ReadAsStringAsync();

            if (!userResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Error adding user: {userContent}");
            }

            // MAKE API CALL TO ADD USER LOGIN
            var loginResponse = await _httpClient.PostAsJsonAsync("api/Login", loginDTO);
            var loginContent = await loginResponse.Content.ReadAsStringAsync();
            if (!loginResponse.IsSuccessStatusCode)
            {
                // rollback addition of user
                var deleteResponse = await _httpClient.DeleteAsync($"api/User/{userDTO.userNum}");
                if (!deleteResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"Error adding login and rollback failed: {await deleteResponse.Content.ReadAsStringAsync()}");
                }
                throw new Exception($"Error adding login information: {loginContent}");
            }



            return "OK";
        }

    }
}
