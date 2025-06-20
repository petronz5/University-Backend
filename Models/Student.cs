using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend_University.Models;

[Table("students")]
[Index("Enrollmentnumber", Name = "students_enrollmentnumber_key", IsUnique = true)]
[Index("Userid", Name = "students_userid_key", IsUnique = true)]
public partial class Student
{
    [Key]
    [Column("studentid")]
    public int Studentid { get; set; }

    [Column("userid")]
    public int? Userid { get; set; }

    [Column("degreecourseid")]
    public int? Degreecourseid { get; set; }

    [Column("enrollmentnumber")]
    [StringLength(50)]
    public string Enrollmentnumber { get; set; } = null!;

    [Column("enrollmentdate")]
    public DateOnly Enrollmentdate { get; set; }

    [InverseProperty("Student")]
    public virtual ICollection<Courseenrollment> Courseenrollments { get; set; } = new List<Courseenrollment>();

    [ForeignKey("Degreecourseid")]
    [InverseProperty("Students")]
    public virtual Degreecourse? Degreecourse { get; set; }

    [InverseProperty("Student")]
    public virtual ICollection<Examregistration> Examregistrations { get; set; } = new List<Examregistration>();

    [ForeignKey("Userid")]
    [InverseProperty("Student")]
    public virtual User? User { get; set; }
}
