﻿using System;
using System.Collections.Generic;

namespace prjLookday.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int ActivityId { get; set; }

    public DateTime? BookingDate { get; set; }

    public decimal Price { get; set; }

    public int BookingStatesId { get; set; }

    public int? Member { get; set; }

    public int? ModelId { get; set; }

    public virtual Activity Activity { get; set; } = null!;

    public virtual BookingState BookingStates { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;
}
