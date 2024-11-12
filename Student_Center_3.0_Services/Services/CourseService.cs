using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Student_Center_3._0_Services.DTOs;

namespace Student_Center_3._0_Services.Services
{

    public class CourseService
    {
        private readonly HttpClient _httpClient;

        public CourseService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
