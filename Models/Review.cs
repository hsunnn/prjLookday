using System;
using System.Collections.Generic;

namespace prjLookday.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int UserId { get; set; }

    public int ActivityId { get; set; }

    public string Comment { get; set; } = null!;

    public double? Rating { get; set; }

    public DateTime? Date { get; set; }

    public virtual Activity Activity { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
