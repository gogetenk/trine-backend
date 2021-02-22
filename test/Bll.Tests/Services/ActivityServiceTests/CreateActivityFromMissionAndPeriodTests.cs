using System;
using System.Threading.Tasks;
using Assistance.Event.Dal.Repositories;
using Assistance.Operational.Bll.Impl.Services;
using Assistance.Operational.Dal.Repositories;
using AutoFixture;
using Dto;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Assistance.Operational.Bll.Tests.Services.ActivityServiceTests
{
    public class CreateActivityFromMissionAndPeriodTests : TestBase
    {
        public const string _activityApiVersion = "1.0";
        public const string _missionApiVersion = "1.0";

        public CreateActivityFromMissionAndPeriodTests()
        {
        }

        [Fact]
        public async Task CreateActivityFromMissionAndPeriod_NominalCase_ExpectConsitentActivity()
        {
            // Arrange
            var mission = new Fixture().Create<MissionDto>();
            var activity = new Fixture().Create<ActivityDto>();
            var period = new DateTime(2000, 01, 01);
            activity.Commercial = It.Is<UserActivityDto>(x => x.Id == mission.Commercial.Id);
            activity.Consultant = It.Is<UserActivityDto>(x => x.Id == mission.Consultant.Id);
            activity.Customer = It.Is<UserActivityDto>(x => x.Id == mission.Customer.Id);
            activity.MissionId = It.Is<string>(x => x == mission.Id);

            var activityRepoMock = new Mock<IActivityRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var eventRepoMock = new Mock<IEventRepository>();
            var missionRepoMock = new Mock<IMissionRepository>();
            var orgaRepoMock = new Mock<IOrganizationRepository>();
            var mailRepoMock = new Mock<IMailRepository>();
            var notifRepoMock = new Mock<INotificationRepository>();
            var configurationMock = new Mock<IConfiguration>();
            var azureStorageRepoMock = new Mock<IAzureStorageRepository>();
            var internalNotificationRepository = new Mock<IInternalNotificationRepository>();

            missionRepoMock
                .Setup(x => x.ApiMissionsByIdGetAsync(mission.Id, _missionApiVersion))
                .ReturnsAsync(mission);
            activityRepoMock
                .Setup(x => x.ApiActivitiesByStartDateendDateGetAsync(period, period.AddMonths(1).AddDays(-1), _activityApiVersion))
                .ReturnsAsync(activity);
            activityRepoMock
                .Setup(x => x.ApiActivitiesPostAsync(It.IsAny<ActivityDto>(), _activityApiVersion))
                .ReturnsAsync(activity);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
                orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var result = await service.CreateActivityFromMissionAndPeriod(mission.Id, period);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(_mapper.Map<ActivityDto>(activity));
        }
    }
}
