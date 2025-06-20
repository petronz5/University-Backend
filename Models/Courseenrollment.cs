using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend_University.Models;

[Table("courseenrollments")]
[Index("Studentid", "Courseid", Name = "courseenrollments_studentid_courseid_key", IsUnique = true)]
public partial class Courseenrollment
{
    [Key]
    [Column("enrollmentid")]
    public int Enrollmentid { get; set; }

    [Column("studentid")]
    public int? Studentid { get; set; }

    [Column("courseid")]
    public int? Courseid { get; set; }

    [Column("enrollmentdate", TypeName = "timestamp without time zone")]
    public DateTime? Enrollmentdate { get; set; }

    [ForeignKey("Courseid")]
    [InverseProperty("Courseenrollments")]
    public virtual Course? Course { get; set; }

    [ForeignKey("Studentid")]
    [InverseProperty("Courseenrollments")]
    public virtual Student? Student { get; set; }
}
