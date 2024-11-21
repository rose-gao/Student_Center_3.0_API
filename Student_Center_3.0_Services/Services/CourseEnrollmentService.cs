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
        private readonly AddCourseService _addCourseService;

        public CourseEnrollmentService(AddCourseService addCourseService)
        {
            _addCourseService = addCourseService; // Ensure proper injection
        }

        public async Task<string> AddCourse(int userNum, int courseNum)
        {
            return await _addCourseService.AddCourse(userNum, courseNum);
        }
    

    }

}
