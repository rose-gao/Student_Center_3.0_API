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
        private readonly DropCourseService _dropCourseService;

        public CourseEnrollmentService(AddCourseService addCourseService, DropCourseService dropCourseService)
        {
            _addCourseService = addCourseService; 
            _dropCourseService = dropCourseService;
        }

        public async Task<string> AddCourse(int userNum, int courseNum)
        {
            return await _addCourseService.AddCourse(userNum, courseNum);
        }

        public async Task<string> DropCourse(int userNum, int courseNum)
        {
            return await _dropCourseService.DropCourse(userNum, courseNum);
        }

    }

}
