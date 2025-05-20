using System;
using System.Collections.Generic;

namespace Backend_University.Models;

public partial class Examsession
{
    public int Examsessionid { get; set; }

    public int? Courseid { get; set; }

    public string Name { get; set; } = null!;

    public DateTime Examdate { get; set; }

    public DateTime Registrationdeadline { get; set; }

    public int? Maxparticipants { get; set; }

    public bool? Isactive { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<Examregistration> Examregistrations { get; set; } = new List<Examregistration>();
}
