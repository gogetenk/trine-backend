using System.Threading.Tasks;
using Assistance.Event.Dal.Repositories;
using Assistance.Operational.Bll.Impl.Builders;
using Assistance.Operational.Bll.Impl.Services;
using Assistance.Operational.Dal.Repositories;
using AutoFixture;
using Dto;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Sogetrel.Sinapse.Framework.Exceptions;
using Xunit;

namespace Assistance.Operational.Bll.Tests.Services.ActivityServiceTests
{
    public class GetActivityByTokenTests : TestBase
    {
        public GetActivityByTokenTests()
        {
        }

        [Fact]
        public async Task GetByToken_NominalCase()
        {
            // Arrange
            var customerEmail = new Fixture().Create<string>();
            var key = new Fixture().Create<string>();
            var activity = new Fixture().Create<ActivityDto>();
            activity.Customer.Email = customerEmail;
            var token = new TokenBuilder()
                .WithExpiration(10000)
                .WithKey(key)
                .WithEmailClaim(customerEmail)
                .WithActivityIdClaim(activity.Id)
                .GetToken();

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

            configurationMock
                .SetupGet(x => x[It.Is<string>(s => s == "AuthenticationSettings:Key")])
                .Returns(key);
            activityRepoMock
                .Setup(x => x.ApiActivitiesByIdGetAsync(activity.Id, null))
                .ReturnsAsync(activity);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
               orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var result = await service.GetByToken(token);

            // Assert
            result.Should().NotBeNull();
            result.Customer.CanSign = true;
            result.Consultant.CanSign = false;
        }

        [Fact]
        public async Task GetByToken_WhenInconsistendToken_ExpectBusinessException()
        {
            // Arrange
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

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
               orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var result = await Assert.ThrowsAsync<BusinessException>(async () => await service.GetByToken("inconsistentToken"));

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("Inconsistent token.");
        }

        [Fact]
        public async Task GetByToken_WhenActivityIdNotPresentInClaims_ExpectBusinessException()
        {
            // Arrange
            var customerEmail = new Fixture().Create<string>();
            var key = new Fixture().Create<string>();
            var token = new TokenBuilder()
                .WithExpiration(10000)
                .WithKey(key)
                .WithEmailClaim(customerEmail)
                .GetToken();

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

            configurationMock
                .SetupGet(x => x[It.Is<string>(s => s == "AuthenticationSettings:Key")])
                .Returns(key);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
               orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var result = await Assert.ThrowsAsync<BusinessException>(async () => await service.GetByToken(token));

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("Inconsistent token : the activityId claim is not present.");
        }

        [Fact]
        public async Task GetByToken_WhenActivityDoesntExist_ExpectBusinessException()
        {
            // Arrange
            var customerEmail = new Fixture().Create<string>();
            var key = new Fixture().Create<string>();
            var activity = new Fixture().Create<ActivityDto>();
            var token = new TokenBuilder()
                .WithExpiration(10000)
                .WithKey(key)
                .WithEmailClaim(customerEmail)
                .WithActivityIdClaim(activity.Id)
                .GetToken();

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

            configurationMock
                .SetupGet(x => x[It.Is<string>(s => s == "AuthenticationSettings:Key")])
                .Returns(key);
            activityRepoMock
                .Setup(x => x.ApiActivitiesByIdGetAsync(activity.Id, null))
                .ReturnsAsync(value: null);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
               orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var result = await Assert.ThrowsAsync<BusinessException>(async () => await service.GetByToken(token));

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("This activity doesn't exist.");
        }
    }
}
