using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend_University.Models;

[Keyless]
public partial class Availableexam
{
    [Column("examsessionid")]
    public int? Examsessionid { get; set; }

    [Column("courseid")]
    public int? Courseid { get; set; }

    [Column("subjectname")]
    [StringLength(255)]
    public string? Subjectname { get; set; }

    [Column("professorname")]
    public string? Professorname { get; set; }

    [Column("examdate", TypeName = "timestamp without time zone")]
    public DateTime? Examdate { get; set; }

    [Column("registrationdeadline", TypeName = "timestamp without time zone")]
    public DateTime? Registrationdeadline { get; set; }
}
