using Student_Center_3._0_Services.DTOs;
using System.Text;
using System.Text.Json;

namespace Student_Center_3._0_Services.Services
{
    public class SwapCourseService
    {
        private readonly HttpClient _httpClient;
        private readonly AddCourseService _addCourseService;

        public SwapCourseService(HttpClient httpClient, AddCourseService addCourseService)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _addCourseService = addCourseService ?? throw new ArgumentNullException(nameof(addCourseService));
        }

        public async Task<string> SwapCourse(int userNum, int dropCourseNum, int addCourseNum)
        {
            if (userNum <= 0 || dropCourseNum <= 0 || addCourseNum <= 0)
            {
                throw new ArgumentException("UserNum, DropCourseNum, and AddCourseNum must be greater than zero.");
            }

            if (dropCourseNum == addCourseNum)
            {
                throw new ArgumentException("Cannot swap the same course.");
            }


            // Retrieve current enrollment record for rollback purposes
            var getEnrollmentResponse = await _httpClient.GetAsync($"api/StudentCourseEnrollment/{userNum}/{dropCourseNum}");
            if (!getEnrollmentResponse.IsSuccessStatusCode)
            {
                return $"Failed to retrieve enrollment for rollback--{getEnrollmentResponse.StatusCode}";
            }

            var droppedCourse = await getEnrollmentResponse.Content.ReadFromJsonAsync<StudentCourseEnrollmentDTO>();
            if (droppedCourse == null)
            {
                return "Failed to fetch dropped course details for rollback.";
            }

            // Temporarily delete course
            var dropResponse = await _httpClient.DeleteAsync($"api/StudentCourseEnrollment/{userNum}/{dropCourseNum}");
            if (!dropResponse.IsSuccessStatusCode)
            {
                return $"Failed to drop course--{dropResponse.StatusCode}";
            }

            // ADD NEW COURSE
            string addResponse = await _addCourseService.AddSingleCourse(userNum, addCourseNum);
            if (addResponse != "OK")
            {
                // Rollback drop operation
                var rollbackResponse = await _httpClient.PostAsJsonAsync("api/StudentCourseEnrollment", droppedCourse);
                return $"Failed to add course. Add Response: {addResponse}, Rollback Response: {rollbackResponse.StatusCode}";
            }

            // FINISH DROPPING-- Decrement enrollment in dropped course
            var courseResponse = await _httpClient.GetAsync($"api/Course/{dropCourseNum}");
            if (!courseResponse.IsSuccessStatusCode)
            {
                return $"Failed to fetch course details--{courseResponse.StatusCode}";
            }

            var courseRecord = await courseResponse.Content.ReadFromJsonAsync<CourseDTO>();
            if (courseRecord == null)
            {
                return "Dropped course record not found.";
            }

            if (courseRecord.numEnrolled > 0)
            {
                courseRecord.numEnrolled -= 1;
            }

            // Update course enrollment
            var content = new StringContent(courseRecord.numEnrolled.ToString(), Encoding.UTF8, "application/json");
            var updateCourseResponse = await _httpClient.PatchAsync($"api/Course/{dropCourseNum}/update-enrollment", content);

            if (!updateCourseResponse.IsSuccessStatusCode)
            {
                return $"Failed to update dropped course enrollment--{updateCourseResponse.StatusCode}";
            }

            return "OK";
        }
    }
}
