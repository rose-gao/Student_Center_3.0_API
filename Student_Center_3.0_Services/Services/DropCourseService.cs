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
            if (userNum <= 0 || courseNum <= 0)
            {
                throw new ArgumentException("UserNum and CourseNum must be greater than zero.");
            }

            // GET MAIN COURSE TO DELETE
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

            // GET LIST OF COURSES TO DELETE (ie. incl. associated LAB + TUT)
            var enrollmentResponse = await _httpClient.GetAsync($"api/StudentCourseEnrollment/user/{userNum}");
            if (!enrollmentResponse.IsSuccessStatusCode)
            {
                return $"{enrollmentResponse.StatusCode}";
            }

            var enrollmentRecords = await enrollmentResponse.Content.ReadFromJsonAsync<List<StudentCourseEnrollmentDTO>>();
            string error = "";

            foreach (var enrollmentRecord in enrollmentRecords) {
                // Drop main course
                if (enrollmentRecord.courseNum == courseRecord.courseNum)
                {
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
                        error += $"{enrollmentRecord.courseNum} failed to drop: {updateCourseResponse.StatusCode} ";
                    }

                }
            
                // Drop auxilliary Labs/Tutorials (no checking if suffixes equal as the course and their labs/tuts will always be part of the same semester)
                else if (enrollmentRecord.courseName == courseRecord.courseName)
                {
                    string removeResponse = await RemoveCourseEnrollment(userNum, enrollmentRecord.courseNum);
                    if (removeResponse != "OK")
                    {
                        error += $"{enrollmentRecord.courseNum} failed to drop: {removeResponse} ";
                    }
                }
            }

            if (error.Length > 0)
            {
                return error;
            }

            return "OK";

        }

        // HELPER: method to fully drop a course, incl. updating course enrollment numbers
        private async Task<string> RemoveCourseEnrollment(int userNum, int courseNum) {
            // Delete course enrollment
            var dropResponse = await _httpClient.DeleteAsync($"api/StudentCourseEnrollment/{userNum}/{courseNum}");
            if (!dropResponse.IsSuccessStatusCode)
            {
                return $"{dropResponse.StatusCode}";
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

            return "OK";
        }

    }

}
