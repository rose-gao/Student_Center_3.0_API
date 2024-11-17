using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Student_Center_3._0_Services.DTOs
{
    public class CourseTimeDTO
    {
        public int courseNum { get; set; }
        public int weekday { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
    }
}
