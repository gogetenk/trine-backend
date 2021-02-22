using Microsoft.Extensions.Logging;

namespace Sogetrel.Sinapse.Framework.Bll.Services
{
    public abstract class ServiceBase
    {
        protected ILogger<ServiceBase> Logger { get; private set; }

        protected ServiceBase(ILogger<ServiceBase> logger)
            => (Logger) = (logger);
    }
}
