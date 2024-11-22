using Student_Center_3._0_Services.DTOs;
using System.Text;
using System.Text.Json;

namespace Student_Center_3._0_Services.Services
{
    public class DropCourseService
    {
        private readonly HttpClient _httpClient;

        public DropCourseService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> DropCourse(int userNum, int courseNum)
        {
            if (userNum <= 0 || courseNum <= 0)
            {
                throw new ArgumentException("UserNum and CourseNum must be greater than zero.");
            }

            // Delete course enrollment
            var enrollmentResponse = await _httpClient.DeleteAsync($"api/StudentCourseEnrollment/{userNum}/{courseNum}");
            if (!enrollmentResponse.IsSuccessStatusCode)
            {
                return $"{enrollmentResponse.StatusCode}";
            }

            // DECREMENT NUMBER OF ENROLLED STUDENTS IN THE COURSE
            var courseResponse = await _httpClient.GetAsync($"api/Course/{courseNum}");
            if (!courseResponse.IsSuccessStatusCode)
            {
                return $"{courseResponse.StatusCode}";
            }

            var courseRecord = await courseResponse.Content.ReadFromJsonAsync<CourseDTO>();
            if (courseRecord == null)
            {
                return "Course not found.";
            }

            // Ensure numEnrolled does not go below zero
            if (courseRecord.numEnrolled > 0)
            {
                courseRecord.numEnrolled -= 1;
            }
            else
            {
                return "Cannot drop course; no students are currently enrolled.";
            }

            // Update course enrollment
            var content = new StringContent(courseRecord.numEnrolled.ToString(), Encoding.UTF8, "application/json");
            var updateCourseResponse = await _httpClient.PatchAsync($"api/Course/{courseNum}/update-enrollment", content);


            if (!updateCourseResponse.IsSuccessStatusCode)
            {
                return $"{updateCourseResponse.StatusCode}";
            }

            return "Course successfully dropped.";
        }

    }

}
