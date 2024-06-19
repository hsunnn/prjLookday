using System;
using System.Collections.Generic;

namespace prjLookday.Models;

public partial class ModelTagJoint
{
    public int TagJointId { get; set; }

    public int ModelTagId { get; set; }

    public int ModelId { get; set; }

    public virtual ActivitiesModel Model { get; set; } = null!;

    public virtual ModelTag ModelTag { get; set; } = null!;
}
