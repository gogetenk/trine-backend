using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Operational.Bll.Impl.Errors;
using Assistance.Operational.Bll.Impl.Services;
using Assistance.Operational.Dal.Repositories;
using Assistance.Operational.Dto;
using AutoFixture;
using Dto;
using FluentAssertions;
using Moq;
using Sogetrel.Sinapse.Framework.Exceptions;
using Xunit;
using DayPartEnum = Dto.DayPartEnum;

namespace Assistance.Operational.Bll.Tests.Services.DashboardServiceTests
{
    public class DashboardServiceTests : TestBase
    {
        public DashboardServiceTests() : base()
        {
        }

        [Fact]
        public async Task GetCountsFromUserTest_NominalCase()
        {
            //Arrange
            var clientId = new Fixture().Create<string>();
            var missionList = new Fixture().CreateMany<MissionDto>(15).ToList();
            missionList[0].Customer.Id = clientId;
            missionList[1].Customer.Id = clientId;
            missionList[2].Customer.Id = clientId;
            int eventCount = 10;
            var user = CreateUserFixture();

            var missionRepoMock = new Mock<IMissionRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var eventRepoMock = new Mock<IEventRepository>();
            var organizationRepoMock = new Mock<IOrganizationRepository>();
            var activityRepoMock = new Mock<IActivityRepository>();

            missionRepoMock.Setup(x => x.ApiMissionsByUserIdByActiveGetAsync(user.Id, true, It.IsAny<string>())).ReturnsAsync(missionList);
            eventRepoMock.Setup(x => x.ApiEventsTargetsByTargetIdCountGetAsync(user.Id, null)).ReturnsAsync(eventCount);
            userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(user.Id, It.IsAny<string>())).ReturnsAsync(user);

            var service = new DashboardService(
                _loggerMock.Object,
                _mapper,
                userRepoMock.Object,
                missionRepoMock.Object,
                eventRepoMock.Object,
                organizationRepoMock.Object,
                activityRepoMock.Object);

            //Act
            var results = await service.GetCountsFromUser(user.Id);

            //Assert
            results.Should().NotBeNull();
            results.EventCount.Should().Be(eventCount);
            results.MissionCount.Should().Be(missionList.Count);
            results.ClientCount.Should().Be(13);
            results.CurrentUser.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task GetCountsFromUserTest_WhenResultsAreNull_Expect0Count()
        {
            //Arrange
            var missionList = default(List<MissionDto>);
            int eventCount = 0;
            var user = CreateUserFixture();

            var missionRepoMock = new Mock<IMissionRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var eventRepoMock = new Mock<IEventRepository>();
            var activityRepoMock = new Mock<IActivityRepository>();
            var organizationRepoMock = new Mock<IOrganizationRepository>();

            missionRepoMock.Setup(x => x.ApiMissionsByUserIdByActiveGetAsync(user.Id, true, It.IsAny<string>())).ReturnsAsync(missionList);
            eventRepoMock.Setup(x => x.ApiEventsTargetsByTargetIdCountGetAsync(user.Id, null)).ReturnsAsync(eventCount);
            userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(user.Id, It.IsAny<string>())).ReturnsAsync(user);

            var service = new DashboardService(
                _loggerMock.Object,
                _mapper,
                userRepoMock.Object,
                missionRepoMock.Object,
                eventRepoMock.Object,
                organizationRepoMock.Object,
                activityRepoMock.Object);

            //Act
            var results = await service.GetCountsFromUser(user.Id);

            //Assert
            results.Should().NotBeNull();
            results.EventCount.Should().Be(0);
            results.MissionCount.Should().Be(0);
            results.ClientCount.Should().Be(0);
            results.CurrentUser.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task GetSalesDashboard_WhenNoOrganization_ExpectException()
        {
            //Arrange
            var user = CreateUserFixture();

            var missionRepoMock = new Mock<IMissionRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var eventRepoMock = new Mock<IEventRepository>();
            var activityRepoMock = new Mock<IActivityRepository>();
            var organizationRepoMock = new Mock<IOrganizationRepository>();

            userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(user.Id, It.IsAny<string>())).ReturnsAsync(user);

