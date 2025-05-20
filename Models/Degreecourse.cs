using System;
using System.Collections.Generic;

namespace Backend_University.Models;

public partial class Degreecourse
{
    public int Degreecourseid { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? Durationinyears { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
