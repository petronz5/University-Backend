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
        => optionsBuilder.UseNpgsql("Server=localhost;Port=5433;Database=UniversityDb;User Id=postgres;Password=postgres;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Availableexam>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("availableexams");

            entity.Property(e => e.Courseid).HasColumnName("courseid");
            entity.Property(e => e.Examdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("examdate");
            entity.Property(e => e.Examsessionid).HasColumnName("examsessionid");
            entity.Property(e => e.Professorname).HasColumnName("professorname");
            entity.Property(e => e.Registrationdeadline)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("registrationdeadline");
            entity.Property(e => e.Subjectname)
                .HasMaxLength(255)
                .HasColumnName("subjectname");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Courseid).HasName("courses_pkey");

            entity.ToTable("courses");

            entity.Property(e => e.Courseid).HasColumnName("courseid");
            entity.Property(e => e.Academicyear)
                .HasMaxLength(10)
                .HasColumnName("academicyear");
            entity.Property(e => e.Professorid).HasColumnName("professorid");
            entity.Property(e => e.Semester).HasColumnName("semester");
            entity.Property(e => e.Subjectid).HasColumnName("subjectid");

            entity.HasOne(d => d.Professor).WithMany(p => p.Courses)
                .HasForeignKey(d => d.Professorid)
                .HasConstraintName("courses_professorid_fkey");

            entity.HasOne(d => d.Subject).WithMany(p => p.Courses)
                .HasForeignKey(d => d.Subjectid)
                .HasConstraintName("courses_subjectid_fkey");
        });

        modelBuilder.Entity<Courseenrollment>(entity =>
        {
            entity.HasKey(e => e.Enrollmentid).HasName("courseenrollments_pkey");

            entity.ToTable("courseenrollments");

            entity.HasIndex(e => new { e.Studentid, e.Courseid }, "courseenrollments_studentid_courseid_key").IsUnique();

            entity.Property(e => e.Enrollmentid).HasColumnName("enrollmentid");
            entity.Property(e => e.Courseid).HasColumnName("courseid");
            entity.Property(e => e.Enrollmentdate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("enrollmentdate");
            entity.Property(e => e.Studentid).HasColumnName("studentid");

            entity.HasOne(d => d.Course).WithMany(p => p.Courseenrollments)
                .HasForeignKey(d => d.Courseid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("courseenrollments_courseid_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.Courseenrollments)
                .HasForeignKey(d => d.Studentid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("courseenrollments_studentid_fkey");
        });

        modelBuilder.Entity<Degreecourse>(entity =>
        {
            entity.HasKey(e => e.Degreecourseid).HasName("degreecourses_pkey");

            entity.ToTable("degreecourses");

            entity.Property(e => e.Degreecourseid).HasColumnName("degreecourseid");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Durationinyears)
                .HasDefaultValue(3)
                .HasColumnName("durationinyears");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Examregistration>(entity =>
        {
            entity.HasKey(e => e.Registrationid).HasName("examregistrations_pkey");

            entity.ToTable("examregistrations");

            entity.HasIndex(e => new { e.Studentid, e.Examsessionid }, "examregistrations_studentid_examsessionid_key").IsUnique();

            entity.Property(e => e.Registrationid).HasColumnName("registrationid");
            entity.Property(e => e.Examsessionid).HasColumnName("examsessionid");
            entity.Property(e => e.Grade).HasColumnName("grade");
            entity.Property(e => e.Registrationdate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("registrationdate");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Registered'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Studentid).HasColumnName("studentid");

            entity.HasOne(d => d.Examsession).WithMany(p => p.Examregistrations)
                .HasForeignKey(d => d.Examsessionid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("examregistrations_examsessionid_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.Examregistrations)
                .HasForeignKey(d => d.Studentid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("examregistrations_studentid_fkey");
        });

        modelBuilder.Entity<Examsession>(entity =>
        {
            entity.HasKey(e => e.Examsessionid).HasName("examsessions_pkey");

            entity.ToTable("examsessions");

            entity.Property(e => e.Examsessionid).HasColumnName("examsessionid");
            entity.Property(e => e.Courseid).HasColumnName("courseid");
            entity.Property(e => e.Examdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("examdate");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Maxparticipants).HasColumnName("maxparticipants");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Registrationdeadline)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("registrationdeadline");

            entity.HasOne(d => d.Course).WithMany(p => p.Examsessions)
                .HasForeignKey(d => d.Courseid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("examsessions_courseid_fkey");
        });

        modelBuilder.Entity<Professor>(entity =>
        {
            entity.HasKey(e => e.Professorid).HasName("professors_pkey");

            entity.ToTable("professors");

            entity.HasIndex(e => e.Userid, "professors_userid_key").IsUnique();

            entity.Property(e => e.Professorid).HasColumnName("professorid");
            entity.Property(e => e.Department)
                .HasMaxLength(255)
                .HasColumnName("department");
            entity.Property(e => e.Hiredate).HasColumnName("hiredate");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithOne(p => p.Professor)
                .HasForeignKey<Professor>(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("professors_userid_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Rolename, "roles_rolename_key").IsUnique();

            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Rolename)
                .HasMaxLength(50)
                .HasColumnName("rolename");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Studentid).HasName("students_pkey");

            entity.ToTable("students");

            entity.HasIndex(e => e.Enrollmentnumber, "students_enrollmentnumber_key").IsUnique();

            entity.HasIndex(e => e.Userid, "students_userid_key").IsUnique();

            entity.Property(e => e.Studentid).HasColumnName("studentid");
            entity.Property(e => e.Degreecourseid).HasColumnName("degreecourseid");
            entity.Property(e => e.Enrollmentdate).HasColumnName("enrollmentdate");
            entity.Property(e => e.Enrollmentnumber)
                .HasMaxLength(50)
                .HasColumnName("enrollmentnumber");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Degreecourse).WithMany(p => p.Students)
                .HasForeignKey(d => d.Degreecourseid)
                .HasConstraintName("students_degreecourseid_fkey");

            entity.HasOne(d => d.User).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("students_userid_fkey");
        });

        modelBuilder.Entity<Studentgrade>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("studentgrades");

            entity.Property(e => e.Examdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("examdate");
            entity.Property(e => e.Grade).HasColumnName("grade");
            entity.Property(e => e.Studentid).HasColumnName("studentid");
            entity.Property(e => e.Studentname).HasColumnName("studentname");
            entity.Property(e => e.Subject)
                .HasMaxLength(255)
                .HasColumnName("subject");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Subjectid).HasName("subjects_pkey");

            entity.ToTable("subjects");

            entity.Property(e => e.Subjectid).HasColumnName("subjectid");
            entity.Property(e => e.Credits).HasColumnName("credits");
            entity.Property(e => e.Degreecourseid).HasColumnName("degreecourseid");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.HasOne(d => d.Degreecourse).WithMany(p => p.Subjects)
                .HasForeignKey(d => d.Degreecourseid)
                .HasConstraintName("subjects_degreecourseid_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Dateofbirth).HasColumnName("dateofbirth");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(100)
                .HasColumnName("firstname");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Lastname)
                .HasMaxLength(100)
                .HasColumnName("lastname");
            entity.Property(e => e.Passwordhash)
                .HasMaxLength(255)
                .HasColumnName("passwordhash");
        });

        modelBuilder.Entity<Userrole>(entity =>
        {
            entity.HasKey(e => e.Userroleid).HasName("userroles_pkey");

            entity.ToTable("userroles");

            entity.HasIndex(e => new { e.Userid, e.Roleid }, "userroles_userid_roleid_key").IsUnique();

            entity.Property(e => e.Userroleid).HasColumnName("userroleid");
            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Role).WithMany(p => p.Userroles)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("userroles_roleid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Userroles)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("userroles_userid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
