﻿using Foxstrap.Enums;
using System;

namespace Foxstrap.Models
{
    public class FoxstrapUser
    {
        public string Username { get; set; } = "";
        public string RobloxId { get; set; } = "";
        public UserRole Role { get; set; } = UserRole.Tester;
        public DateTime AddedAt { get; set; } = DateTime.Now;
        public string AddedBy { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }
}

