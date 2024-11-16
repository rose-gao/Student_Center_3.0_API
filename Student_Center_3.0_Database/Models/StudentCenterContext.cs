using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Student_Center_3._0_Database.Models;

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
        public DbSet<CourseTime> CourseTimes { get; set; }
        public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }
        public DbSet<CourseAntirequisite> CourseAntirequisites { get; set; }
        public DbSet<StudentCourseEnrollment> StudentCourseEnrollments { get; set; }
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


            // Configure StudentCourseEnrollment foreign key relationships
            modelBuilder.Entity<StudentCourseEnrollment>()
                .HasKey(sce => new { sce.userNum, sce.courseNum }); // Composite primary key

            modelBuilder.Entity<StudentCourseEnrollment>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(sce => sce.userNum)  // Set userNum as foreign key to User table
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete if a User is removed

            modelBuilder.Entity<StudentCourseEnrollment>()
                .HasOne<Course>()
                .WithMany()
                .HasForeignKey(sce => sce.courseNum) // Set courseNum as foreign key to Course table
                .OnDelete(DeleteBehavior.Cascade);   


            // initialize composite key for CourseAntirequisites
            modelBuilder.Entity<CourseAntirequisite>()
                .HasKey(ca => new { ca.course, ca.antirequisite });

            // initialize composite key, FK, and time conversions for CourseTime
            modelBuilder.Entity<CourseTime>()
                .HasKey(ct => new { ct.courseNum, ct.weekday }); // Composite key

            modelBuilder.Entity<CourseTime>()
                .Property(ct => ct.startTime)
                .HasConversion(
                    v => v,
                    v => TimeSpan.FromTicks(v.Ticks)) // Default TimeSpan to SQL conversion
                .HasColumnType("time"); // Store as SQL's time

            modelBuilder.Entity<CourseTime>()
                .Property(ct => ct.endTime)
                .HasConversion(
                    v => v,
                    v => TimeSpan.FromTicks(v.Ticks)) // Default TimeSpan to SQL conversion
                .HasColumnType("time"); // Store as SQL's time

            modelBuilder.Entity<CourseTime>()
                .HasOne<Course>()
                .WithMany()
                .HasForeignKey(ct => ct.courseNum) // Set courseNum as foreign key to Course table
                .OnDelete(DeleteBehavior.Cascade);   

        }
    }
}
