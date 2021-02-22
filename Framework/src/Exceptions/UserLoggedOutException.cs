using System;

namespace Sogetrel.Sinapse.Framework.Exceptions
{

    public class UserLoggedOutException : BusinessException
    {
        public UserLoggedOutException(string message) : base(message)
        {
        }

        public UserLoggedOutException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
