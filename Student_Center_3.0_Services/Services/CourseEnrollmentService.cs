using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Student_Center_3._0_Services.DTOs;

namespace Student_Center_3._0_Services.Services
{

    public class CourseEnrollmentService
    {
        private readonly AddCourseService _addCourseService;
        private readonly DropCourseService _dropCourseService;
        private readonly SwapCourseService _swapCourseService;

        public CourseEnrollmentService(AddCourseService addCourseService, DropCourseService dropCourseService, SwapCourseService swapCourseService)
        {
            _addCourseService = addCourseService; 
            _dropCourseService = dropCourseService;
            _swapCourseService = swapCourseService;
        }

        public async Task<string> AddCourse(int userNum, List<int> courseNum)
        {
            return await _addCourseService.AddCourse(userNum, courseNum);
        }

        public async Task<string> DropCourse(int userNum, int courseNum)
        {
            return await _dropCourseService.DropCourse(userNum, courseNum);
        }

        public async Task<string> SwapCourse(int userNum, int dropCourseNum, int addCourseNum)
        {
            return await _swapCourseService.SwapCourse(userNum, dropCourseNum, addCourseNum);
        }

    }

}
