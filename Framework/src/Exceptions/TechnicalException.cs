using System;

namespace Sogetrel.Sinapse.Framework.Exceptions
{
    [Serializable]
    /// <summary>
    /// An exception that occurs along with a technical error
    /// </summary>
    public class TechnicalException : Exception
    {
        public TechnicalException(string message) : base(message)
        {
        }

        public TechnicalException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
