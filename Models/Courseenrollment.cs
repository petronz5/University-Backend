using System;
using System.Collections.Generic;

namespace Backend_University.Models;

public partial class Courseenrollment
{
    public int Enrollmentid { get; set; }

    public int? Studentid { get; set; }

    public int? Courseid { get; set; }

    public DateTime? Enrollmentdate { get; set; }

    public virtual Course? Course { get; set; }

    public virtual Student? Student { get; set; }
}
