using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Student_Center_3._0_Database.Models
{
    public class CourseAntirequisite
    {
        [Key]
        [Column(Order = 0, TypeName = "nvarchar(60)")]
        public string course { get; set; }

        [Key]
        [Column(Order = 1,TypeName = "nvarchar(60)")]
        public string antirequisite { get; set; }
    }
}
