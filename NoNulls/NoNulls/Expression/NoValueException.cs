using System;

namespace NoNulls
{
    public class NoValueException : Exception
    {
        public NoValueException(string message, params object[] properties) : base(String.Format(message, properties))
        {
            
        }
    }
}