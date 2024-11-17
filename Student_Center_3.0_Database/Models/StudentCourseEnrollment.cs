using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Student_Center_3._0_Database.Utils;

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

        [Column(TypeName = "nvarchar(3)")]
        public string courseSuffix { get; set; }

        [JsonProperty("startDate")]
        [JsonConverter(typeof(CustomDateTimeConverterUtils))]
        public DateTime startDate { get; set; }

        [JsonProperty("endDate")]
        [JsonConverter(typeof(CustomDateTimeConverterUtils))]
        public DateTime endDate { get; set; }

        public double courseWeight { get; set; }
    }
}
