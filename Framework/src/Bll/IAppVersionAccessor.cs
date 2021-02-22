namespace Sogetrel.Sinapse.Framework.Bll
{
    /// <summary>
    /// Interface to get version
    /// </summary>
    public interface IAppVersionAccessor
    {
        /// <summary>
        /// Returns the version
        /// </summary>
        string Version { get; }
    }
}
