using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend_University.Models;

[Table("examsessions")]
public partial class Examsession
{
    [Key]
    [Column("examsessionid")]
    public int Examsessionid { get; set; }

    [Column("courseid")]
    public int? Courseid { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("examdate", TypeName = "timestamp without time zone")]
    public DateTime Examdate { get; set; }

    [Column("registrationdeadline", TypeName = "timestamp without time zone")]
    public DateTime Registrationdeadline { get; set; }

    [Column("maxparticipants")]
    public int? Maxparticipants { get; set; }

    [Column("isactive")]
    public bool? Isactive { get; set; }

    [ForeignKey("Courseid")]
    [InverseProperty("Examsessions")]
    public virtual Course? Course { get; set; }

    [InverseProperty("Examsession")]
    public virtual ICollection<Examregistration> Examregistrations { get; set; } = new List<Examregistration>();
}
