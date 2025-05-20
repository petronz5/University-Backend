using System;
using System.Collections.Generic;

namespace Backend_University.Models;

public partial class Professor
{
    public int Professorid { get; set; }

    public int? Userid { get; set; }

    public string? Department { get; set; }

    public DateOnly? Hiredate { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual User? User { get; set; }
}
