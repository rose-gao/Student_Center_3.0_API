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
        public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }
        public DbSet<CourseAntirequisite> CourseAntirequisites { get; set; }
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

            // initialize composite key for CourseAntirequisites
            modelBuilder.Entity<CourseAntirequisite>()
                .HasKey(ca => new { ca.course, ca.antirequisite });
        }
    }
}
