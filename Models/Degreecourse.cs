using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend_University.Models;

[Table("degreecourses")]
public partial class Degreecourse
{
    [Key]
    [Column("degreecourseid")]
    public int Degreecourseid { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("durationinyears")]
    public int? Durationinyears { get; set; }

    [InverseProperty("Degreecourse")]
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    [InverseProperty("Degreecourse")]
    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
