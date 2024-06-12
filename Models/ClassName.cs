using System;
using System.Collections.Generic;

namespace prjLookday.Models;

public partial class ClassName
{
    public int ClassId { get; set; }

    public string? ClassName1 { get; set; }

    public virtual ICollection<ClassJoint> ClassJoints { get; set; } = new List<ClassJoint>();
}
