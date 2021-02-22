namespace Sogetrel.Sinapse.Framework.Web.Http
{
    /// <summary>
    /// Configuration for Http Clients.
    /// </summary>
    public interface IHttpClientConfiguration
    {
        /// <summary>
        /// Name for the Http Client to be used in the repositories.
        /// </summary>
        string HttpClientName { get; }
    }
}