            var service = new DashboardService(
                _loggerMock.Object,
                _mapper,
                userRepoMock.Object,
                missionRepoMock.Object,
                eventRepoMock.Object,
                organizationRepoMock.Object,
                activityRepoMock.Object);

            //Act
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.GetSalesDashboard(user.Id));

            //Assert
            exc.Should().NotBeNull();
            exc.Message.Should().Be(ErrorMessages.wrongUserRole);
        }

        [Fact]
        public async Task GetSalesDashboard_NominalCase_ExpectDashboardData()
        {
            //Arrange
            var user = CreateUserFixture();

            user.GlobalRole = UserDto.GlobalRoleEnum.Sales;
            var organization = new Fixture().Create<OrganizationDto>();
            organization.OwnerId = user.Id;

            var mission = new Fixture().Create<MissionDto>();
            mission.OrganizationId = organization.Id;
            mission.OrganizationName = organization.Name;
            mission.OrganizationIcon = organization.Icon;
            mission.DailyPrice = 500;

            var activity = new Fixture().Create<ActivityDto>();
            activity.MissionId = mission.Id;

            activity.Days = new List<GridDayDto>()
            {
                new GridDayDto()
                {
                    Day = new DateTime(2020, 1, 1),
                    IsOpen = true,
                    WorkedPart = DayPartEnum.Full
                },
                new GridDayDto()
                {
                    Day = new DateTime(2020, 1, 2),
                    IsOpen = true,
                    WorkedPart = DayPartEnum.Full
                },
                new GridDayDto()
                {
                    Day = new DateTime(2020, 1, 3),
                    IsOpen = true,
                    WorkedPart = DayPartEnum.None
                }
            };

            var missionRepoMock = new Mock<IMissionRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var eventRepoMock = new Mock<IEventRepository>();
            var activityRepoMock = new Mock<IActivityRepository>();
            var organizationRepoMock = new Mock<IOrganizationRepository>();

            userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(user.Id, It.IsAny<string>())).ReturnsAsync(user);
            organizationRepoMock.Setup(x => x.GetOrganization(user.Id)).Returns(organization);
            missionRepoMock.Setup(x => x.ApiMissionsOrganizationsByIdGetAsync(organization.Id, It.IsAny<string>())).ReturnsAsync(new List<MissionDto>() { mission });

            var missionIds = new List<string> { mission.Id };
            activityRepoMock.Setup(x => x.ApiActivitiesMissionsByMissionIdsGetAsync(missionIds, null)).ReturnsAsync(new List<ActivityDto>()
            {
                activity
            });

            var service = new DashboardService(
                _loggerMock.Object,
                _mapper,
                userRepoMock.Object,
                missionRepoMock.Object,
                eventRepoMock.Object,
                organizationRepoMock.Object,
                activityRepoMock.Object);

            //Act
            var dashboardData = await service.GetSalesDashboard(user.Id);

            //Assert
            Assert.Equal(1000, dashboardData.SalesRevenue);
            Assert.Equal(2, dashboardData.WorkedDays);
            Assert.Equal(3, dashboardData.TotalDays);
            Assert.Single(dashboardData.Missions);
        }

        [Fact]
        public async Task GetCountsFromUserTest_WhenUserIsNull_ExpectBusinessException()
        {
            //Arrange
            var clientId = new Fixture().Create<string>();
            var user = CreateUserFixture();

            var missionRepoMock = new Mock<IMissionRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var eventRepoMock = new Mock<IEventRepository>();
            var activityRepoMock = new Mock<IActivityRepository>();
            var organizationRepoMock = new Mock<IOrganizationRepository>();
            userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(user.Id, It.IsAny<string>())).ReturnsAsync(value: null);

            var service = new DashboardService(
                _loggerMock.Object,
                _mapper,
                userRepoMock.Object,
                missionRepoMock.Object,
                eventRepoMock.Object,
                organizationRepoMock.Object,
                activityRepoMock.Object);

            //Act
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.GetCountsFromUser(user.Id));

            //Assert
            exc.Should().NotBeNull();
            exc.Message.Should().Be(ErrorMessages.userCannotBeNull);
        }

        [Fact]
        public async Task GetDashboardData_NominalCase()
        {
            //Arrange
            var user = CreateUserFixture();
            var activitiesToReturn = new Fixture().Create<List<ActivityDto>>();
            activitiesToReturn.ForEach(x => x.DaysNumber = (float)new Random().Next());
            var missionRepoMock = new Mock<IMissionRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var eventRepoMock = new Mock<IEventRepository>();
            var activityRepoMock = new Mock<IActivityRepository>();
            var organizationRepoMock = new Mock<IOrganizationRepository>();
            activityRepoMock
                .Setup(x => x.ApiActivitiesUsersByUserIdGetAsync(user.Id, null))
                .ReturnsAsync(activitiesToReturn);

            var service = new DashboardService(
                _loggerMock.Object,
                _mapper,
                userRepoMock.Object,
                missionRepoMock.Object,
                eventRepoMock.Object,
                organizationRepoMock.Object,
                activityRepoMock.Object);

            //Act
            var result = await service.GetDashboardData(user.Id);

            //Assert
            result.Should().NotBeNull();
            result.Activities.Should().BeEquivalentTo(activitiesToReturn);
            result.Activities.Should().BeInDescendingOrder(x => x.StartDate);
        }

        [Fact]
        public async Task GetDashboardData_WhenNoActivities_ExpectEmpty()
        {
            //Arrange
            var user = CreateUserFixture();

            var missionRepoMock = new Mock<IMissionRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var eventRepoMock = new Mock<IEventRepository>();
            var activityRepoMock = new Mock<IActivityRepository>();
            var organizationRepoMock = new Mock<IOrganizationRepository>();
            activityRepoMock
                .Setup(x => x.ApiActivitiesUsersByUserIdGetAsync(user.Id, null))
                .ReturnsAsync(value: null);

            var service = new DashboardService(
                _loggerMock.Object,
                _mapper,
                userRepoMock.Object,
                missionRepoMock.Object,
                eventRepoMock.Object,
                organizationRepoMock.Object,
                activityRepoMock.Object);

            //Act
            var result = await service.GetDashboardData(user.Id);

            //Assert
            result.Should().NotBeNull();
            result.Activities.Should().BeNull();
            result.Indicators.Should().BeNull();
            result.NewActivityBanner.Should().BeNull();
        }

        //[Fact]
        //public async Task GetEventsFromUser_NominalCase()
        //{
        //    //Arrange
        //    var userId = new Fixture().Create<string>();
        //    var eventList = new Fixture().CreateMany<ModelEventDto>(15).ToList();

        //    var missionRepoMock = new Mock<IMissionApi>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var companyRepoMock = new Mock<ICompanyRepository>();
        //    var invoiceRepoMock = new Mock<IInvoiceRepository>();
        //    var organizationRepoMock = new Mock<IOrganizationApi>();
        //    eventRepoMock.Setup(x => x.ApiEventsTargetsByTargetIdGetAsync(userId, null)).ReturnsAsync(eventList);

        //    var service = new DashboardService(_loggerMock.Object, _mapper, userRepoMock.Object, missionRepoMock.Object, eventRepoMock.Object,  invoiceRepoMock.Object, organizationRepoMock.Object);

        //    //Act
        //    var results = await service.GetEventsFromUser(userId);

        //    //Assert
        //    results.Should().NotBeNull();
        //    results.Count().Should().Be(eventList.Count);
        //}

        //[Fact]
        //public async Task GetEventsFromUser_WhenResultIsNull_ExpectNull()
        //{
        //    //Arrange
        //    var userId = new Fixture().Create<string>();

        //    var missionRepoMock = new Mock<IMissionApi>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var companyRepoMock = new Mock<ICompanyRepository>();
        //    var invoiceRepoMock = new Mock<IInvoiceRepository>();
        //    var organizationRepoMock = new Mock<IOrganizationApi>();
        //    eventRepoMock.Setup(x => x.ApiEventsUserByUserIdGetAsync(userId, null)).ReturnsAsync(default(List<ModelEventDto>));

        //    var service = new DashboardService(_loggerMock.Object, _mapper, userRepoMock.Object, missionRepoMock.Object, eventRepoMock.Object,  invoiceRepoMock.Object, organizationRepoMock.Object);

        //    //Act
        //    var results = await service.GetEventsFromUser(userId);

        //    //Assert
        //    results.Should().BeNull();
        //}

    }
}
