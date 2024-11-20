using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        // Call the SearchCourses API
        var response = await _httpClient.GetAsync($"api/Course/search?query={searchQuery}");
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Failed to retrieve courses");
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
        }

        // Add lab components to their parent courses
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
