using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CourseRegisterApp.CourseData;

public partial class CourseRegisterContext : DbContext
{
    public CourseRegisterContext()
    {
    }

    public CourseRegisterContext(DbContextOptions<CourseRegisterContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseUser> CourseUsers { get; set; }

    public virtual DbSet<User> Users { get; set; }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasOne(d => d.LecturerNavigation).WithMany(p => p.Courses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Course_User");
        });

        modelBuilder.Entity<CourseUser>(entity =>
        {
            entity.HasOne(d => d.Course).WithMany(p => p.CourseUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseUser_Course");

            entity.HasOne(d => d.User).WithMany(p => p.CourseUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseUser_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
