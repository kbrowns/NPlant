using System;

namespace NPlant.Exceptions
{
    public class ConsoleException : Exception
    {
        public ConsoleException(string message) : base(message)
        {
        }

        public ConsoleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}