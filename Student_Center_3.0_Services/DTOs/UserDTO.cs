using System.ComponentModel.DataAnnotations;

namespace Student_Center_3._0_Services.DTOs
{
    public class UserDTO
    {
        [Required]
        public int userNum { get; set; }

        [Required]
        [StringLength(60)]
        public string firstName { get; set; }

        public string? middleName { get; set; }

        [Required]
        [StringLength(60)]
        public string lastName { get; set; }

        [Required]
        [StringLength(10)]
        public string birthday { get; set; }

        [Required]
        [StringLength(9, MinimumLength = 9)]
        public string socialInsuranceNum { get; set; }

        [Required]
        [StringLength(25)]
        public string email { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10)]
        public string phoneNum { get; set; }

        [Required]
        [StringLength(100)]
        public string streetAddress { get; set; }

        [Required]
        [StringLength(60)]
        public string city { get; set; }

        [Required]
        [StringLength(2)]
        public string province { get; set; }

        [Required]
        [StringLength(7)]
        public string postalCode { get; set; }

        [Required]
        public bool isAdmin { get; set; }
    }
}
