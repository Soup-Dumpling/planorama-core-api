using System;
using System.Collections.Generic;

namespace Planorama.User.Core.Exceptions
{
    public class RefreshTokenException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public RefreshTokenException(IDictionary<string, string[]> errors)
        {
            Errors = errors;
        }
    }
}
