using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Student_Center_3._0_Database.DTOs;
using Student_Center_3._0_Services.DTOs;

namespace Student_Center_3._0_Services.Services
{

    public class CourseEnrollmentService
    {
        private readonly HttpClient _httpClient;

        public CourseEnrollmentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> AddCourse(int userNum, int courseNum)
        {
            // IS COURSE FULL?
            var courseResponse = await _httpClient.GetAsync($"api/Course/{courseNum}");

            if (!courseResponse.IsSuccessStatusCode)
            {
                return "Course Not Found";
            }

            var courseRecord = await courseResponse.Content.ReadFromJsonAsync<CourseDTO>();

            if (courseRecord == null)
            {
                return "Course Not Found";
            }

            if (courseRecord.numEnrolled == courseRecord.totalSeats)
            {
                return "Course full";
            }

            // Fetch list of the student's currently enrolled courses, track total number of planned credits
            var creditResponse = await _httpClient.GetAsync($"api/StudentCourseEnrollment/user/{userNum}");
            double totalCredits = 0;
            List<StudentCourseEnrollmentDTO> enrolledCourses = new List<StudentCourseEnrollmentDTO>();


            if (creditResponse.IsSuccessStatusCode)
            {
                // Deserialize the response into a list of StudentCourseEnrollment
                enrolledCourses = await creditResponse.Content.ReadFromJsonAsync<List<StudentCourseEnrollmentDTO>>();

                if (enrolledCourses.Any())
                {
                    // Sum the course weights
                    totalCredits = enrolledCourses.Sum(crs => crs.courseWeight);

                    // HAS STUDENT ALREADY ENROLLED IN THIS COURSE?
                    bool courseExists = enrolledCourses.Any(course => course.courseName == courseRecord.courseName);
                    if (courseExists)
                    {
                        return "Duplicate Enrollment";
                    }

                }

            }

            // DOES ADDING COURSE EXCEED 5.0 CREDITS/YEAR LIMIT?
            totalCredits += courseRecord.courseWeight;

            // Check if the total credits exceed 5.0
            if (totalCredits > 5.0)
            {
                return "Exceeds credit limit";
            }
       

            // DOES COURSE'S CLASS TIMES CONFLICT WITH STUDENT'S CURRENTLY ENROLLED CLASSES?
            if (!await VerifyNoConflicts(courseRecord, enrolledCourses))
            {
                return "Time conflict";
            }

            // DOES STUDENT FULFILL PREREQUISITES?
            if (!await VerifyEnrollmentRequirements(userNum, courseRecord.courseName))
            {
                return "Lacking prerequisites or having antirequisites";
            }

            // INCREMENT NUMBER OF ENROLLED STUDENTS IN THE COURSE
            courseRecord.numEnrolled += 1;
            var content = new StringContent(courseRecord.numEnrolled.ToString(), Encoding.UTF8, "application/json");

            var updateCourseResponse = await _httpClient.PatchAsync($"api/Course/{courseNum}/update-enrollment", content);

            if (updateCourseResponse.IsSuccessStatusCode)
            {
                // ADD COURSE TO STUDENT'S RECORD
                var newEnrollment = new StudentCourseEnrollmentDTO
                {
                    userNum = userNum,
                    courseNum = courseNum,
                    courseName = courseRecord.courseName,
                    courseSuffix = courseRecord.courseSuffix,
                    startDate = courseRecord.startDate.ToString("yyyy-MM-dd"),
                    endDate = courseRecord.endDate.ToString("yyyy-MM-dd"),
                    courseWeight = courseRecord.courseWeight
                };

                var enrollmentResponse = await _httpClient.PostAsJsonAsync("api/StudentCourseEnrollment", newEnrollment);

                if (enrollmentResponse.IsSuccessStatusCode)
                {
                    return "OK";
                }
                else
                {
                    var errorContent = await enrollmentResponse.Content.ReadAsStringAsync();
                    return $"{errorContent}";
                }
            }

            else
            {
                var errorContent = await updateCourseResponse.Content.ReadAsStringAsync();
                return $"{errorContent}";
            }

        }

