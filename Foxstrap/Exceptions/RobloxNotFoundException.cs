﻿using System;

namespace Foxstrap.Exceptions
{
    public class RobloxNotFoundException : Exception
    {
        public RobloxNotFoundException()
            : base("Roblox installation not found. Please install Roblox first.") { }

        public RobloxNotFoundException(string message)
            : base(message) { }
    }
}

