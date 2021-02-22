using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Event.Dal.Repositories;
using Assistance.Operational.Bll.Impl.Services;
using Assistance.Operational.Dal.Repositories;
using AutoFixture;
using Dto;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Assistance.Operational.Bll.Tests.Services.ActivityServiceTests
{
    public class CreateFillActivityNotifications : TestBase
    {
        public CreateFillActivityNotifications() : base()
        {
        }

        [Fact]
        public async Task CreateFillActivityNotifications_NominalCase_ExpectAllNotificationsSent()
        {
            // Arrange
            var missions = new Fixture().CreateMany<MissionDto>().ToList();
            var activities = new Fixture().CreateMany<ActivityDto>().ToList();
            var consultantIds = missions.Select(x => x.Id).Distinct().ToList();

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

            var expectedNotifContent = It.Is<Dictionary<string, string>>(y =>
                    y.ContainsKey("messageParam")
                    && y.ContainsKey("messageContent")
                    && y.ContainsValue("C'est la fin du mois !")
                    && y.ContainsValue("Il faut penser à remplir son CRA ! Vous avez au moins une mission qui nécessite votre attention."));

            activityRepoMock
                .Setup(x => x.ApiActivitiesUsersByUserIdsByDateGetAsync(It.IsAny<List<string>>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync(activities);
            missionRepoMock
                .Setup(x => x.ApiMissionsActiveByActiveGetAsync(true, It.IsAny<string>()))
                .ReturnsAsync(missions);
            notifRepoMock
                .Setup(x => x.SendTemplatedNotification(expectedNotifContent, consultantIds))
                .Returns(Task.CompletedTask);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
                orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            await service.CreateFillActivityNotifications();
        }

        [Fact]
        public async Task CreateFillActivityNotifications_WhenNoMissions_ExpectNothing()
        {
            // Arrange
            var missions = new Fixture().CreateMany<MissionDto>().ToList();
            var activities = new Fixture().CreateMany<ActivityDto>().ToList();
            var consultantIds = missions.Select(x => x.Id).Distinct().ToList();

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
                .Setup(x => x.ApiMissionsActiveByActiveGetAsync(true, It.IsAny<string>()))
                .ReturnsAsync(value: null);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object,
                missionRepoMock.Object, orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            await service.CreateFillActivityNotifications();
        }

        [Fact]
        public async Task CreateFillActivityNotifications_WhenNoActivities_ExpectNothing()
        {
            // Arrange
            var missions = new Fixture().CreateMany<MissionDto>().ToList();
            var activities = new Fixture().CreateMany<ActivityDto>().ToList();
            var consultantIds = missions.Select(x => x.Id).Distinct().ToList();

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

            var expectedNotifContent = It.Is<Dictionary<string, string>>(y =>
                    y.ContainsKey("messageParam")
                    && y.ContainsKey("messageContent")
                    && y.ContainsValue("C'est la fin du mois !")
                    && y.ContainsValue("Il faut penser à remplir son CRA ! Vous avez au moins une mission qui nécessite votre attention."));

            activityRepoMock
                .Setup(x => x.ApiActivitiesUsersByUserIdsByDateGetAsync(It.IsAny<List<string>>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync(value: null);
            missionRepoMock
                .Setup(x => x.ApiMissionsActiveByActiveGetAsync(true, It.IsAny<string>()))
                .ReturnsAsync(missions);
            notifRepoMock
                .Setup(x => x.SendTemplatedNotification(expectedNotifContent, consultantIds))
                .Returns(Task.CompletedTask);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
                orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            await service.CreateFillActivityNotifications();
        }

    }
}