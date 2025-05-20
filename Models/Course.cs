using System;
using System.Collections.Generic;

namespace Backend_University.Models;

public partial class Course
{
    public int Courseid { get; set; }

    public int? Subjectid { get; set; }

    public int? Professorid { get; set; }

    public string Academicyear { get; set; } = null!;

    public int Semester { get; set; }

    public virtual ICollection<Courseenrollment> Courseenrollments { get; set; } = new List<Courseenrollment>();

    public virtual ICollection<Examsession> Examsessions { get; set; } = new List<Examsession>();

    public virtual Professor? Professor { get; set; }

    public virtual Subject? Subject { get; set; }
}