        public async Task<bool> VerifyNoConflicts(CourseDTO courseRecord, List<StudentCourseEnrollmentDTO> studentCourseEnrollment)
        {
            // GET LIST OF CLASS TIMES FOR THE STUDENT'S ENROLLED COURSES
            var classList = new List<int> { courseRecord.courseNum };

            // Find list of courses that occur in the same semester(s) as the requested course
            foreach (var enrollment in studentCourseEnrollment)
            {
                var crsStart = DateTime.Parse(enrollment.startDate);
                var crsEnd = DateTime.Parse(enrollment.endDate);

                if ((crsStart >= courseRecord.startDate & crsStart <= courseRecord.endDate) |
                    (crsEnd >= courseRecord.startDate & crsEnd <= courseRecord.endDate))
                {
                    classList.Add(enrollment.courseNum);
                }
            }

            var enrolledCourseTimes = new List<CourseTimeDTO>();

            // Fetch class times from table (the first entry in classList is the requested course; if classList.Count == 1, the student is not currently enrolled in any other course) 
            if (classList.Count > 1)
            {
                string courseNumList = string.Join(",", classList);
                string query = $"SELECT * FROM CourseTimes WHERE courseNum IN ({courseNumList})";

                var response = await _httpClient.PostAsJsonAsync("api/Course/executeQuery", query);

                // No enrolled courses to fetch times for
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error fetching course times: {errorContent}");
                }

                // Deserialize JSON response into a list of dictionaries for flexibility
                enrolledCourseTimes = await response.Content.ReadFromJsonAsync<List<CourseTimeDTO>>();
            }
            // No enrolled courses that could conflict
            else
            {
                return true;
            }

            return await CheckTimeOverlaps(courseRecord.courseNum, enrolledCourseTimes);
        }

