using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_Center_3._0_Database.Models
{
    public class StudentCourseHistory
    {
        [Key, ForeignKey("User")]
        public int userNum { get; set; }


    }
}
