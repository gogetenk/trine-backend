using System;
using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Sogetrel.Sinapse.Framework.Bll.Router;
using Sogetrel.Sinapse.Framework.Bll.Services;
using Sogetrel.Sinapse.Framework.Exceptions;
using Xunit;

namespace Bll.UnitTests.Router
{
    [Trait("Category", "UnitTests")]
    public class ServiceFactoryTest
    {
        private const string _tenantTypeName = "TenantType";

        [Fact]
        public void GetService_NominalCase_ExpectServiceBaseInstance()
        {
            var loggerMock = new Mock<ILogger<ServiceBase>>();
            var xTenant = "XTenant";
            var yTenant = "YTenant";

            var sinapseServiceCollection = new List<Tuple<string, ServiceBase>>
            {
                new Tuple<string, ServiceBase>(xTenant, new XFakeServiceImpl(loggerMock.Object)),
                new Tuple<string, ServiceBase>(yTenant, new YFakeServiceImpl(loggerMock.Object))
            };

            var claims = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(_tenantTypeName, xTenant)
            });

            var serviceFactory = new ServiceFactory(sinapseServiceCollection);
            var resolvedService = serviceFactory.GetService<IFakeService>(claims);

            resolvedService.Should().BeOfType<XFakeServiceImpl>();
            resolvedService.DoWork().Should().Be("X");

            //Changement de tenant
            claims = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(_tenantTypeName, yTenant)
            });

            serviceFactory = new ServiceFactory(sinapseServiceCollection);
            resolvedService = serviceFactory.GetService<IFakeService>(claims);

            resolvedService.Should().BeOfType<YFakeServiceImpl>();
            resolvedService.DoWork().Should().Be("Y");
        }


        [Fact]
        public void GetService_WhenServiceIsNotRegistered_ExpectTechnicalException()
        {
            var loggerMock = new Mock<ILogger<ServiceBase>>();
            var xTenant = "XTenant";

            var sinapseServiceCollection = new List<Tuple<string, ServiceBase>>();

            var claims = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(_tenantTypeName, xTenant)
            });

            var serviceFactory = new ServiceFactory(sinapseServiceCollection);
            var exc = Assert.Throws<TechnicalException>(() => serviceFactory.GetService<IFakeService>(claims));

            exc.Should().NotBeNull();
        }

        [Fact]
        public void GetService_WhenClaimValueIsEmpty_ExpectBusinessException()
        {
            var loggerMock = new Mock<ILogger<ServiceBase>>();
            var sinapseServiceCollection = new List<Tuple<string, ServiceBase>>();

            var claims = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(_tenantTypeName, "")
            });

            var serviceFactory = new ServiceFactory(sinapseServiceCollection);
            var exc = Assert.Throws<BusinessException>(() => serviceFactory.GetService<IFakeService>(claims));

            exc.Should().NotBeNull();
        }

        [Fact]
        public void GetService_WhenClaimIsNull_ExpectBusinessException()
        {
            var loggerMock = new Mock<ILogger<ServiceBase>>();
            var sinapseServiceCollection = new List<Tuple<string, ServiceBase>>();

            var claims = new ClaimsIdentity(new List<Claim>()
            {
                new Claim("","")
            });

            var serviceFactory = new ServiceFactory(sinapseServiceCollection);
            var exc = Assert.Throws<BusinessException>(() => serviceFactory.GetService<IFakeService>(claims));

            exc.Should().NotBeNull();
        }
    }
}
