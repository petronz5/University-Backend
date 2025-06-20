using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend_University.Models;

[Table("courses")]
public partial class Course
{
    [Key]
    [Column("courseid")]
    public int Courseid { get; set; }

    [Column("subjectid")]
    public int? Subjectid { get; set; }

    [Column("professorid")]
    public int? Professorid { get; set; }

    [Column("academicyear")]
    [StringLength(10)]
    public string Academicyear { get; set; } = null!;

    [Column("semester")]
    public int Semester { get; set; }

    [InverseProperty("Course")]
    public virtual ICollection<Courseenrollment> Courseenrollments { get; set; } = new List<Courseenrollment>();

    [InverseProperty("Course")]
    public virtual ICollection<Examsession> Examsessions { get; set; } = new List<Examsession>();

    [ForeignKey("Professorid")]
    [InverseProperty("Courses")]
    public virtual Professor? Professor { get; set; }

    [ForeignKey("Subjectid")]
    [InverseProperty("Courses")]
    public virtual Subject? Subject { get; set; }
}
