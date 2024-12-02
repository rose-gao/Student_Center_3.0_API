using System.Net.Http.Json;
using System.Text.Json;
using Student_Center_3._0_Services.DTOs;

namespace Student_Center_3._0_Services.Services
{
    public class ScheduleService
    {
        private readonly HttpClient _httpClient;

        public ScheduleService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ScheduleResponseDTO?> GetSchedule(int userNum)
        {
            // Fetch enrolled courses
            var enrollmentResponse = await _httpClient.GetAsync($"api/StudentCourseEnrollment/user/{userNum}");
            if (!enrollmentResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching enrollments: {enrollmentResponse.StatusCode}");
            }

            var enrollments = await enrollmentResponse.Content.ReadFromJsonAsync<List<StudentCourseEnrollmentDTO>>();
            if (enrollments == null || enrollments.Count == 0)
            {
                return null;
            }

            // Fetch and structure schedules
            var sem1schedule = new List<object>();
            var sem2schedule = new List<object>();

            foreach (var enrollment in enrollments)
            {
                var courseResponse = await _httpClient.GetAsync($"api/Course/{enrollment.courseNum}");
                var courseTimesResponse = await _httpClient.GetAsync($"api/CourseTime/course/{enrollment.courseNum}");

                if (!courseResponse.IsSuccessStatusCode || !courseTimesResponse.IsSuccessStatusCode)
                {
                    continue; // Skip this course if any API call fails
                }

                var course = await courseResponse.Content.ReadFromJsonAsync<CourseDTO>();
                var courseTimes = await courseTimesResponse.Content.ReadFromJsonAsync<List<CourseTimeDTO>>();

                if (course == null || courseTimes == null) continue;

                // Add courses to the correct semester
                if (course.startDate.Month == 9)
                {
                    AddCourseToSchedule(sem1schedule, enrollment, course, courseTimes);
                }

                if (course.endDate.Month == 4)
                {
                    AddCourseToSchedule(sem2schedule, enrollment, course, courseTimes);
                }
            }

            // Group schedules by weekday
            var groupedSem1Schedule = GroupScheduleByWeekday(sem1schedule);
            var groupedSem2Schedule = GroupScheduleByWeekday(sem2schedule);

            return new ScheduleResponseDTO
            {
                Semester1 = groupedSem1Schedule.ToList(),
                Semester2 = groupedSem2Schedule.ToList()
            };
        }

        private void AddCourseToSchedule(List<object> schedule, StudentCourseEnrollmentDTO enrollment, CourseDTO course, List<CourseTimeDTO> courseTimes)
        {
            foreach (var time in courseTimes)
            {
                schedule.Add(new
                {
                    Weekday = Enum.GetName(typeof(DayOfWeek), time.weekday),
                    StartTime = time.startTime,
                    EndTime = time.endTime,
                    CourseName = $"{enrollment.courseName}{enrollment.courseSuffix}",
                    Type = course.courseAlias.Contains("LAB", StringComparison.OrdinalIgnoreCase) ? "LAB" :
                           course.courseAlias.Contains("TUT", StringComparison.OrdinalIgnoreCase) ? "TUT" : "Lecture"
                });
            }
        }

        private IEnumerable<object> GroupScheduleByWeekday(IEnumerable<object> schedule)
        {
            return schedule
                .GroupBy(s => s.GetType().GetProperty("Weekday")?.GetValue(s, null))
                .Select(g => new
                {
                    Weekday = g.Key,
                    Classes = g.Select(c => new
                    {
                        StartTime = c.GetType().GetProperty("StartTime")?.GetValue(c, null),
                        EndTime = c.GetType().GetProperty("EndTime")?.GetValue(c, null),
                        CourseName = c.GetType().GetProperty("CourseName")?.GetValue(c, null),
                        Type = c.GetType().GetProperty("Type")?.GetValue(c, null)
                    }).ToList()
                });
        }

    }
}
