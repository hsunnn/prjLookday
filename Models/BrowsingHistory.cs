﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjLookday.Models;

public partial class BrowsingHistory
{
    public int BrowsingHistoryId { get; set; }

    public int? UserId { get; set; }

    public int? ActivityId { get; set; }

    public DateTime? BrowseTime { get; set; }

    public virtual Activity Activity { get; set; }

    public virtual User User { get; set; }
}