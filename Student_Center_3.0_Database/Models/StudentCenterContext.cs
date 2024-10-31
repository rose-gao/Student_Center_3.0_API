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

        // Implement one-to-one relationship b/w Student & Login; initialize Login's Foreign Key
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(s => s.Login)
                .WithOne(l => l.User)
                .HasForeignKey<Login>(l => l.userNum)
                .IsRequired();
            base.OnModelCreating(modelBuilder);
        }
    }
}
