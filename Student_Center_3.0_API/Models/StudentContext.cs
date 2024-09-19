using Microsoft.EntityFrameworkCore;

namespace Student_Center_3._0_API.Models
{
    public class StudentContext : DbContext
    {
        public StudentContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
    }
}
