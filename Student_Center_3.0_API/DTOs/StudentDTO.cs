using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Student_Center_3._0_API.DTOs
{
    public class StudentDTO
    {
        public int studentNum { get; set; }

        public string firstName { get; set; }

        public string? middleName { get; set; }

        public string lastName { get; set; }

        public string birthday { get; set; }

        public string socialInsuranceNum { get; set; }

        public string email { get; set; }

        public string phoneNum { get; set; }

        public string streetAddress { get; set; }

        public string city { get; set; }

        public string province { get; set; }

        public string postalCode { get; set; }

    }

}
