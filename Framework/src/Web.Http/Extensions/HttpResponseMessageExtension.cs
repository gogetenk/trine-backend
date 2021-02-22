using System.Net.Http;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Sogetrel.Sinapse.Framework.Web.Http.Extensions
{
    /// <summary>
    /// Extensions for System.Net.Http.HttpResponseMessage.
    /// </summary>
    public static class HttpResponseMessageExtension
    {

        /// <summary>
        /// EnsureStatusCodePassingStatusCodeToException
        /// </summary>
        /// <exception cref="HttpException ">if statusCode is not succes</exception>
        /// <returns></returns>
        public static void EnsureStatusCodePassingStatusCodeToException(this HttpResponseMessage response)
        {
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                throw new HttpException(response.StatusCode, e.Message, e);
            }
        }
    }
}
