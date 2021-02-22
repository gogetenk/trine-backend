using Sogetrel.Sinapse.Framework.Model;

namespace Sogetrel.Sinapse.Framework.Bll
{
    /// <summary>
    /// Interface to get App version
    /// </summary>
    public interface IAppVersionService
    {
        /// <summary>
        /// Returns AppVersionModel
        /// </summary>
        AppVersionModel GetAppVersion();
    }
}
