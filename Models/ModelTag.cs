using System;
using System.Collections.Generic;

namespace prjLookday.Models;

public partial class ModelTag
{
    public int ModelTagId { get; set; }

    public string? Tags { get; set; }

    public virtual ICollection<ModelTagJoint> ModelTagJoints { get; set; } = new List<ModelTagJoint>();
}
