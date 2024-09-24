using System;

namespace Vectorize.NET.Exceptions
{
    public class VectorizationException : Exception
    {
        public VectorizationException() { }

        public VectorizationException(string message)
            : base(message) { }

        public VectorizationException(string message, Exception inner)
            : base(message, inner) { }
    }
}