using System;
using System.Collections.Generic;

namespace prjLookday.Models;

public partial class Country
{
    public int CountryId { get; set; }

    public string? Country1 { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}
