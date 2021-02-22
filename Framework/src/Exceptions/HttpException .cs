using System;
using System.Net;
using System.Net.Http;

namespace Sogetrel.Sinapse.Framework.Exceptions
{
    public class HttpException : HttpRequestException
    {
        public HttpStatusCode StatusCode { get; set; }

        public HttpException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpException(HttpStatusCode statusCode, string message, Exception inner) : base(message, inner)
        {
            StatusCode = statusCode;
        }
    }
}
