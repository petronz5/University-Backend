using System;
using System.Collections.Generic;

namespace Backend_University.Models;

public partial class Availableexam
{
    public int? Examsessionid { get; set; }

    public int? Courseid { get; set; }

    public string? Subjectname { get; set; }

    public string? Professorname { get; set; }

    public DateTime? Examdate { get; set; }

    public DateTime? Registrationdeadline { get; set; }
}
