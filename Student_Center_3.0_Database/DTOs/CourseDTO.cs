using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Student_Center_3._0_Database.DTOs
{
    public class CourseDTO
    {
        public string courseName { get; set; }
        public string courseSuffix { get; set; }
        public string courseAlias{ get; set; }
        public string courseDesc { get; set; }
        public string extraInformation { get; set; }
        public string prerequisites { get; set; }
        public string antirequisites { get; set; }
        public double courseWeight { get; set; }  
        public string courseSemester { get; set; }
        public string courseTime { get; set; } 
        public string? instructor { get; set; }
        public string? room { get; set; }
        public int numEnrolled { get; set; }
        public int totalSeats { get; set; }
    }
}
