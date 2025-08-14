using System;
using System.Collections.Generic;

namespace Planorama.User.Core.Exceptions
{
    public class NotFoundException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public NotFoundException(IDictionary<string, string[]> errors) 
        {
            Errors = errors;
        }
    }
}
