using Microsoft.EntityFrameworkCore;

namespace Student_Center_3._0_Database.Models
{
    public class StudentCenterContext : DbContext
    {
        public StudentCenterContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Course> Courses { get; set; }  
        public DbSet<CoursePrereq> CoursePrereqs { get; set; }
        public DbSet<PrereqGroup> PrereqGroups { get; set; } 

        // Implement one-to-one relationship b/w Student & Login; initialize Login's Foreign Key
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(s => s.Login)
                .WithOne(l => l.User)
                .HasForeignKey<Login>(l => l.userNum)
                .IsRequired();
            base.OnModelCreating(modelBuilder);

            // Composite key for CoursePrerequisite
            modelBuilder.Entity<CoursePrereq>()
                .HasKey(cp => new { cp.CourseNum, cp.PrerequisiteNum, cp.GroupId });

            // One-to-many relationship between Course and CoursePrerequisite
            modelBuilder.Entity<CoursePrereq>()
                .HasOne(cp => cp.Course)
                .WithMany(c => c.Prerequisites)
                .HasForeignKey(cp => cp.CourseNum)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship between CoursePrerequisite and PrerequisiteGroup
            modelBuilder.Entity<CoursePrereq>()
                .HasOne(cp => cp.PrereqGroup)
                .WithMany(pg => pg.CoursePrereqs)
                .HasForeignKey(cp => cp.GroupId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
