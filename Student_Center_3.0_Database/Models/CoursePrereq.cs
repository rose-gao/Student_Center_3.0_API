using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Student_Center_3._0_Database.Models
{
    public class CoursePrereq
    {
        [Key]
        [Column(Order = 0)]
        public int CourseNum { get; set; } // FK to Course

        [Key]
        [Column(Order = 1)]
        public int PrerequisiteNum { get; set; } // FK to Course (prerequisite)

        public int GroupId { get; set; } // FK to PrerequisiteGroup

        // Navigation properties: allows the full attributes of the object to be easily accessed
        [ForeignKey("CourseNum")]
        public Course Course { get; set; }

        [ForeignKey("PrerequisiteNum")]
        public Course PrerequisiteCourse { get; set; }

        [ForeignKey("GroupId")]
        public PrereqGroup PrereqGroup { get; set; }
    }
}
