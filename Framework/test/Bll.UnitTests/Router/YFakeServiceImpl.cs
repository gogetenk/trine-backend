using Microsoft.Extensions.Logging;
using Sogetrel.Sinapse.Framework.Bll.Services;

namespace Bll.UnitTests.Router
{
    public class YFakeServiceImpl : ServiceBase, IFakeService
    {
        public YFakeServiceImpl(ILogger<ServiceBase> logger) : base(logger)
        {
        }

        public string DoWork()
        {
            return "Y";
        }
    }
}
