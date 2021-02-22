using System.Security.Claims;

namespace Sogetrel.Sinapse.Framework.Bll.Router
{
    public interface IServiceFactory
    {
        /// <summary>
        /// This methods takes a ServiceBase and returns the registered instance associated in the Startup.cs ConfigureServices method, depending on the User identity claims
        /// </summary>
        /// <typeparam name="T">Type to request</typeparam>
        /// <param name="ctx">User Identity claims contained in the authentication context</param>
        /// <returns>The instance of type T registered in the Router</returns>
        T GetService<T>(ClaimsIdentity ctx) where T : class;
    }
}
