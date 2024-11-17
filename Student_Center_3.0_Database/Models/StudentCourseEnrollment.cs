using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Student_Center_3._0_Database.Models
{
    public class StudentCourseEnrollment
    {
        [Key, ForeignKey("User")]
        [Column(Order = 0)]
        public int userNum { get; set; }

        [Key, ForeignKey("Course")]
        [Column(Order = 1)]
        public int courseNum { get; set; }

        // ex. Computer Science 1026
        [Column(TypeName = "nvarchar(60)")]
        public string courseName { get; set; }

        // ex. "A" for Computer Science 1026A
        [Column(TypeName = "nvarchar(3)")]
        public string courseSuffix { get; set; }

        public double courseWeight { get; set; }
    }
}
