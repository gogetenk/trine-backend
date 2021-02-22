using Microsoft.Extensions.Logging;
using Sogetrel.Sinapse.Framework.Bll.Services;

namespace Bll.UnitTests.Router
{
    public class XFakeServiceImpl : ServiceBase, IFakeService
    {
        public XFakeServiceImpl(ILogger<ServiceBase> logger) : base(logger)
        {
        }

        public string DoWork()
        {
            return "X";
        }
    }
}
