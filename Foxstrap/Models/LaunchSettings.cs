﻿namespace Foxstrap.Models
{
    public class LaunchSettings
    {
        public string? LaunchUrl { get; set; }
        public bool IsStudio { get; set; } = false;
        public bool ApplyMods { get; set; } = true;
        public bool ApplyFastFlags { get; set; } = true;
    }
}

