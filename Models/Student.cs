using System;
using System.Collections.Generic;

namespace Backend_University.Models;

public partial class Student
{
    public int Studentid { get; set; }

    public int? Userid { get; set; }

    public int? Degreecourseid { get; set; }

    public string Enrollmentnumber { get; set; } = null!;

    public DateOnly Enrollmentdate { get; set; }

    public virtual ICollection<Courseenrollment> Courseenrollments { get; set; } = new List<Courseenrollment>();

    public virtual Degreecourse? Degreecourse { get; set; }

    public virtual ICollection<Examregistration> Examregistrations { get; set; } = new List<Examregistration>();

    public virtual User? User { get; set; }
}
