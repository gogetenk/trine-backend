using Assistance.Operational.WebApi.MappingProfiles;
using AutoFixture;
using AutoMapper;
using Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Sogetrel.Sinapse.Framework.Bll.Services;

namespace Assistance.Operational.Bll.Tests.Services
{
    public abstract class TestBase
    {
        protected readonly Mock<IConfiguration> _configurationMock;
        protected readonly Mock<ILogger<ServiceBase>> _loggerMock;
        protected readonly IMapper _mapper;
        protected readonly Mock<IConfiguration> _configuration;

        public TestBase()
        {
            _loggerMock = new Mock<ILogger<ServiceBase>>();
            _configurationMock = new Mock<IConfiguration>();
            _mapper = BuildAutoMapper();
            _configuration = new Mock<IConfiguration>();
        }

        protected IMapper BuildAutoMapper()
        {
            var config = new MapperConfiguration(x =>
            {
                x.AddProfile(new MappingProfile());
                x.AllowNullCollections = true;
            });

            var mapper = config.CreateMapper();
            mapper.ConfigurationProvider.AssertConfigurationIsValid();

            return mapper;
        }

        protected UserDto CreateUserFixture()
        {
            return new Fixture().Create<UserDto>();
        }
    }
}
