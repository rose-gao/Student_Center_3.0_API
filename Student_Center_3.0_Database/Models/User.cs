using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

// TODO: Make Courses table, Enrollment Table + Enrollment History (with Many-to-Many relationship with User)
namespace Student_Center_3._0_Database.Models
{
    public class User
    {
        [Key]
        public int userNum { get; set; }

        [Column(TypeName = "nvarchar(60)")]
        public string firstName { get; set; }

        [Column(TypeName = "nvarchar(60)")]
        public string? middleName { get; set; }

        [Column(TypeName = "nvarchar(60)")]
        public string lastName { get; set; }

        //dd/mm/yyyy
        [Column(TypeName = "nvarchar(10)")]
        public string birthday { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string socialInsuranceNum { get; set; }

        [Column(TypeName = "nvarchar(25)")]
        public string email { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string phoneNum { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string streetAddress { get; set; }

        [Column(TypeName = "nvarchar(60)")]
        public string city { get; set; }

        [Column(TypeName = "nvarchar(2)")]
        public string province { get; set; }

        [Column(TypeName = "nvarchar(7)")]
        public string postalCode { get; set; }

        public bool isAdmin { get; set; }

        [JsonIgnore]
        public virtual Login Login { get; set; }
    }
}
