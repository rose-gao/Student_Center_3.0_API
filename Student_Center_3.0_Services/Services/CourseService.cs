using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Student_Center_3._0_Services.Services
{

    public class CourseService
    {
        private readonly HttpClient _httpClient;

        public CourseService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CheckPrerequisite(int userNum, string requestedCourse)
        {
            // Fetch prerequisite expression for the requested course
            var prereqExpResponse = await _httpClient.GetStringAsync($"api/CoursePrerequisite/{requestedCourse}");
            string prereqExp = JsonSerializer.Deserialize<string>(prereqExpResponse);

            // Fetch the student's course history and join with Courses to get course weights
            var courseHistoryUrl = $"api/StudentCourseHistory/{userNum}";
            var courseHistoryResponse = await _httpClient.GetStringAsync(courseHistoryUrl);
            var courseHistoryList = JsonSerializer.Deserialize<List<CourseHistoryRecord>>(courseHistoryResponse);

            // Convert to a dictionary of 'courseName' : courseWeight
            var courseHistory = courseHistoryList
                .GroupBy(c => c.courseName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(c => c.courseWeight).Distinct().Sum()
                );

            // Call the helper method to check if prerequisites are fulfilled
            return PrereqFulfilled(prereqExp, courseHistory);
        }

        // Helper method to evaluate prerequisite fulfillment
        private bool PrereqFulfilled(string prereqExp, Dictionary<string, double> courseHistory)
        {
            var prereq = prereqExp.Split(',');
            var courseStack = new Stack<string>();
            var groupStack = new Stack<bool>();

            foreach (var pre in prereq)
            {
                if (pre == "OR")
                {
                    bool fulfilled = false;

                    // Logical operator 'OR' between groups
                    if (courseStack.Count == 0)
                    {
                        var grp1 = groupStack.Pop();
                        var grp2 = groupStack.Pop();
                        fulfilled = grp1 || grp2;
                    }
                    else if (courseStack.Count == 1)
                    {
                        bool crs = courseHistory.ContainsKey(courseStack.Pop());
                        bool grp = groupStack.Pop();
                        fulfilled = crs || grp;
                    }
                    else
                    {
                        while (courseStack.Count > 0)
                        {
                            if (courseHistory.ContainsKey(courseStack.Pop()))
                            {
                                fulfilled = true;
                                courseStack.Clear();
                            }
                        }
                    }
                    groupStack.Push(fulfilled);
                }
                else if (pre == "AND")
                {
                    bool fulfilled = true;

                    // Logical operator 'AND' between groups
                    if (courseStack.Count == 0)
                    {
                        var grp1 = groupStack.Pop();
                        var grp2 = groupStack.Pop();
                        fulfilled = grp1 && grp2;
                    }
                    else if (courseStack.Count == 1)
                    {
                        bool crs = courseHistory.ContainsKey(courseStack.Pop());
                        bool grp = groupStack.Pop();
                        fulfilled = crs && grp;
                    }
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
