namespace Sogetrel.Sinapse.Framework.Web.Http.Correlation
{
    /// <summary>
    /// Provides a method to get the current correlation id.
    /// </summary>
    public interface ICorrelationIdAccessor
    {
        /// <summary>
        /// Gets the current correlation id.
        /// </summary>
        string GetCorrelationId();
    }
}
