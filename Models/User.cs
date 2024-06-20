﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace prjLookday.Models;

public partial class User
{
    [DisplayName("ID")]
    public int UserId { get; set; }

    [DisplayName("姓名")]
    public string Username { get; set; }

    [DisplayName("帳號")]
    public string Email { get; set; }

    [DisplayName("密碼")]
    public string Password { get; set; }

    public int? Preferences { get; set; }

    [DisplayName("角色 ID")]
    public int RoleId { get; set; }

    [DisplayName("照片")]
    public byte[] UserPic { get; set; }

    [DisplayName("電話")]
    public string FPhone { get; set; }

    public virtual ICollection<ActionJoint> ActionJoints { get; set; } = new List<ActionJoint>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<CreditCardInfo> CreditCardInfos { get; set; } = new List<CreditCardInfo>();

    public virtual ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role Role { get; set; } = null!;
}
