﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Student_Center_3._0_Database.Models;

#nullable disable

namespace Student_Center_3._0_Database.Migrations
{
    [DbContext(typeof(StudentCenterContext))]
    [Migration("20241112013705_CourseAntirequisites")]
    partial class CourseAntirequisites
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Student_Center_3._0_Database.Models.Course", b =>
                {
                    b.Property<int>("courseNum")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("courseNum"));

                    b.Property<string>("antirequisites")
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<string>("courseAlias")
                        .IsRequired()
                        .HasColumnType("nvarchar(60)");

                    b.Property<string>("courseDesc")
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<string>("courseName")
                        .IsRequired()
                        .HasColumnType("nvarchar(60)");

                    b.Property<string>("courseSemester")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("courseSuffix")
                        .IsRequired()
                        .HasColumnType("nvarchar(3)");

                    b.Property<string>("courseTime")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)");

                    b.Property<double>("courseWeight")
                        .HasColumnType("float");

                    b.Property<string>("extraInformation")
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<string>("instructor")
                        .HasColumnType("nvarchar(60)");

                    b.Property<int>("numEnrolled")
                        .HasColumnType("int");

                    b.Property<string>("prerequisites")
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<string>("room")
                        .HasColumnType("nvarchar(60)");

                    b.Property<int>("totalSeats")
                        .HasColumnType("int");

                    b.HasKey("courseNum");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("Student_Center_3._0_Database.Models.CourseAntirequisite", b =>
                {
                    b.Property<string>("course")
                        .HasColumnType("nvarchar(60)")
                        .HasColumnOrder(0);

                    b.Property<string>("antirequisite")
                        .HasColumnType("nvarchar(60)")
                        .HasColumnOrder(1);

                    b.HasKey("course", "antirequisite");

                    b.ToTable("CourseAntirequisite");
                });

            modelBuilder.Entity("Student_Center_3._0_Database.Models.CoursePrerequisite", b =>
                {
                    b.Property<string>("course")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("prerequisiteExpression")
                        .IsRequired()
                        .HasColumnType("nvarchar(MAX)");

                    b.HasKey("course");

                    b.ToTable("CoursePrerequisites");
                });

            modelBuilder.Entity("Student_Center_3._0_Database.Models.Login", b =>
                {
                    b.Property<string>("userId")
                        .HasColumnType("nvarchar(15)");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("nvarchar(256)");

                    b.Property<int>("userNum")
                        .HasColumnType("int");

                    b.HasKey("userId");

                    b.HasIndex("userNum")
                        .IsUnique();

                    b.ToTable("Logins");
                });

            modelBuilder.Entity("Student_Center_3._0_Database.Models.StudentCourseHistory", b =>
                {
                    b.Property<int>("userNum")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<string>("course")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnOrder(1);

                    b.Property<double>("courseWeight")
                        .HasColumnType("float");

                    b.HasKey("userNum", "course");

                    b.ToTable("StudentCourseHistories");
                });

            modelBuilder.Entity("Student_Center_3._0_Database.Models.User", b =>
                {
                    b.Property<int>("userNum")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("userNum"));

                    b.Property<string>("birthday")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("city")
                        .IsRequired()
                        .HasColumnType("nvarchar(60)");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(25)");

                    b.Property<string>("firstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(60)");

                    b.Property<bool>("isAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("lastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(60)");

                    b.Property<string>("middleName")
                        .HasColumnType("nvarchar(60)");

                    b.Property<string>("phoneNum")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("postalCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(7)");

                    b.Property<string>("province")
                        .IsRequired()
                        .HasColumnType("nvarchar(2)");

                    b.Property<string>("socialInsuranceNum")
                        .IsRequired()
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("streetAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("userNum");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Student_Center_3._0_Database.Models.Login", b =>
                {
                    b.HasOne("Student_Center_3._0_Database.Models.User", "User")
                        .WithOne("Login")
                        .HasForeignKey("Student_Center_3._0_Database.Models.Login", "userNum")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Student_Center_3._0_Database.Models.StudentCourseHistory", b =>
                {
                    b.HasOne("Student_Center_3._0_Database.Models.User", null)
                        .WithMany()
                        .HasForeignKey("userNum")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Student_Center_3._0_Database.Models.User", b =>
                {
                    b.Navigation("Login")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
