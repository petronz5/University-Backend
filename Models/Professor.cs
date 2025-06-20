using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend_University.Models;

[Table("professors")]
[Index("Userid", Name = "professors_userid_key", IsUnique = true)]
public partial class Professor
{
    [Key]
    [Column("professorid")]
    public int Professorid { get; set; }

    [Column("userid")]
    public int? Userid { get; set; }

    [Column("department")]
    [StringLength(255)]
    public string? Department { get; set; }

    [Column("hiredate")]
    public DateOnly? Hiredate { get; set; }

    [InverseProperty("Professor")]
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    [ForeignKey("Userid")]
    [InverseProperty("Professor")]
    public virtual User? User { get; set; }
}
