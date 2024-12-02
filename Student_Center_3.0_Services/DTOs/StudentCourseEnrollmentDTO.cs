using System.Text.Json.Serialization;

namespace Student_Center_3._0_Services.DTOs
{
    public class StudentCourseEnrollmentDTO
    {
        public int userNum { get; set; }

        public int courseNum { get; set; }

        public string courseName { get; set; }

        public string courseSuffix { get; set; }

        public string courseAlias { get; set; }

        public string startDate { get; set; }

        public string endDate { get; set; }

        public double courseWeight { get; set; }
    }
}
