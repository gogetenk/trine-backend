using System;

namespace Sogetrel.Sinapse.Framework.Exceptions
{
    [Serializable]
    public class DalException : Exception
    {
        public DalException(string message) : base(message)
        {
        }

        public DalException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
