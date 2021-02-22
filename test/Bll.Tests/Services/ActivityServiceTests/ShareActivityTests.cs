using System.Threading.Tasks;
using Assistance.Event.Dal.Repositories;
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
    public class ShareActivityTests : TestBase
    {
        public ShareActivityTests()
        {
        }

        [Fact]
        public async Task ShareActivityWithEmail_WhenCustomerIsNotRegistered_ExpectPartialUserDataInActivityObject()
        {
            // Arrange
            var user = new Fixture().Create<UserDto>();
            var activity = new Fixture().Create<ActivityDto>();
            activity.Consultant.Id = user.Id;
            activity.Commercial = null;
            activity.Customer = null;
            var customerMail = new Fixture().Create<string>();
            var updatedActivity = activity;
            updatedActivity.Customer = new UserActivityDto()
            {
                Email = customerMail,
                ProfilePicUrl = "http://www.gravatar.com/avatar/?d=identicon"
            };

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

            activityRepoMock
                .Setup(x => x.ApiActivitiesByIdGetAsync(activity.Id, null))
                .ReturnsAsync(activity);
            activityRepoMock
                .Setup(x => x.ApiActivitiesByIdPatchAsync(activity.Id, It.Is<ActivityDto>(y => y.Customer.Email == customerMail), null))
                .ReturnsAsync(updatedActivity);
            configurationMock
                .SetupGet(x => x[It.Is<string>(s => s == "AuthenticationSettings:TokenExpirationInSeconds")])
                .Returns("200");
            configurationMock
               .SetupGet(x => x[It.Is<string>(s => s == "AuthenticationSettings:Key")])
               .Returns("bWEgY2xlZiB2cmFpbWVudCB0csOocyBkaWZmaWNpbGUgw6AgZMOpY2hpZmZyZXI=");
            configurationMock
               .SetupGet(x => x[It.Is<string>(s => s == "WebApp:Url")])
               .Returns("http://toto.com");
            configurationMock
               .SetupGet(x => x[It.Is<string>(s => s == "WebApp:SignInvitationPath")])
               .Returns("/path/to/toto");

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
               orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var result = await service.ShareActivityWithEmail(user.Id, activity.Id, customerMail);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(updatedActivity);
            result.Consultant.CanSign = false;
            result.Customer.CanSign = true;
            mailRepoMock.Verify(x => x.SendAsync(customerMail, It.IsAny<string>(), It.IsAny<object>(), null, null, null));
        }

        [Fact]
        public async Task ShareActivityWithEmail_WhenCustomerIsRegistered_ExpectFullUserDataInActivityObject_AndRegularEmailInvitationWithoutToken()
        {
            // Arrange
            var user = new Fixture().Create<UserDto>();
            var activity = new Fixture().Create<ActivityDto>();
            activity.Consultant.Id = user.Id;
            activity.Commercial = null;
            activity.Customer = null;
            var customer = new Fixture().Create<UserDto>();
            var updatedActivity = activity;
            updatedActivity.Customer = new UserActivityDto()
            {
                Id = customer.Id,
                Email = customer.Email,
                Firstname = customer.Firstname,
                Lastname = customer.Lastname,
                ProfilePicUrl = customer.ProfilePicUrl
            };
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

            activityRepoMock
                .Setup(x => x.ApiActivitiesByIdGetAsync(activity.Id, null))
                .ReturnsAsync(activity);
            userRepoMock
                .Setup(x => x.GetByEmail(customer.Email))
                .ReturnsAsync(customer);
            activityRepoMock
                .Setup(x => x.ApiActivitiesByIdPatchAsync(activity.Id, It.Is<ActivityDto>(y => y.Customer.Email == customer.Email), null))
                .ReturnsAsync(updatedActivity);
            configurationMock
               .SetupGet(x => x[It.Is<string>(s => s == "WebApp:Url")])
               .Returns("http://toto.com");
            configurationMock
               .SetupGet(x => x[It.Is<string>(s => s == "WebApp:SignActivityPath")])
               .Returns("/path/to/toto");

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
               orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var result = await service.ShareActivityWithEmail(user.Id, activity.Id, updatedActivity.Customer.Email);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(updatedActivity);
            result.Consultant.CanSign = false;
            result.Customer.CanSign = true;
            mailRepoMock.Verify(x => x.SendAsync(updatedActivity.Customer.Email, It.IsAny<string>(), It.IsAny<object>(), null, null, null));
        }

        [Fact]
        public async Task ShareActivityWithEmail_WhenActivityNotFound_ExpectBusinessException()
        {
            // Arrange
            var user = new Fixture().Create<UserDto>();
            var customerMail = new Fixture().Create<string>();


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

            activityRepoMock
                .Setup(x => x.ApiActivitiesByIdGetAsync(It.IsAny<string>(), null))
                .ReturnsAsync(value: null);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
               orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.ShareActivityWithEmail(user.Id, "toto", customerMail));

            // Assert
            exc.Should().NotBeNull();
            exc.Message.Should().BeEquivalentTo("Activity not found");
        }

        [Fact]
        public async Task ShareActivityWithEmail_WhenTheCurrentUserIsNotTheConsultant_ExpectBusinessException()
        {
            // Arrange
            var user = new Fixture().Create<UserDto>();
            var activity = new Fixture().Create<ActivityDto>();
            activity.Consultant.Id = "not the same id";
            activity.Commercial = null;
            activity.Customer = null;
            var customer = new Fixture().Create<UserDto>();
            var updatedActivity = activity;
            updatedActivity.Customer = new UserActivityDto()
            {
                Id = customer.Id,
                Email = customer.Email,
                Firstname = customer.Firstname,
                Lastname = customer.Lastname,
                ProfilePicUrl = customer.ProfilePicUrl
            };
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

            activityRepoMock
                .Setup(x => x.ApiActivitiesByIdGetAsync(activity.Id, null))
                .ReturnsAsync(activity);
            userRepoMock
                .Setup(x => x.GetByEmail(customer.Email))
                .ReturnsAsync(customer);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
               orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.ShareActivityWithEmail(user.Id, activity.Id, "email"));

            // Assert
            exc.Should().NotBeNull();
            exc.Message.Should().BeEquivalentTo("You have to be the consultant in order to share your activity report.");
        }
    }
}
