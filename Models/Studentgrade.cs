using System;
using System.Collections.Generic;

namespace Backend_University.Models;

public partial class Studentgrade
{
    public int? Studentid { get; set; }

    public string? Studentname { get; set; }

    public string? Subject { get; set; }

    public int? Grade { get; set; }

    public DateTime? Examdate { get; set; }
}
