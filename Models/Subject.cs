using System;
using System.Collections.Generic;

namespace Backend_University.Models;

public partial class Subject
{
    public int Subjectid { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int Credits { get; set; }

    public int? Degreecourseid { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual Degreecourse? Degreecourse { get; set; }
}
