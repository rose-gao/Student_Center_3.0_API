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
using Moq.Protected;
using Moq;

namespace Student_Center_3._0_xUnitTests
{
    public class DropCourseServiceTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DropCourseServiceTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        // DropCourse should function correctly and return "OK" if all dependent HTTP requests succeed. Also indirectly tests helpers ProcessCourseDrops, DropSingleCourse, and UpdateCourseEnrollment
        [Fact]
        public async Task DropCourse_DropsAllAssociatedCourses_WhenValid()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/"); // Set BaseAddress

            var service = new DropCourseService(httpClient);
            var userNum = 12345;
            var courseNum = 101;

            // Simulate the user being enrolled
            mockHttp.When(HttpMethod.Get, $"http://localhost/api/StudentCourseEnrollment/{userNum}/{courseNum}")
                    .Respond(HttpStatusCode.OK, JsonContent.Create(new StudentCourseEnrollmentDTO
                    {
                        courseNum = courseNum,
                        courseName = "Course 101",
                        courseSuffix = "A"
                    }));

            // Simulate fetching all enrollments
            mockHttp.When($"http://localhost/api/StudentCourseEnrollment/user/{userNum}")
                    .Respond(HttpStatusCode.OK, JsonContent.Create(new List<StudentCourseEnrollmentDTO>
                    {
                    new StudentCourseEnrollmentDTO { courseNum = courseNum, courseName = "Course 101", courseSuffix = "A", courseAlias = "Math Course" },
                    new StudentCourseEnrollmentDTO { courseNum = 102, courseName = "Course 101", courseSuffix = "A", courseAlias = "LAB"}
                    }));

            // Simulate successful deletion of courses
            mockHttp.When(HttpMethod.Delete, $"http://localhost/api/StudentCourseEnrollment/{userNum}/{courseNum}")
                    .Respond(HttpStatusCode.OK);
            mockHttp.When($"http://localhost/api/StudentCourseEnrollment/{userNum}/102")
                    .Respond(HttpStatusCode.OK);

            // Simulate fetching course details
            mockHttp.When($"http://localhost/api/Course/{courseNum}")
                    .Respond(HttpStatusCode.OK, JsonContent.Create(new CourseDTO { courseNum = courseNum, numEnrolled = 20 }));
            mockHttp.When($"http://localhost/api/Course/102")
                    .Respond(HttpStatusCode.OK, JsonContent.Create(new CourseDTO { courseNum = 102, numEnrolled = 15 }));

            // Simulate updating enrollment
            mockHttp.When($"http://localhost/api/Course/{courseNum}/update-enrollment")
                    .Respond(HttpStatusCode.OK);
            mockHttp.When($"http://localhost/api/Course/102/update-enrollment")
                    .Respond(HttpStatusCode.OK);

            // Act
            var result = await service.DropCourse(userNum, courseNum);

