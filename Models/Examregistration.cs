using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend_University.Models;

[Table("examregistrations")]
[Index("Studentid", "Examsessionid", Name = "examregistrations_studentid_examsessionid_key", IsUnique = true)]
public partial class Examregistration
{
    [Key]
    [Column("registrationid")]
    public int Registrationid { get; set; }

    [Column("studentid")]
    public int? Studentid { get; set; }

    [Column("examsessionid")]
    public int? Examsessionid { get; set; }

    [Column("registrationdate", TypeName = "timestamp without time zone")]
    public DateTime? Registrationdate { get; set; }

    [Column("grade")]
    public int? Grade { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string? Status { get; set; }

    [ForeignKey("Examsessionid")]
    [InverseProperty("Examregistrations")]
    public virtual Examsession? Examsession { get; set; }

    [ForeignKey("Studentid")]
    [InverseProperty("Examregistrations")]
    public virtual Student? Student { get; set; }
}
