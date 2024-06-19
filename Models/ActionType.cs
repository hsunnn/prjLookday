using System;
using System.Collections.Generic;

namespace prjLookday.Models;

public partial class ActionType
{
    public int ActionTypeId { get; set; }

    public string? Action { get; set; }

    public virtual ICollection<ActionJoint> ActionJoints { get; set; } = new List<ActionJoint>();
}
