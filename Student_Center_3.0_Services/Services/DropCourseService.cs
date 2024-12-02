using Student_Center_3._0_Services.DTOs;
using System.Runtime.CompilerServices;
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
            
            // CHECK IF STUDENT IS ENROLLED IN THE COURSE
            var isEnrolled = await _httpClient.GetAsync($"api/StudentCourseEnrollment/{userNum}/{courseNum}");
            if (!isEnrolled.IsSuccessStatusCode)
            {
                return $"Student is not enrolled in course";
            }

            var courseRecord = await isEnrolled.Content.ReadFromJsonAsync<StudentCourseEnrollmentDTO>();

            // GET LIST OF COURSES TO DELETE (including associated LABs/TUTs)
            var enrollmentResponse = await _httpClient.GetAsync($"api/StudentCourseEnrollment/user/{userNum}");
            if (!enrollmentResponse.IsSuccessStatusCode)
            {
                return $"Failed to fetch enrollments for user {userNum}: {enrollmentResponse.StatusCode}";
            }

            var enrollmentRecords = await enrollmentResponse.Content.ReadFromJsonAsync<List<StudentCourseEnrollmentDTO>>();
            if (enrollmentRecords == null || !enrollmentRecords.Any())
            {
                return $"No courses found for user {userNum}.";
            }

            return await ProcessCourseDrops(userNum, courseRecord, enrollmentRecords);

        }
        
        // HELPER: delete all courses in list
        internal async Task<string> ProcessCourseDrops(int userNum, StudentCourseEnrollmentDTO courseRecord, List<StudentCourseEnrollmentDTO> enrollmentRecords)
        { 
            var errors = new List<string>();

            foreach (var enrollmentRecord in enrollmentRecords)
            {
                if (enrollmentRecord.courseName == courseRecord.courseName && enrollmentRecord.courseSuffix == courseRecord.courseSuffix)
                {
                    var dropResponse = await DropSingleCourse(userNum, enrollmentRecord.courseNum);
                    if (dropResponse != "OK")
                    {
                        errors.Add($"Failed to drop course {enrollmentRecord.courseNum}: {dropResponse}");
                    }
                }
            }

            return errors.Any() ? string.Join("; ", errors) : "OK";
        }

        internal async Task<string> DropSingleCourse(int userNum, int courseNum)
        {
            if (userNum <= 0 || courseNum <= 0)
            {
                return "Invalid input.";
            }

            // DELETE COURSE ENROLLMENT
            var dropResponse = await _httpClient.DeleteAsync($"api/StudentCourseEnrollment/{userNum}/{courseNum}");
            if (!dropResponse.IsSuccessStatusCode)
            {
                return $"Failed to delete enrollment: {dropResponse.StatusCode}";
            }

            // FETCH COURSE
            var courseResponse = await _httpClient.GetAsync($"api/Course/{courseNum}");
            if (!courseResponse.IsSuccessStatusCode)
            {
                return $"Failed to fetch course {courseNum}: {courseResponse.StatusCode}";
            }

            var courseRecord = await courseResponse.Content.ReadFromJsonAsync<CourseDTO>();
            if (courseRecord == null)
            {
                return $"Course {courseNum} not found.";
            }

            // UPDATE COURSE ENROLLMENT
            return await UpdateCourseEnrollment(courseRecord, -1);
        }

        internal async Task<string> UpdateCourseEnrollment(CourseDTO courseRecord, int change)
        {
            if (courseRecord.numEnrolled + change < 0)
            {
                return "Cannot reduce enrollment below zero.";
            }

            courseRecord.numEnrolled += change;

            var content = new StringContent(courseRecord.numEnrolled.ToString(), Encoding.UTF8, "application/json");
            var updateResponse = await _httpClient.PatchAsync($"api/Course/{courseRecord.courseNum}/update-enrollment", content);

            if (!updateResponse.IsSuccessStatusCode)
            {
                return $"Failed to update course {courseRecord.courseNum}: {updateResponse.StatusCode}";
            }

            return "OK";
        }


    }

}
