using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend_University.Models;

[Table("userroles")]
[Index("Userid", "Roleid", Name = "userroles_userid_roleid_key", IsUnique = true)]
public partial class Userrole
{
    [Key]
    [Column("userroleid")]
    public int Userroleid { get; set; }

    [Column("userid")]
    public int? Userid { get; set; }

    [Column("roleid")]
    public int? Roleid { get; set; }

    [ForeignKey("Roleid")]
    [InverseProperty("Userroles")]
    public virtual Role? Role { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("Userroles")]
    public virtual User? User { get; set; }
}
