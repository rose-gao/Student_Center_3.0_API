using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// course : lab relation -- when adding a lab, must input courseNum; then add courseNum : labNum to database table
// Finding CS 1026B in prereq CS 1026A/B, use some startwith()
namespace Student_Center_3._0_Database.Models
{
    public class Course
    {
        [Key]
        public int courseNum { get; set; }
        
        // ex. Computer Science 1026
        [Column(TypeName = "nvarchar(60)")]
        public string courseName { get; set; }
            
        // ex. "A" for Computer Science 1026A
        [Column(TypeName = "nvarchar(3)")]
        public string courseSuffix { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string courseDesc { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string extraInformation { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string? prerequisites { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string? antirequisites { get; set; }

        public int courseWeight { get; set; }

        [Column(TypeName = "nvarchar(60)")]
        public string courseSemester { get; set; }

        [Column(TypeName = "nvarchar(60)")]
        public string courseTime { get; set; }

        [Column(TypeName = "nvarchar(60)")]
        public string? instructor { get; set; }

        [Column(TypeName = "nvarchar(60)")]
        public string? room { get; set; }

        public int numEnrolled { get; set; }

        public int totalSeats  { get; set; }

        public bool isLab { get; set; }

        // Navigation property: allows access and iteration through all the CoursePrereq
        // records associated with that group
        public ICollection<CoursePrereq> CoursePrereqs { get; set; }


    }
}
