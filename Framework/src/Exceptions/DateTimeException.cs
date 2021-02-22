using System;

namespace Sogetrel.Sinapse.Framework.Exceptions
{
    [Serializable]
    public class DateTimeException : Exception
    {
        public DateTimeException(string message) : base(message)
        {
        }

        public DateTimeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
