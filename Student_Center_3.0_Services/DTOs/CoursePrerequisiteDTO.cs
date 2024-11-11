using System.ComponentModel.DataAnnotations.Schema;

namespace Student_Center_3._0_Services.DTOs
{
    public class CoursePrerequisiteDTO
    {
        public string course { get; set; }
        public string prerequisiteExpression { get; set; }
    }
}
