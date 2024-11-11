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
        public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }
        public DbSet<StudentCourseHistory> StudentCourseHistories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // initialize foreign key in Login
            modelBuilder.Entity<User>()
                .HasOne(s => s.Login)
                .WithOne(l => l.User)
                .HasForeignKey<Login>(l => l.userNum)
                .IsRequired();
            base.OnModelCreating(modelBuilder);

            // initialize composite key for StudentCourseHistory
            modelBuilder.Entity<StudentCourseHistory>()
            .HasKey(sch => new { sch.userNum, sch.course });

            // Configure StudentCourseHistory foreign key relationship, userNum --> User table; one : many relationship b/w userNum : course
            modelBuilder.Entity<StudentCourseHistory>()
                .HasOne<User>()               
                .WithMany()                    // Specify that User can have many StudentCourseHistories
                .HasForeignKey(sch => sch.userNum) // Define userNum as the foreign key
                .OnDelete(DeleteBehavior.Cascade); // Specify cascade delete if a user is removed


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
