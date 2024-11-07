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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // initialize foreign key in Login
            modelBuilder.Entity<User>()
                .HasOne(s => s.Login)
                .WithOne(l => l.User)
                .HasForeignKey<Login>(l => l.userNum)
                .IsRequired();
            base.OnModelCreating(modelBuilder);

            // Initialize Composite Key for CoursePrereq
            // necessary for groupId to be part of composite key?
            modelBuilder.Entity<CoursePrereq>()
                .HasKey(cp => new { cp.courseName, cp.prerequisite });


            /*
            // Initialize foreign key "course" in CoursePrereq + one : many relationship between courses : prerequisites
            modelBuilder.Entity<CoursePrereq>()
                .HasOne(cp => cp.Course)
                .WithMany(c => c.CoursePrereqs)
                .HasForeignKey(cp => cp.courseName)
                .HasPrincipalKey(c => c.courseName)
                .OnDelete(DeleteBehavior.Restrict); // Prevents cascade deletion if a course is removed
            */

            // Initialize foreign key "groupId" in CoursePrerq + one : many relationship between PrereqGroup entry : CoursePrereq entries
            modelBuilder.Entity<CoursePrereq>()
                .HasOne(cp => cp.PrereqGroup)
                .WithMany(pg => pg.CoursePrereqs)
                .HasForeignKey(cp => cp.groupId);
        }
    }
}
