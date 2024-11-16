using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_Center_3._0_Database.Models
{
    public class CourseTime
    {
        [Key, ForeignKey("Course")]
        [Column(Order = 0)]
        public int courseNum { get; set; }

        [Key]
        [Column(Order = 1)]
        public int weekday {  get; set; }

        public TimeSpan startTime { get; set; }
        public TimeSpan endTime { get; set; }

    }
}
