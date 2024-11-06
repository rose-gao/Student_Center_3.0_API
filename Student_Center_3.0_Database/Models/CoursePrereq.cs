using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Student_Center_3._0_Database.Models
{
    public class CoursePrereq
    {
        [Key]
        [Column(Order = 0)]
        public string courseName { get; set; } // FK to Course

        [Key]
        [Column(Order = 1)]
        public string prerequisite { get; set; } // either a course or a group

        public int groupId { get; set; } // FK to PrereqGroup

        // Navigation properties: allows the full attributes of the object to be easily accessed
        [ForeignKey("courseName")]
        public Course Course { get; set; }

        [ForeignKey("groupId")]
        public PrereqGroup PrereqGroup { get; set; }
    }
}
