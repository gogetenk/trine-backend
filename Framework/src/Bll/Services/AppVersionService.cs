using Microsoft.Extensions.Logging;
using Sogetrel.Sinapse.Framework.Model;

namespace Sogetrel.Sinapse.Framework.Bll.Services
{
    /// <summary>
    /// Service which gets application version
    /// </summary>
    public class AppVersionService : ServiceBase, IAppVersionService
    {
        private readonly IAppVersionAccessor _versionAccessor;

        public AppVersionService(ILogger<AppVersionService> logger, IAppVersionAccessor versionAccessor) : base(logger)
        {
            _versionAccessor = versionAccessor;
        }

        /// <inheritdoc />
        public AppVersionModel GetAppVersion()
        {
            return new AppVersionModel() { Version = _versionAccessor.Version };
        }
    }
}