            // Assert
            Assert.Equal("OK", result);
        }


        // Trying to drop a course a student is not enrolled in
        [Fact]
        public async Task DropCourse_ReturnsError_WhenStudentNotEnrolled()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/"); // Set BaseAddress

            var service = new DropCourseService(httpClient);
            var userNum = 12345;
            var courseNum = 101;

            // Simulate the user not being enrolled
            mockHttp.When(HttpMethod.Get, $"http://localhost/api/StudentCourseEnrollment/{userNum}/{courseNum}")
            .Respond(HttpStatusCode.NotFound);


            // Act
            var result = await service.DropCourse(userNum, courseNum);

            // Assert
            Assert.Equal("Student is not enrolled in course", result);
        }


        // Unable to fetch any of the courses a student is enrolled in
        [Fact]
        public async Task DropCourse_ReturnsError_WhenFetchingEnrollmentsFails()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/"); // Set BaseAddress

            var service = new DropCourseService(httpClient); 
            var userNum = 12345;
            var courseNum = 101;

            // Simulate the user being enrolled
            mockHttp.When(HttpMethod.Get, $"http://localhost/api/StudentCourseEnrollment/{userNum}/{courseNum}")
                    .Respond(HttpStatusCode.OK, JsonContent.Create(new StudentCourseEnrollmentDTO
                    {
                        courseNum = courseNum,
                        courseName = "Course 101",
                        courseSuffix = "A"
                    }));

            // Simulate a failure when fetching all enrollments associated with the course (lecture, labs, tuts)
            mockHttp.When($"http://localhost/api/StudentCourseEnrollment/user/{userNum}")
                    .Respond(HttpStatusCode.InternalServerError);

            // Act
            var result = await service.DropCourse(userNum, courseNum);

            // Assert
            Assert.Equal($"Failed to fetch enrollments for user {userNum}: InternalServerError", result);
        }


        // Simulate DropSingleCourse error: trying to drop a course with an invalid course number
        [Fact]
        public async Task DropCourse_DropSingleCourseError_InvalidInput()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/"); // Set BaseAddress

            var service = new DropCourseService(httpClient);
            var userNum = 12345;
            var courseNum = 101;

            // Simulate the user being enrolled
            mockHttp.When(HttpMethod.Get, $"http://localhost/api/StudentCourseEnrollment/{userNum}/{courseNum}")
                    .Respond(HttpStatusCode.OK, JsonContent.Create(new StudentCourseEnrollmentDTO
                    {
                        courseNum = courseNum,
                        courseName = "Course 101",
                        courseSuffix = "A"
                    }));

            // Simulate fetching all enrollments-- the second course has an invalid courseNum
            mockHttp.When($"http://localhost/api/StudentCourseEnrollment/user/{userNum}")
                    .Respond(HttpStatusCode.OK, JsonContent.Create(new List<StudentCourseEnrollmentDTO>
                    {
                new StudentCourseEnrollmentDTO { courseNum = courseNum, courseName = "Course 101", courseSuffix = "A", courseAlias = "Math Course" },
                new StudentCourseEnrollmentDTO { courseNum = -2, courseName = "Course 101", courseSuffix = "A", courseAlias = "LAB" }
                    }));

            // Mock deletion of course for userNum and courseNum
            mockHttp.When(HttpMethod.Delete, $"http://localhost/api/StudentCourseEnrollment/{userNum}/{courseNum}")
                    .Respond(HttpStatusCode.OK);

            // Simulate fetching course details for courseNum
            mockHttp.When($"http://localhost/api/Course/{courseNum}")
                    .Respond(HttpStatusCode.OK, JsonContent.Create(new CourseDTO { courseNum = courseNum, numEnrolled = 20 }));

            // Simulate updating course enrollment
            mockHttp.When($"http://localhost/api/Course/{courseNum}/update-enrollment")
                    .Respond(HttpStatusCode.OK);

            // Act
            var result = await service.DropCourse(userNum, courseNum);

            // Assert
            Assert.Equal("Failed to drop course -2: Invalid input.", result);
        }

        // Simulate DropSingleCourse error: unable to drop a course
        [Fact]
        public async Task DropCourse_DropSingleCourseError_UnableToDeleteCourse()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/"); // Set BaseAddress

            var service = new DropCourseService(httpClient);
            var userNum = 12345;
            var courseNum = 101;

            // Simulate the user being enrolled
            mockHttp.When(HttpMethod.Get, $"http://localhost/api/StudentCourseEnrollment/{userNum}/{courseNum}")
                    .Respond(HttpStatusCode.OK, JsonContent.Create(new StudentCourseEnrollmentDTO
                    {
                        courseNum = courseNum,
                        courseName = "Course 101",
                        courseSuffix = "A"
                    }));

            // Simulate fetching all enrollments
            mockHttp.When($"http://localhost/api/StudentCourseEnrollment/user/{userNum}")
                    .Respond(HttpStatusCode.OK, JsonContent.Create(new List<StudentCourseEnrollmentDTO>
                    {
                new StudentCourseEnrollmentDTO { courseNum = courseNum, courseName = "Course 101", courseSuffix = "A", courseAlias = "Math Course" }
                    }));

            // Mock deletion of course for userNum and courseNum
            mockHttp.When(HttpMethod.Delete, $"http://localhost/api/StudentCourseEnrollment/{userNum}/{courseNum}")
                    .Respond(HttpStatusCode.InternalServerError);

            // Act
            var result = await service.DropCourse(userNum, courseNum);

            // Assert
            Assert.Equal("Failed to drop course 101: Failed to delete enrollment- InternalServerError", result);
        }

        // Simulate DropSingleCourse error: unable to fetch course information for updating enrollment numbers
        [Fact]
        public async Task DropCourse_DropSingleCourseError_UnableToFetchCourse()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/"); // Set BaseAddress

            var service = new DropCourseService(httpClient);
            var userNum = 12345;
            var courseNum = 101;

            // Simulate the user being enrolled
            mockHttp.When(HttpMethod.Get, $"http://localhost/api/StudentCourseEnrollment/{userNum}/{courseNum}")
                    .Respond(HttpStatusCode.OK, JsonContent.Create(new StudentCourseEnrollmentDTO
                    {
                        courseNum = courseNum,
                        courseName = "Course 101",
                        courseSuffix = "A"
                    }));

            // Simulate fetching all enrollments
            mockHttp.When($"http://localhost/api/StudentCourseEnrollment/user/{userNum}")
                    .Respond(HttpStatusCode.OK, JsonContent.Create(new List<StudentCourseEnrollmentDTO>
                    {
                new StudentCourseEnrollmentDTO { courseNum = courseNum, courseName = "Course 101", courseSuffix = "A", courseAlias = "Math Course" },
                    }));

            // Mock deletion of course for userNum and courseNum
            mockHttp.When(HttpMethod.Delete, $"http://localhost/api/StudentCourseEnrollment/{userNum}/{courseNum}")
                    .Respond(HttpStatusCode.OK);


            // Simulate fetching course details for courseNum
            mockHttp.When($"http://localhost/api/Course/{courseNum}")
                    .Respond(HttpStatusCode.InternalServerError);


            // Act
            var result = await service.DropCourse(userNum, courseNum);

            // Assert
            Assert.Equal("Failed to drop course 101: Failed to fetch course 101- InternalServerError", result);
        }


        // Simulate UpdateCourseEnrollment error: trying to reduce the number of enrolled students below 0
        [Fact]
        public async Task DropCourse_UpdateCourseEnrollmentError_EnrollmentNumberBelowZero()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/"); // Set BaseAddress

            var service = new DropCourseService(httpClient);
            var userNum = 12345;
            var courseNum = 101;

            // Simulate the user being enrolled
            mockHttp.When(HttpMethod.Get, $"http://localhost/api/StudentCourseEnrollment/{userNum}/{courseNum}")
                    .Respond(HttpStatusCode.OK, JsonContent.Create(new StudentCourseEnrollmentDTO
                    {
                        courseNum = courseNum,
                        courseName = "Course 101",
                        courseSuffix = "A"
                    }));

            // Simulate fetching all enrollments
            mockHttp.When($"http://localhost/api/StudentCourseEnrollment/user/{userNum}")
                    .Respond(HttpStatusCode.OK, JsonContent.Create(new List<StudentCourseEnrollmentDTO>
                    {
                new StudentCourseEnrollmentDTO { courseNum = courseNum, courseName = "Course 101", courseSuffix = "A", courseAlias = "Math Course" },
                    }));

            // Mock deletion of course for userNum and courseNum
            mockHttp.When(HttpMethod.Delete, $"http://localhost/api/StudentCourseEnrollment/{userNum}/{courseNum}")
                    .Respond(HttpStatusCode.OK);

            // Simulate fetching course details for courseNum
            mockHttp.When($"http://localhost/api/Course/{courseNum}")
                    .Respond(HttpStatusCode.OK, JsonContent.Create(new CourseDTO { courseNum = courseNum, numEnrolled = 0 }));

            // Act
            var result = await service.DropCourse(userNum, courseNum);

            // Assert
            Assert.Equal("Failed to drop course 101: Cannot reduce enrollment below zero.", result);
        }

        // Simulate DropSingleCourse error: unable to update the number of enrolled students after a course is dropped.
        [Fact]
        public async Task DropCourse_UpdateCourseEnrollmentError_UnableToUpdateEnrollmentNumbers()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/"); // Set BaseAddress

            var service = new DropCourseService(httpClient);
            var userNum = 12345;
            var courseNum = 101;

            // Simulate the user being enrolled
            mockHttp.When(HttpMethod.Get, $"http://localhost/api/StudentCourseEnrollment/{userNum}/{courseNum}")
                    .Respond(HttpStatusCode.OK, JsonContent.Create(new StudentCourseEnrollmentDTO
                    {
                        courseNum = courseNum,
                        courseName = "Course 101",
                        courseSuffix = "A"
                    }));

            // Simulate fetching all enrollments
            mockHttp.When($"http://localhost/api/StudentCourseEnrollment/user/{userNum}")
                    .Respond(HttpStatusCode.OK, JsonContent.Create(new List<StudentCourseEnrollmentDTO>
                    {
                new StudentCourseEnrollmentDTO { courseNum = courseNum, courseName = "Course 101", courseSuffix = "A", courseAlias = "Math Course" },
                    }));

            // Mock deletion of course for userNum and courseNum
            mockHttp.When(HttpMethod.Delete, $"http://localhost/api/StudentCourseEnrollment/{userNum}/{courseNum}")
                    .Respond(HttpStatusCode.OK);

            // Simulate fetching course details for courseNum
            mockHttp.When($"http://localhost/api/Course/{courseNum}")
                    .Respond(HttpStatusCode.OK, JsonContent.Create(new CourseDTO { courseNum = courseNum, numEnrolled = 20 }));

            // Simulate updating course enrollment
            mockHttp.When($"http://localhost/api/Course/{courseNum}/update-enrollment")
                    .Respond(HttpStatusCode.InternalServerError);

            // Act
            var result = await service.DropCourse(userNum, courseNum);

            // Assert
            Assert.Equal("Failed to drop course 101: Failed to update course 101- InternalServerError", result);
        }

    }

}
