using System;
using System.Collections.Generic;

namespace Backend_University.Models;

public partial class Examregistration
{
    public int Registrationid { get; set; }

    public int? Studentid { get; set; }

    public int? Examsessionid { get; set; }

    public DateTime? Registrationdate { get; set; }

    public int? Grade { get; set; }

    public string? Status { get; set; }

    public virtual Examsession? Examsession { get; set; }

    public virtual Student? Student { get; set; }
}
