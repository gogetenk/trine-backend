using System;

namespace Sogetrel.Sinapse.Framework.Exceptions.Extensions
{
    public static class ExceptionExtractorExtensions
    {
        public static T Extract<T>(this Exception exception) where T : Exception
        {
            // stop conditions
            if (exception == null)
            {
                return default(T);
            }
            if (exception is T)
            {
                return (T)exception;
            }
            // recursive call
            var innerException = exception.InnerException;
            return (innerException == null) ? default(T) : innerException.Extract<T>();
        }
    }
}
