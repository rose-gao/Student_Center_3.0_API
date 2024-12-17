using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Student_Center_3._0_Services.Services;
using Student_Center_3._0_Services.DTOs;
using Xunit.Abstractions;
using System.Text.Json;

namespace Student_Center_3._0_xUnitTests
{
    public class ScheduleServiceTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ScheduleServiceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        // GetSchedule should function correctly; also indirectly testing private helpers AddCourseToSchedule and GroupScheduleByWeekday
        [Fact]
        public async Task GetSchedule_ShouldReturnCorrectScheduleWhenCoursesEnrolled()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/"); // Set BaseAddress

            var scheduleService = new ScheduleService(httpClient);
            int userNum = 12345;

            // Mock the response for enrolled courses
            var enrolledCourses = new List<StudentCourseEnrollmentDTO>
            {
                new StudentCourseEnrollmentDTO { courseNum = 101, courseName = "Course 101", courseSuffix = "A" },
                new StudentCourseEnrollmentDTO { courseNum = 102, courseName = "Course 102", courseSuffix = "B" },
            };

            mockHttp
                .When(HttpMethod.Get, $"http://localhost/api/StudentCourseEnrollment/user/{userNum}")
                .Respond(JsonContent.Create(enrolledCourses));

            // Mock the response for course details
            mockHttp
                .When(HttpMethod.Get, "http://localhost/api/Course/101")
                .Respond(JsonContent.Create(new CourseDTO
                {
                    courseNum = 101,
                    courseAlias = "Math course",
                    courseName = "Course 101",
                    startDate = new DateTime(2024, 9, 1),
                    endDate = new DateTime(2024, 12, 1)
                }));

            mockHttp
                .When(HttpMethod.Get, "http://localhost/api/Course/102")
                .Respond(JsonContent.Create(new CourseDTO
                {
                    courseNum = 102,
                    courseAlias = "LAB",
                    courseName = "Course 102",
                    startDate = new DateTime(2024, 12, 1),
                    endDate = new DateTime(2025, 4, 1)
                }));

            // Mock the response for course times
            mockHttp
                .When(HttpMethod.Get, "http://localhost/api/CourseTime/course/101")
                .Respond(JsonContent.Create(new List<CourseTimeDTO>
                {
                    new CourseTimeDTO { weekday = 1, startTime = TimeSpan.Parse("10:00:00"), endTime = TimeSpan.Parse("12:00:00") }
                }));

            mockHttp
                .When(HttpMethod.Get, "http://localhost/api/CourseTime/course/102")
                .Respond(JsonContent.Create(new List<CourseTimeDTO>
                {
                    new CourseTimeDTO { weekday = 3, startTime = TimeSpan.Parse("13:00:00"), endTime = TimeSpan.Parse("15:00:00") }
                }));

            // Act
            var result = await scheduleService.GetSchedule(userNum);

            // Assert correct number of courses in each semester
            Assert.Single(result.Semester1);
            Assert.Single(result.Semester2);


            // CHECK VALIDITY OF SEMESTER 1
            var sem1Schedule = result.Semester1[0];
            Assert.Equal("Monday", sem1Schedule.GetType().GetProperty("Weekday")?.GetValue(sem1Schedule, null));


            // Access the "Classes" property 
            var classes = sem1Schedule.GetType().GetProperty("Classes")?.GetValue(sem1Schedule, null) as IEnumerable<object>;
            Assert.NotNull(classes);

            // Retrieve the first class in the "Classes" list
            var firstClass = classes.FirstOrDefault();
            Assert.NotNull(firstClass);

            // Access the "CourseName" property of the first class
            Assert.Equal("Course 101A", firstClass.GetType().GetProperty("CourseName")?.GetValue(firstClass, null));
            Assert.Equal(TimeSpan.Parse("10:00:00"), firstClass.GetType().GetProperty("StartTime")?.GetValue(firstClass, null));
            Assert.Equal(TimeSpan.Parse("12:00:00"), firstClass.GetType().GetProperty("EndTime")?.GetValue(firstClass, null));
            Assert.Equal("Lecture", firstClass.GetType().GetProperty("Type")?.GetValue(firstClass, null));


            // CHECK VALIDITY OF SEMESTER 2
            var sem2Schedule = result.Semester2[0];
            Assert.Equal("Wednesday", sem2Schedule.GetType().GetProperty("Weekday")?.GetValue(sem2Schedule, null));


            // Access the "Classes" property 
            classes = sem2Schedule.GetType().GetProperty("Classes")?.GetValue(sem2Schedule, null) as IEnumerable<object>;
            Assert.NotNull(classes);

            // Retrieve the first class in the "Classes" list
            firstClass = classes.FirstOrDefault();
            Assert.NotNull(firstClass);

