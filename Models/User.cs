using System;
using System.Collections.Generic;

namespace Backend_University.Models;

public partial class User
{
    public int Userid { get; set; }

    public string Email { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public DateOnly? Dateofbirth { get; set; }

    public DateTime? Createdat { get; set; }

    public bool? Isactive { get; set; }

    public virtual Professor? Professor { get; set; }

    public virtual Student? Student { get; set; }

    public virtual ICollection<Userrole> Userroles { get; set; } = new List<Userrole>();
}
