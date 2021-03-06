﻿using System;

namespace CrossPlatformAdapter.Exceptions
{
    public class PlatformSpecificTypeNotFoundException : Exception
    {
        public PlatformSpecificTypeNotFoundException()
        {
        }

        public PlatformSpecificTypeNotFoundException(string message)
            : base(message)
        {
        }

        public PlatformSpecificTypeNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
