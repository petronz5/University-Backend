using System;
using System.Collections.Generic;
using Backend_University.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_University.Data;

public partial class UniversityDbContext : DbContext
{
    public UniversityDbContext()
    {
    }

    public UniversityDbContext(DbContextOptions<UniversityDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Availableexam> Availableexams { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Courseenrollment> Courseenrollments { get; set; }

    public virtual DbSet<Degreecourse> Degreecourses { get; set; }

    public virtual DbSet<Examregistration> Examregistrations { get; set; }

    public virtual DbSet<Examsession> Examsessions { get; set; }

    public virtual DbSet<Professor> Professors { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Studentgrade> Studentgrades { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userrole> Userroles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=UniversityDb;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Availableexam>(entity =>
        {
            entity.ToView("availableexams");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Courseid).HasName("courses_pkey");

            entity.HasOne(d => d.Professor).WithMany(p => p.Courses).HasConstraintName("courses_professorid_fkey");

            entity.HasOne(d => d.Subject).WithMany(p => p.Courses).HasConstraintName("courses_subjectid_fkey");
        });

        modelBuilder.Entity<Courseenrollment>(entity =>
        {
            entity.HasKey(e => e.Enrollmentid).HasName("courseenrollments_pkey");

            entity.Property(e => e.Enrollmentdate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Course).WithMany(p => p.Courseenrollments)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("courseenrollments_courseid_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.Courseenrollments)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("courseenrollments_studentid_fkey");
        });

        modelBuilder.Entity<Degreecourse>(entity =>
        {
            entity.HasKey(e => e.Degreecourseid).HasName("degreecourses_pkey");

            entity.Property(e => e.Durationinyears).HasDefaultValue(3);
        });

        modelBuilder.Entity<Examregistration>(entity =>
        {
            entity.HasKey(e => e.Registrationid).HasName("examregistrations_pkey");

            entity.Property(e => e.Registrationdate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValueSql("'Registered'::character varying");

            entity.HasOne(d => d.Examsession).WithMany(p => p.Examregistrations)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("examregistrations_examsessionid_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.Examregistrations)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("examregistrations_studentid_fkey");
        });

        modelBuilder.Entity<Examsession>(entity =>
        {
            entity.HasKey(e => e.Examsessionid).HasName("examsessions_pkey");

            entity.Property(e => e.Isactive).HasDefaultValue(true);

            entity.HasOne(d => d.Course).WithMany(p => p.Examsessions)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("examsessions_courseid_fkey");
        });

        modelBuilder.Entity<Professor>(entity =>
        {
            entity.HasKey(e => e.Professorid).HasName("professors_pkey");

            entity.HasOne(d => d.User).WithOne(p => p.Professor)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("professors_userid_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("roles_pkey");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Studentid).HasName("students_pkey");

            entity.HasOne(d => d.Degreecourse).WithMany(p => p.Students).HasConstraintName("students_degreecourseid_fkey");

            entity.HasOne(d => d.User).WithOne(p => p.Student)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("students_userid_fkey");
        });

        modelBuilder.Entity<Studentgrade>(entity =>
        {
            entity.ToView("studentgrades");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Subjectid).HasName("subjects_pkey");

            entity.HasOne(d => d.Degreecourse).WithMany(p => p.Subjects).HasConstraintName("subjects_degreecourseid_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("users_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Isactive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Userrole>(entity =>
        {
            entity.HasKey(e => e.Userroleid).HasName("userroles_pkey");
            
            entity.Property(e => e.Userroleid)
                .HasColumnName("userroleid")
                .ValueGeneratedOnAdd();

            entity.HasOne(d => d.Role).WithMany(p => p.Userroles)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("userroles_roleid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Userroles)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("userroles_userid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
