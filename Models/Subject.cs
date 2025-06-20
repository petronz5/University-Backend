using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend_University.Models;

[Table("subjects")]
public partial class Subject
{
    [Key]
    [Column("subjectid")]
    public int Subjectid { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("credits")]
    public int Credits { get; set; }

    [Column("degreecourseid")]
    public int? Degreecourseid { get; set; }

    [InverseProperty("Subject")]
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    [ForeignKey("Degreecourseid")]
    [InverseProperty("Subjects")]
    public virtual Degreecourse? Degreecourse { get; set; }
}
