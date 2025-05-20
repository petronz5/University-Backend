using System;
using System.Collections.Generic;

namespace Backend_University.Models;

public partial class Userrole
{
    public int Userroleid { get; set; }

    public int? Userid { get; set; }

    public int? Roleid { get; set; }

    public virtual Role? Role { get; set; }

    public virtual User? User { get; set; }
}
