using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Sogetrel.Sinapse.Framework.Bll.Services;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Sogetrel.Sinapse.Framework.Bll.Router
{
    public class ServiceFactory : IServiceFactory
    {
        private readonly List<Tuple<string, ServiceBase>> _serviceDictionary;
        private const string _tenantTypeName = "TenantType";

        public ServiceFactory(List<Tuple<string, ServiceBase>> serviceDictionary)
        {
            _serviceDictionary = serviceDictionary;
        }

        public T GetService<T>(ClaimsIdentity ctx) where T : class
        {
            Claim tenantClaim = ctx.FindFirst(_tenantTypeName) ?? throw new BusinessException($"Authentication error : claim TenantType {_tenantTypeName} is missing.");
            var tenantValue = string.IsNullOrEmpty(tenantClaim.Value) ? throw new BusinessException($"Authentication error : claim TenantType is empty. Type required : {typeof(T)}") : tenantClaim.Value;

            ServiceBase service = _serviceDictionary.FirstOrDefault(x => x.Item1 == tenantValue && x.Item2 is T)?.Item2;

            if (service == null)
                throw new TechnicalException($"Authentication error : the service of type {typeof(T)} and tenant {_tenantTypeName} is not referenced in the service dictionary.");

            return service as T;
        }
    }
}
