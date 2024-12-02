using Student_Center_3._0_Services.DTOs;
using System.Text;
using System.Text.Json;

namespace Student_Center_3._0_Services.Services
{
    public class SwapCourseService
    {
        private readonly HttpClient _httpClient;
        private readonly AddCourseService _addCourseService;
        private readonly DropCourseService _dropCourseService;

        public SwapCourseService(HttpClient httpClient, AddCourseService addCourseService, DropCourseService dropCourseService)
        {
            _httpClient = httpClient;
            _addCourseService = addCourseService;
            _dropCourseService = dropCourseService;
        }

        public async Task<string> SwapCourse(int userNum, int dropCourseNum, List<int> addCourseNums)
        {
            var errors = new StringBuilder();

            // Fetch drop course record
            var dropRecord = await GetEnrollmentRecordAsync(userNum, dropCourseNum);
            if (dropRecord == null)
                return $"Failed to fetch course details for {dropCourseNum}.";

            // Fetch associated courses for dropping
            var coursesToDrop = await GetAssociatedCoursesAsync(userNum, dropRecord.courseName, dropRecord.courseSuffix);

            // Attempt to drop courses
            foreach (var course in coursesToDrop)
            {
                var dropResponse = await _httpClient.DeleteAsync($"api/StudentCourseEnrollment/{userNum}/{course.courseNum}");
                if (!dropResponse.IsSuccessStatusCode)
                {
                    errors.AppendLine($"Failed to drop course {course.courseNum}: {dropResponse.StatusCode}");
                    await RollbackAsync(userNum, coursesToDrop); // Rollback immediately
                    return errors.ToString();
                }
            }

            // Attempt to add new courses
            string addResponse = await _addCourseService.AddCourse(userNum, addCourseNums);
            if (addResponse != "OK")
            {
                await RollbackAsync(userNum, coursesToDrop); // Rollback drops, adds are already atomic
                return $"Failed to add courses: {addResponse}.";
            }

            // Decrement enrollment for dropped courses
            foreach (var course in coursesToDrop)
            {
                var courseDTO = await GetCourseAsync(course.courseNum);
                if (courseDTO == null)
                {
                    errors.AppendLine($"Failed to fetch course {course.courseNum} for enrollment update.");
                    continue;
                }

                string updateResponse = await _dropCourseService.UpdateCourseEnrollment(courseDTO, -1);
                if (updateResponse != "OK")
                {
                    errors.AppendLine($"Failed to update enrollment for course {course.courseNum}: {updateResponse}");
                }
            }

            return errors.Length > 0 ? errors.ToString() : "OK";
        }

        private async Task<StudentCourseEnrollmentDTO?> GetEnrollmentRecordAsync(int userNum, int courseNum)
        {
            var response = await _httpClient.GetAsync($"api/StudentCourseEnrollment/{userNum}/{courseNum}");
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<StudentCourseEnrollmentDTO>()
                : null;
        }

        private async Task<List<StudentCourseEnrollmentDTO>> GetAssociatedCoursesAsync(int userNum, string courseName, string courseSuffix)
        {
            var response = await _httpClient.GetAsync($"api/StudentCourseEnrollment/user/{userNum}");
            if (!response.IsSuccessStatusCode)
                return new List<StudentCourseEnrollmentDTO>();

            var enrollments = await response.Content.ReadFromJsonAsync<List<StudentCourseEnrollmentDTO>>();
            return enrollments?.Where(e => e.courseName == courseName && e.courseSuffix == courseSuffix).ToList()
                   ?? new List<StudentCourseEnrollmentDTO>();
        }

        private async Task<CourseDTO?> GetCourseAsync(int courseNum)
        {
            var response = await _httpClient.GetAsync($"api/Course/{courseNum}");
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<CourseDTO>()
                : null;
        }

        private async Task RollbackAsync(int userNum, List<StudentCourseEnrollmentDTO> coursesToDrop)
        {
            var errors = new List<string>();
            foreach (var course in coursesToDrop)
            {
                var rollbackResponse = await _httpClient.PostAsJsonAsync("api/StudentCourseEnrollment", course);

            }
        }
            
    }
}