        // HELPER: check student's current timetable and see if any time overlaps will occur if the requested course is added to the schedule.
        private async Task<bool> CheckTimeOverlaps(int courseNum, List<CourseTimeDTO> enrolledCourseTimes) 
        { 

            // Process the data into the dictionary format
            var courseSchedule = new Dictionary<int, List<List<TimeSpan>>>();
            var courseToVerify = new Dictionary<int, List<List<TimeSpan>>>();

            foreach (var courseTime in enrolledCourseTimes)
            {
                if (courseTime.courseNum == courseNum)
                {
                    if (!courseToVerify.ContainsKey(courseTime.weekday))
                    {
                        // If the weekday is not in the dictionary, add it with a new list
                        courseToVerify[courseTime.weekday] = new List<List<TimeSpan>>();
                    }

                    // Add the course times as a nested list [startTime, endTime]
                    courseToVerify[courseTime.weekday].Add(new List<TimeSpan> { TimeSpan.Parse(courseTime.startTime), TimeSpan.Parse(courseTime.endTime) });

                    continue;

                }

                if (!courseSchedule.ContainsKey(courseTime.weekday))
                {
                    // If the weekday is not in the dictionary, add it with a new list
                    courseSchedule[courseTime.weekday] = new List<List<TimeSpan>>();
                }

                // Add the course times as a nested list [startTime, endTime]
                courseSchedule[courseTime.weekday].Add(new List<TimeSpan> { TimeSpan.Parse(courseTime.startTime), TimeSpan.Parse(courseTime.endTime)});
            }

            // Sort the class times for each weekday by startTime
            foreach (var day in courseSchedule)
            {
                day.Value.Sort((time1, time2) =>
                    time1[0].CompareTo(time2[0]));
            }

            // CHECK FOR ANY TIME-INTERVAL OVERLAPS
            foreach (var day in courseToVerify)
            {

                // current schedule doesn't have any classes on the requested day
                if (!courseSchedule.ContainsKey(day.Key))
                {
                    continue;
                }

                var classTimesForDay = courseSchedule[day.Key];

                foreach (var reqTime in day.Value)
                {
                    foreach (var classTime in classTimesForDay)
                    {
                        // If the class time ends before the requested time starts, move onto next class time
                        if (reqTime[0] >= classTime[1])
                        {
                            continue;
                        }

                        // If the class time starts after the requested time ends, break early
                        if (reqTime[1] <= classTime[0])
                        {
                            break; 
                        }

                        // if requested time range overlaps with a class time, return False
                        if (reqTime[0] <= classTime[0] || reqTime[1] >= classTime[1])
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public async Task<bool> VerifyEnrollmentRequirements(int userNum, string requestedCourse)
        {
            // FETCH COURSE PREREQUISITES
            // Fetch prerequisite expression for the requested course
            var prereqResponse = await _httpClient.GetAsync($"api/CoursePrerequisite/{requestedCourse}");

            // if unsuccessful, then no prereqs for the course exist, student may take it
            if (!prereqResponse.IsSuccessStatusCode)
            {
                return true;
            }

            var prereq = await prereqResponse.Content.ReadFromJsonAsync<CoursePrerequisiteDTO>();

            // Fetch the student's course history and join with Courses to get course weights
            var historyResponse = await _httpClient.GetAsync($"api/StudentCourseHistory/user/{userNum}");

            // FETCH STUDENT COURSE HISTORY
            // student has no course history but prerequisites for the requested course exist, return false
            if (!historyResponse.IsSuccessStatusCode)
            {
                return false;
            }

            // Deserialize course history as Dictionary<string, double>
            var courseHistory = await historyResponse.Content.ReadFromJsonAsync<Dictionary<string, double>>();

            // FETCH COURSE ANTIREQUISITES
            var antireqResponse = await _httpClient.GetAsync($"api/CourseAntirequisite/course/{requestedCourse}");

            // no antirequisites, simply check only prereqs
            if (!antireqResponse.IsSuccessStatusCode)
            {
                return PrereqFulfilled(prereq.prerequisiteExpression, courseHistory);
            }

            var antireq = await antireqResponse.Content.ReadFromJsonAsync<List<string>>();

            // if student has already taken this course, they can't take it again
            antireq.Add(requestedCourse);

            // CHECK PREREQUISITE FULFILLMENT AND ENSURE STUDENT HAS NOT ANTIREQUISITES
            return PrereqFulfilled(prereq.prerequisiteExpression, courseHistory) && AntireqFulfilled(antireq, courseHistory);
        }


        // HELPER: see if student has taken any antirequisites that make them ineligible for the requested course
        private bool AntireqFulfilled(List<string> antirequisite,  Dictionary<string, double> courseHistory)
        {
            foreach (var anti in antirequisite)
            {
                if (courseHistory.ContainsKey(anti)){
                    return false;
                }
            }
            return true;
        } 


        /* HELPER: uses a modified "postfix expression" evaluator algo to check if a student has fulfilled required prerequisites
         * ex. Course A prereqs: (AND: Course B, Course C, Course D) OR (CREDITS 1.0 FROM: Course E, Course F, Course G)
         *     becomes this postfix string: Course B, Course C, Course D, AND, Course E, Course F, Course G, CREDITS 1.0, OR
         *     
         *     string prereqExp: the postfix string to evaluate
         *     Dictionary<string, double> courseHistory: a student's course history, in form of {'course' : course weighting}
         */
        private bool PrereqFulfilled(string prereqExp, Dictionary<string, double> courseHistory)
        {
            var prereq = prereqExp.Split(',');
            var courseStack = new Stack<string>(); // stack of courses yet to be checked
            var groupStack = new Stack<bool>(); // stack of course groupings --> each group is a list of courses that previously had a logical operator applied to it

            // iterate through each element in prereq expression
            foreach (var pre in prereq)
            {
                if (pre == "OR")
                {
                    bool fulfilled = false;

                    // courseStack is empty --> OR operation occurs between two previously-processed groups
                    if (courseStack.Count == 0)
                    {
                        var grp1 = groupStack.Pop();
                        var grp2 = groupStack.Pop();
                        fulfilled = grp1 || grp2;
                    }

                    // only one entry in courseStack --> OR operation occurs between a group and a course
                    else if (courseStack.Count == 1)
                    {
                        bool crs = courseHistory.ContainsKey(courseStack.Pop());
                        bool grp = groupStack.Pop();
                        fulfilled = crs || grp;
                    }

                    // OR operation occrus between >= 2 courses
                    else
                    {
                        while (courseStack.Count > 0)
                        {
                            if (courseHistory.ContainsKey(courseStack.Pop()))
                            {
                                fulfilled = true;

                                // terminate while early + clear courseStack for subsequent courses not part of current grouping
                                courseStack.Clear();
                            }
                        }
                    }
                    groupStack.Push(fulfilled);
                }

                else if (pre == "AND")
                {
                    bool fulfilled = true;

                    // courseStack is empty --> AND operation occurs between two previously-processed groups
                    if (courseStack.Count == 0)
                    {
                        var grp1 = groupStack.Pop();
                        var grp2 = groupStack.Pop();
                        fulfilled = grp1 && grp2;
                    }

                    // only one entry in courseStack --> AND operation occurs between a group and a course
                    else if (courseStack.Count == 1)
                    {
                        bool crs = courseHistory.ContainsKey(courseStack.Pop());
                        bool grp = groupStack.Pop();
                        fulfilled = crs && grp;
                    }

                    // AND operation occrus between >= 2 courses
                    else
                    {
                        while (courseStack.Count > 0)
                        {
                            if (!courseHistory.ContainsKey(courseStack.Pop()))
                            {
                                fulfilled = false;
                                courseStack.Clear();
                            }
                        }
                    }
                    groupStack.Push(fulfilled);
                }

                // only need to count number of credits, will always occur between list of >= 2 courses
                else if (pre.StartsWith("CREDIT"))
                {
                    bool fulfilled = false;
                    double credit = double.Parse(pre.Split(' ')[1]);
                    double cnt = 0;

                    while (courseStack.Count > 0)
                    {
                        var crs = courseStack.Pop();
                        if (courseHistory.TryGetValue(crs, out double weight))
                        {
                            cnt += weight;
                            if (cnt >= credit)
                            {
                                fulfilled = true;
                                courseStack.Clear();
                            }
                        }
                    }
                    groupStack.Push(fulfilled);
                }
                else
                {
                    courseStack.Push(pre);
                }
            }

            return groupStack.Pop();
        }
    }

}
