using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend_University.Models;

[Table("users")]
[Index("Email", Name = "users_email_key", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("userid")]
    public int Userid { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string Email { get; set; } = null!;

    [Column("passwordhash")]
    [StringLength(255)]
    public string Passwordhash { get; set; } = null!;

    [Column("firstname")]
    [StringLength(100)]
    public string Firstname { get; set; } = null!;

    [Column("lastname")]
    [StringLength(100)]
    public string Lastname { get; set; } = null!;

    [Column("dateofbirth")]
    public DateOnly? Dateofbirth { get; set; }

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime? Createdat { get; set; }

    [Column("isactive")]
    public bool? Isactive { get; set; }

    [InverseProperty("User")]
    public virtual Professor? Professor { get; set; }

    [InverseProperty("User")]
    public virtual Student? Student { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Userrole> Userroles { get; set; } = new List<Userrole>();
}
