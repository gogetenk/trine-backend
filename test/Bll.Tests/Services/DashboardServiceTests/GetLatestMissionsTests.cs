using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Operational.Bll.Impl.Errors;
using Assistance.Operational.Bll.Impl.Services;
using Assistance.Operational.Dal.Repositories;
using AutoFixture;
using Dto;
using FluentAssertions;
using Moq;
using Sogetrel.Sinapse.Framework.Exceptions;
using Xunit;

namespace Assistance.Operational.Bll.Tests.Services.DashboardServiceTests
{
    public class GetLatestMissionsTests : TestBase
    {
        private const string _userApiVersion = "1.0";

        public GetLatestMissionsTests() : base()
        {
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetMissionFromUserTest_NominalCase_ExpectOk(bool isTripartite)
        {
            //Arrange
            var consultant = new Fixture().Create<UserDto>();
            var commercial = new Fixture().Create<UserDto>();
            var customer = new Fixture().Create<UserDto>();

            var missionList = new Fixture().Create<List<MissionDto>>();
            missionList.ForEach((x) => { x.Consultant.Id = consultant.Id; x.IsTripartite = isTripartite; x.Commercial.Id = commercial.Id; x.Customer.Id = customer.Id; });
            CreateFixtures(out Mock<IMissionRepository> missionRepoMock, out Mock<IEventRepository> eventRepoMock, out Mock<IUserRepository> userRepoMock, out Mock<ICompanyRepository> companyRepoMock);
            var invoiceRepoMock = new Mock<IInvoiceRepository>();
            var organizationRepoMock = new Mock<IOrganizationRepository>();
            var activityRepoMock = new Mock<IActivityRepository>();

            missionRepoMock
                .Setup(x => x.ApiMissionsByUserIdByActiveGetAsync(It.IsAny<string>(), true, It.IsAny<string>()))
                .ReturnsAsync(missionList);
            userRepoMock
                .Setup(x => x.ApiUsersIdsGetAsync(It.IsAny<List<string>>(), It.IsAny<string>()))
                .ReturnsAsync(new List<UserDto>() { consultant, commercial, customer });
            var service = new DashboardService(
                _loggerMock.Object,
                _mapper,
                userRepoMock.Object,
                missionRepoMock.Object,
                eventRepoMock.Object,
                organizationRepoMock.Object,
                activityRepoMock.Object);

            //Act
            var results = await service.GetLatestMissionsFromUser(consultant.Id, true);

            //Assert
            results.Should().NotBeNull();
            results.Count.Should().BeGreaterThan(0);
            //results.Where(x => x.ConsultantName.Contains(consultant.Lastname))
            //       .Should().NotBeEmpty();
            results.Any(x => missionList.Any(y => y.Id == x.Id)).Should().BeTrue();
            results.Any(x => missionList.Any(y => !string.IsNullOrEmpty(y.Commercial.Id))).Should().BeTrue();
        }

        private static void CreateFixtures(out Mock<IMissionRepository> missionRepoMock, out Mock<IEventRepository> eventRepoMock, out Mock<IUserRepository> userRepoMock, out Mock<ICompanyRepository> companyRepoMock)
        {
            missionRepoMock = new Mock<IMissionRepository>();
            eventRepoMock = new Mock<IEventRepository>();
            userRepoMock = new Mock<IUserRepository>();
            companyRepoMock = new Mock<ICompanyRepository>();
        }

        [Fact]
        public async Task GetMissionDataTest_WhenTripartiteButCommercialIsNull_ExpectBusinessException()
        {
            //Arrange
            var consultant = CreateUserFixture();
            var customer = CreateUserFixture();
            var missionList = new Fixture().Create<List<MissionDto>>();
            missionList.ForEach((x) => { x.Consultant.Id = consultant.Id; x.IsTripartite = true; x.Commercial = null; x.Customer.Id = customer.Id; });

            var missionRepoMock = new Mock<IMissionRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var eventRepoMock = new Mock<IEventRepository>();
            var companyRepoMock = new Mock<ICompanyRepository>();
            var invoiceRepoMock = new Mock<IInvoiceRepository>();
            var organizationRepoMock = new Mock<IOrganizationRepository>();
            var activityRepoMock = new Mock<IActivityRepository>();

            missionRepoMock.Setup(x => x.ApiMissionsByUserIdByActiveGetAsync(consultant.Id, true, It.IsAny<string>())).ReturnsAsync(missionList);
            userRepoMock.Setup(x => x
            .ApiUsersIdsGetAsync(It.Is<List<string>>(y => y.Contains(consultant.Id) && y.Contains(customer.Id)), It.IsAny<string>()))
            .ReturnsAsync(new List<UserDto>() { _mapper.Map<UserDto>(consultant), _mapper.Map<UserDto>(customer) });

            var service = new DashboardService(
                _loggerMock.Object,
                _mapper,
                userRepoMock.Object,
                missionRepoMock.Object,
                eventRepoMock.Object,
                organizationRepoMock.Object,
                activityRepoMock.Object);

            //Act / Assert
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.GetLatestMissionsFromUser(consultant.Id, true));
            exc.Message.Should().Be(ErrorMessages.commercialCannotBeNull);
        }

        [Fact]
        public async Task GetMissionDataTest_WhenNoMissions_ExpectNull()
        {
            //Arrange
            string id = "inutile";
            var missionRepoMock = new Mock<IMissionRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var eventRepoMock = new Mock<IEventRepository>();
            var activityRepoMock = new Mock<IActivityRepository>();
            var companyRepoMock = new Mock<ICompanyRepository>();
            var invoiceRepoMock = new Mock<IInvoiceRepository>();
            var organizationRepoMock = new Mock<IOrganizationRepository>();
            missionRepoMock.Setup(x => x.ApiMissionsByUserIdByActiveGetAsync(id, true, It.IsAny<string>())).ReturnsAsync(default(List<MissionDto>));
            var service = new DashboardService(
                _loggerMock.Object,
                _mapper,
                userRepoMock.Object,
                missionRepoMock.Object,
                eventRepoMock.Object,
                organizationRepoMock.Object,
                activityRepoMock.Object);

            //Act
            var results = await service.GetLatestMissionsFromUser(id, true);

            //Assert
            results.Should().BeNull();
        }
    }
}
