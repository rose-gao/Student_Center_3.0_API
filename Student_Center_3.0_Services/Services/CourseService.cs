using Student_Center_3._0_Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
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

    public async Task<string> AddCourse(CourseCreateDTO courseCreateDTO)
    {
        if (courseCreateDTO == null)
        {
            return "Course infromation cannot be empty.";
        }

        if (!string.IsNullOrEmpty(courseCreateDTO.prerequisites))
        {
            try
            {
                bool result = await ValidatePrereqs(courseCreateDTO.prerequisites);
                if (!result)
                {
                    return "Preqrequisites must be in the following format: 'Calculus 1000,Calculus 1301,Calculus 1500,OR,Computer Science 1026,Computer Science 1027,Computer Science 1033,CREDIT 1.0,AND'.";
                }
            }
            catch (Exception ex)
            {
                return "Preqrequisites must be in the following format: 'Calculus 1000,Calculus 1301,Calculus 1500,OR,Computer Science 1026,Computer Science 1027,Computer Science 1033,CREDIT 1.0,AND'.";
            }
        }

        if (!string.IsNullOrEmpty(courseCreateDTO.antirequisites) && !await ValidateAntireqs(courseCreateDTO.antirequisites))
        {
            return "Antirequisites must be in a list: 'Calculus 1000,Computer Science 1026,Math 1600'";
        }

        // ADD COURSE TO TABLE
        var courseResponse = await _httpClient.PostAsJsonAsync($"api/Course", courseCreateDTO);
        if (!courseResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Error adding course: {courseResponse.StatusCode}");
        }

        var errors = new List<string>();

        // ADD PREREQUISITES TO PREREQUISITE TABLE
        var coursePrerequisiteDTO = new CoursePrerequisiteDTO
        {
            course = courseCreateDTO.courseName,
            prerequisiteExpression = courseCreateDTO.prerequisites
        };

        if (!string.IsNullOrEmpty(courseCreateDTO.prerequisites))
        {
            var prereqResponse = await _httpClient.PostAsJsonAsync($"api/CoursePrerequisite", coursePrerequisiteDTO);

            if (!prereqResponse.IsSuccessStatusCode && prereqResponse.StatusCode != System.Net.HttpStatusCode.Conflict)
            {
                errors.Add($"Failed to add prerequisite: {prereqResponse.StatusCode}");
            }

        }
        


        if (!string.IsNullOrEmpty(courseCreateDTO.antirequisites))
        {
            // ADD ANTIREQUISITES TO ANTIREQUISITE TABLE
            string[] antireqs = courseCreateDTO.antirequisites.Split(',').Select(a => a.Trim()).ToArray();
            foreach (var antireq in antireqs)
            {
                var courseAntirequisiteDTO = new CourseAntirequisiteDTO
                {
                    course = courseCreateDTO.courseName,
                    antirequisite = antireq
                };

                var antireqResponse = await _httpClient.PostAsJsonAsync($"api/CourseAntirequisite", courseAntirequisiteDTO);

                if (!antireqResponse.IsSuccessStatusCode && antireqResponse.StatusCode != System.Net.HttpStatusCode.Conflict)
                {
                    errors.Add($"Failed to add antirequisite: {antireq}, {antireqResponse.StatusCode}");
                }

            }

        }

        return errors.Any() ? string.Join("; ", errors) : "OK";

    }

    // HELPER: check validity of prerequisite string by simulating the prerequisite checking algo from AddCourseService
    private async Task<bool> ValidatePrereqs(string coursePrerequisites)
    {
        var prereq = coursePrerequisites.Split(',').Select(a => a.Trim()).ToArray(); ;
        var courseStack = new Stack<string>(); // stack of courses yet to be checked
        var groupStack = new Stack<bool>(); // stack of course groupings --> each group is a list of courses that previously had a logical operator applied to it

        // iterate through each element in prereq expression
        foreach (var pre in prereq)
        {
            if (pre == "OR" || pre == "AND")
            {
                // courseStack is empty --> OR operation occurs between two previously-processed groups
                if (courseStack.Count == 0)
                {
                    groupStack.Pop();
                    groupStack.Pop();
                }

                // only one entry in courseStack --> OR operation occurs between a group and a course
                else if (courseStack.Count == 1)
                {
                    courseStack.Pop();
                    groupStack.Pop();
                }

                // OR operation occrus between >= 2 courses
                else
                {
                    // clear courseStack for subsequent courses not part of current grouping
                    courseStack.Clear();
                }
                groupStack.Push(true);
            }

            // check that the length of the list of courses is >= 2
            else if (pre.StartsWith("CREDIT"))
            {
                if (courseStack.Count < 2)
                {
                    return false;
                }
                courseStack.Clear();

                groupStack.Push(true);
            }

            // Check the Course name is valid
            else
            {
                if (!Regex.IsMatch(pre, @"^[A-Za-z ]+ \d+$"))
                {
                    return false;
                }

                courseStack.Push(pre);
            }
        }

        if (groupStack.Count > 1 || courseStack.Count > 0)
        {
            return false;
        }
        return true;
    }
    

    private async Task<bool> ValidateAntireqs(string courseAntirequisites)
    {
        if (courseAntirequisites.EndsWith(","))
        {
            return false;
        }

        // Split the courses by comma
        string[] courseArray = courseAntirequisites.Split(',').Select(a => a.Trim()).ToArray();

        // Define regex for course format: letters followed by a space and digits
        string pattern = @"^[A-Za-z ]+ \d+$";

        // Check each course using regex
        return courseArray.All(course => Regex.IsMatch(course, pattern));

    }

}
