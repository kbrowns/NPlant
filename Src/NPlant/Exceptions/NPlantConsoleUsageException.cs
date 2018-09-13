using System;

namespace NPlant.Exceptions
{
    public class NPlantConsoleUsageException : Exception
    {
        public NPlantConsoleUsageException(string message) : base(message)
        {
        }

        public NPlantConsoleUsageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}