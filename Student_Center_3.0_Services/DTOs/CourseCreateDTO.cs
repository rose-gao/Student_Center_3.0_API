using System.ComponentModel.DataAnnotations;

namespace Student_Center_3._0_Services.DTOs
{
    public class CourseCreateDTO
    {
        [Required]
        [StringLength(60)]
        public string courseName { get; set; }

        [Required]
        [StringLength(1)]
        public string courseSuffix { get; set; }

        [Required]
        [StringLength(60)]
        public string courseAlias { get; set; }

        [StringLength(400)]
        public string? courseDesc { get; set; }

        [StringLength(400)]
        public string? extraInformation { get; set; }

        [StringLength(400)]
        public string? prerequisites { get; set; }

        [StringLength(400)]
        public string? antirequisites { get; set; }

        public double courseWeight { get; set; }

        [Required]
        public string startDate { get; set; }

        [Required]
        public string endDate { get; set; }

        public string? instructor { get; set; }

        public string? room { get; set; }

        [Required]
        public int numEnrolled { get; set; }

        [Required]
        public int totalSeats { get; set; }
    }
}
