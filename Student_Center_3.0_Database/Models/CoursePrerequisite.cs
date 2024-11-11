using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Student_Center_3._0_Database.Models
{
    public class CoursePrerequisite
    {
        [Key]
        public string course { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string prerequisiteExpression {  get; set; }
    }
}
