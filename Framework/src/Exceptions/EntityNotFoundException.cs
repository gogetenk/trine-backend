using System;

namespace Sogetrel.Sinapse.Framework.Exceptions
{
    [Serializable]
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message) : base(message)
        {
        }
        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
