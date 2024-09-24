using Microsoft.EntityFrameworkCore;

namespace Student_Center_3._0_API.Models
{
    public class StudentCenterContext : DbContext
    {
        public StudentCenterContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Login> Logins { get; set; }

        // Implement one-to-one relationship b/w Student & Login; initialize Login's Foreign Key
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Login)
                .WithOne(l => l.Student)
                .HasForeignKey<Login>(l => l.studentNum)
                .IsRequired();
            base.OnModelCreating(modelBuilder);
        }
    }
}
