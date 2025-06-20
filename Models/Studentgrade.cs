using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend_University.Models;

[Keyless]
public partial class Studentgrade
{
    [Column("studentid")]
    public int? Studentid { get; set; }

    [Column("studentname")]
    public string? Studentname { get; set; }

    [Column("subject")]
    [StringLength(255)]
    public string? Subject { get; set; }

    [Column("grade")]
    public int? Grade { get; set; }

    [Column("examdate", TypeName = "timestamp without time zone")]
    public DateTime? Examdate { get; set; }
}
