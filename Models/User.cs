﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace prjLookday.Models;

public partial class User
{
   
    public int UserId { get; set; }

    [DisplayName("姓名")]
    public string Username { get; set; }

    [DisplayName("帳號")]
    public string Email { get; set; }

    [DisplayName("密碼")]
    public string Password { get; set; }

    public int? Preferences { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<ActionJoint> ActionJoints { get; set; } = new List<ActionJoint>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<CreditCardInfo> CreditCardInfos { get; set; } = new List<CreditCardInfo>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role Role { get; set; }
}