using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;

public class CourseService
{
    private readonly HttpClient _httpClient;

    public CourseService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<List<Dictionary<string, object>>> GetCoursesLabsBySearch(string searchQuery)
    {
        // Prepare the SQL query with string interpolation to directly insert the searchQuery into the SQL
        string query = $@"SELECT c.courseNum, c.courseName, c.courseSuffix, c.courseAlias, c.courseDesc, c.extraInformation, c.prerequisites, c.antirequisites, c.courseWeight, c.startDate, c.endDate, c.instructor, c.room, c.numEnrolled, c.totalSeats, (SELECT ct.weekday, ct.startTime, ct.endTime FROM CourseTimes ct WHERE ct.courseNum = c.courseNum FOR JSON PATH) AS courseTimes FROM Courses c WHERE c.courseName LIKE CONCAT('%', '{searchQuery}', '%')";


        // Send the query to the CourseController API to execute
        var response = await _httpClient.PostAsJsonAsync("api/Course/executeQuery", query);
        var responseContent = await response.Content.ReadAsStringAsync();


        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Failed to retrieve courses: {responseContent}");
        }

        // Deserialize JSON response into a list of dictionaries for flexibility
        var courses = await response.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>();

        if (courses == null)
        {
            return new List<Dictionary<string, object>>();
        }

        // Separate courses and labs/tutorials
        var mainCourses = new List<Dictionary<string, object>>();
        var labs = new List<Dictionary<string, object>>();

        foreach (var course in courses)
        {
            // Check if "courseWeight" exists and extract the value safely
            if (course.ContainsKey("courseWeight") && course["courseWeight"] is JsonElement courseWeightElement)
            {
                // Convert JsonElement to double
                double courseWeight = courseWeightElement.GetDouble();

                // If courseWeight is 0, treat it as a lab/tutorial component
                if (courseWeight == 0)
                {
                    labs.Add(course);
                }
                else
                {
                    mainCourses.Add(course);
                }
            }

            // Format the courseTimes field
            if (course.ContainsKey("courseTimes"))
            {
                string courseTimesJson = course["courseTimes"].ToString();
                var courseTimesList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(courseTimesJson);

                if (courseTimesList != null)
                {
                    var formattedCourseTimes = new Dictionary<int, List<string>>();

                    foreach (var courseTime in courseTimesList)
                    {
                        // Ensure each courseTime entry has the necessary keys (weekday, startTime, endTime)
                        if (courseTime.ContainsKey("weekday") && courseTime["weekday"] is JsonElement weekdayElement &&
                            courseTime.ContainsKey("startTime") && courseTime["startTime"] is JsonElement startTimeElement &&
                            courseTime.ContainsKey("endTime") && courseTime["endTime"] is JsonElement endTimeElement)
                        {
                            // Access the weekday as an integer
                            int weekday = weekdayElement.GetInt32();

                            // Access startTime and endTime as strings
                            string startTime = startTimeElement.GetString();
                            string endTime = endTimeElement.GetString();

                            // Add the times to the formatted dictionary
                            if (!formattedCourseTimes.ContainsKey(weekday))
                            {
                                formattedCourseTimes[weekday] = new List<string>();
                            }

                            formattedCourseTimes[weekday].Add(startTime);
                            formattedCourseTimes[weekday].Add(endTime);
                        }
                    }


                    // Replace courseTimes with the formatted dictionary
                    course["courseTimes"] = formattedCourseTimes;
                }
            }

        }

        foreach (var course in mainCourses)
        {
            Console.WriteLine("course");

            // Identify matching lab components for each course by courseName and courseSuffix
            foreach (var lab in labs)
            {
                var matchingLabs = labs
                    .Where(lab => lab["courseName"].ToString().Trim().Equals(course["courseName"].ToString().Trim()) &&
                                  lab["courseSuffix"].ToString().Trim().Equals(course["courseSuffix"].ToString().Trim()))
                    .ToList();

                if (matchingLabs.Any())
                {
                    // Add labs as a "labComponents" key in the parent course JSON
                    course["labComponents"] = matchingLabs;
                }
            }

        }

        return mainCourses;
    }
}