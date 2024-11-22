namespace Student_Center_3._0_Services.Services
{
    public class DropCourseService
    {
        private readonly HttpClient _httpClient;

        public DropCourseService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> DropCourse(int userNum, int courseNum)
        {
            if (userNum <= 0 || courseNum <= 0)
            {
                throw new ArgumentException("UserNum and CourseNum must be greater than zero.");
            }

            var courseResponse = await _httpClient.DeleteAsync($"api/StudentCourseEnrollment/{userNum}/{courseNum}");

            if (!courseResponse.IsSuccessStatusCode)
            {
                return $"{courseResponse.StatusCode}";
            }

            return "OK";
        }
    }

}