            // Access the "CourseName" property of the first class
            Assert.Equal("Course 102B", firstClass.GetType().GetProperty("CourseName")?.GetValue(firstClass, null));
            Assert.Equal(TimeSpan.Parse("13:00:00"), firstClass.GetType().GetProperty("StartTime")?.GetValue(firstClass, null));
            Assert.Equal(TimeSpan.Parse("15:00:00"), firstClass.GetType().GetProperty("EndTime")?.GetValue(firstClass, null));
            Assert.Equal("LAB", firstClass.GetType().GetProperty("Type")?.GetValue(firstClass, null));

        }


        // Get Schedule should return empty lists if the student has no currently-enrolled courses
        [Fact]
        public async Task GetSchedule_ShouldReturnEmptySchedule_WhenEnrollmentResponseIsUnsuccessful()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/"); // Set BaseAddress

            var scheduleService = new ScheduleService(httpClient);
            int userNum = 12345;

            mockHttp
                .When(HttpMethod.Get, $"http://localhost/api/StudentCourseEnrollment/user/{userNum}")
                .Respond(HttpStatusCode.BadRequest);


            // Act
            var result = await scheduleService.GetSchedule(userNum);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Semester1); // Expecting Semester1 to be empty
            Assert.Empty(result.Semester2); // Expecting Semester2 to be empty

        }

        // Edge Case: GetSchedule should function correctly for full-year courses. The full year course should appear in the schedules of both semesters.
        [Fact]
        public async Task GetSchedule_ShouldReturnCorrectScheduleForFullYearCourse()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/"); // Set BaseAddress

            var scheduleService = new ScheduleService(httpClient);
            int userNum = 12345;

            // Mock the response for enrolled courses
            var enrolledCourses = new List<StudentCourseEnrollmentDTO>
            {
                new StudentCourseEnrollmentDTO { courseNum = 101, courseName = "Course 101", courseSuffix = "E" }
            };

            mockHttp
                .When(HttpMethod.Get, $"http://localhost/api/StudentCourseEnrollment/user/{userNum}")
                .Respond(JsonContent.Create(enrolledCourses));

            // Mock the response for course details
            mockHttp
                .When(HttpMethod.Get, "http://localhost/api/Course/101")
                .Respond(JsonContent.Create(new CourseDTO
                {
                    courseNum = 101,
                    courseAlias = "Math course",
                    courseName = "Course 101",
                    startDate = new DateTime(2024, 9, 1),
                    endDate = new DateTime(2025, 4, 1)
                }));


            // Mock the response for course times
            mockHttp
                .When(HttpMethod.Get, "http://localhost/api/CourseTime/course/101")
                .Respond(JsonContent.Create(new List<CourseTimeDTO>
                {
                    new CourseTimeDTO { weekday = 1, startTime = TimeSpan.Parse("10:00:00"), endTime = TimeSpan.Parse("12:00:00") }
                }));


            // Act
            var result = await scheduleService.GetSchedule(userNum);

            // Assert correct number of courses in each semester
            Assert.Single(result.Semester1);
            Assert.Single(result.Semester2);


            // CHECK VALIDITY OF SEMESTER 1
            var sem1Schedule = result.Semester1[0];
            Assert.Equal("Monday", sem1Schedule.GetType().GetProperty("Weekday")?.GetValue(sem1Schedule, null));


            // Access the "Classes" property (assumed to be a list or enumerable)
            var classes = sem1Schedule.GetType().GetProperty("Classes")?.GetValue(sem1Schedule, null) as IEnumerable<object>;
            Assert.NotNull(classes);

            // Retrieve the first class in the "Classes" list
            var firstClass = classes.FirstOrDefault();
            Assert.NotNull(firstClass);

            // Access the "CourseName" property of the first class
            Assert.Equal("Course 101E", firstClass.GetType().GetProperty("CourseName")?.GetValue(firstClass, null));
            Assert.Equal(TimeSpan.Parse("10:00:00"), firstClass.GetType().GetProperty("StartTime")?.GetValue(firstClass, null));
            Assert.Equal(TimeSpan.Parse("12:00:00"), firstClass.GetType().GetProperty("EndTime")?.GetValue(firstClass, null));
            Assert.Equal("Lecture", firstClass.GetType().GetProperty("Type")?.GetValue(firstClass, null));


            // CHECK VALIDITY OF SEMESTER 2
            var sem2Schedule = result.Semester2[0];
            Assert.Equal("Monday", sem2Schedule.GetType().GetProperty("Weekday")?.GetValue(sem2Schedule, null));


            // Access the "Classes" property (assumed to be a list or enumerable)
            classes = sem2Schedule.GetType().GetProperty("Classes")?.GetValue(sem2Schedule, null) as IEnumerable<object>;
            Assert.NotNull(classes);

            // Retrieve the first class in the "Classes" list
            firstClass = classes.FirstOrDefault();
            Assert.NotNull(firstClass);

            // Access the "CourseName" property of the first class
            Assert.Equal("Course 101E", firstClass.GetType().GetProperty("CourseName")?.GetValue(firstClass, null));
            Assert.Equal(TimeSpan.Parse("10:00:00"), firstClass.GetType().GetProperty("StartTime")?.GetValue(firstClass, null));
            Assert.Equal(TimeSpan.Parse("12:00:00"), firstClass.GetType().GetProperty("EndTime")?.GetValue(firstClass, null));
            Assert.Equal("Lecture", firstClass.GetType().GetProperty("Type")?.GetValue(firstClass, null));

        }

    }
}
