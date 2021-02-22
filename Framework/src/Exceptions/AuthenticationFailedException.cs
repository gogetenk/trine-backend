using System;

namespace Sogetrel.Sinapse.Framework.Exceptions
{
    public class AuthenticationFailedException : Exception
    {
        public AuthenticationFailedException(string message)
            : base(message)
        {
        }

        public AuthenticationFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
