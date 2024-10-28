using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Student_Center_3._0_API.Models
{
    public class Login
    {
        [Key, Column(TypeName = "nvarchar(15)")]
        public string userId { get; set; }

        [Column(TypeName = "nvarchar(40)")]
        public string password { get; set; }

        [ForeignKey("User")]
        public int userNum { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

    }
}
