﻿using System;
using System.Collections.Generic;

namespace prjLookday.Models;

public partial class ActivitiesModel
{
    public int ModelId { get; set; }

    public int? ActivityId { get; set; }

    public string? ModelName { get; set; }

    public decimal? ModelPrice { get; set; }

    public DateOnly? ModelDate { get; set; }

    public string? ModelContent { get; set; }

    public virtual Activity? Activity { get; set; }

    public virtual ICollection<ModelTagJoint> ModelTagJoints { get; set; } = new List<ModelTagJoint>();
}
