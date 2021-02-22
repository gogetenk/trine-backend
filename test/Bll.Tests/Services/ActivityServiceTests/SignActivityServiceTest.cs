using System;
using System.Threading.Tasks;
using Assistance.Event.Dal.Repositories;
using Assistance.Operational.Bll.Impl.Errors;
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
    public class SignActivityServiceTest : TestBase
    {
        public SignActivityServiceTest() : base()
        {
        }

        //[Theory]
        //[InlineData("1337", ActivityStatusEnum.CustomerSigned)]
        //[InlineData("1338", ActivityStatusEnum.Generated)]
        //[InlineData("1339", ActivityStatusEnum.ConsultantSigned)]
        //public async Task SignActivityReport_NominalCase_ExpectUpdatedActivityReport(string userId, ActivityStatusEnum status)
        //{
        //    // Arrange
        //    var activity = new Fixture().Create<ActivityDto>();
        //    activity.Commercial = new UserActivityDto() { Id = "1337" };
        //    activity.Consultant = new UserActivityDto() { Id = "1338" };
        //    activity.Customer = new UserActivityDto() { Id = "1339" };

        //    var user = new Fixture().Create<User>();
        //    user.Id = userId;

        //    var activityRepoMock = new Mock<IActivityRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var missionRepoMock = new Mock<IMissionApi>();
        //    var orgaRepoMock = new Mock<IOrganizationApi>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var notifRepoMock = new Mock<INotificationRepository>();
        //    var configurationMock = new Mock<IConfiguration>();

        //    userRepoMock
        //        .Setup(x => x.ApiUsersByIdGetAsync(user.Id, It.IsAny<string>()))
        //        .ReturnsAsync(user);

        //    activityRepoMock
        //        .Setup(x => x.ApiActivitiesByIdGetAsync(activity.Id, It.IsAny<string>()))
        //        .ReturnsAsync(_mapper.Map<ActivityDto>(activity));

        //    activity.Commercial.SignatureDate = DateTime.UtcNow;
        //    activity.Customer.SignatureDate = DateTime.UtcNow;
        //    activity.Consultant.SignatureDate = DateTime.UtcNow;
        //    activity.Status = status;

        //    activityRepoMock
        //        .Setup(x => x.ApiActivitiesPatchAsync(It.IsAny<ActivityDto>(), activity.Id, It.IsAny<string>()))
        //        .ReturnsAsync(_mapper.Map<ActivityDto>(activity));

        //    var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object, orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object);

        //    // Act
        //    ActivityDto updatedActivityReport = await service.SignActivityReport(activity.Id, user.Id);

        //    // Assert
        //    updatedActivityReport.Should().NotBeNull();

        //    switch (user.Id)
        //    {
        //        case "1337":
        //            updatedActivityReport.Commercial.SignatureDate.Should().BeCloseTo(DateTime.UtcNow, 2000);
        //            updatedActivityReport.Status.Should().Be(ActivityStatusEnum.CustomerSigned);
        //            break;
        //        case "1338":
        //            updatedActivityReport.Consultant.SignatureDate.Should().BeCloseTo(DateTime.UtcNow, 2000);
        //            updatedActivityReport.Status.Should().Be(ActivityStatusEnum.ConsultantSigned);
        //            break;
        //        case "1339":
        //            updatedActivityReport.Customer.SignatureDate.Should().BeCloseTo(DateTime.UtcNow, 2000);
        //            updatedActivityReport.Status.Should().Be(ActivityStatusEnum.CustomerSigned);
        //            break;
        //    }
        //}

        [Fact]
        public async Task SignActivityReportAsConsultant_NominalCase_ExpectUpdatedActivityReport()
        {
            // Arrange
            var user = new Fixture().Create<UserDto>();
            var mission = new Fixture().Create<MissionDto>();
            var activity = new Fixture().Create<ActivityDto>();

            activity.MissionId = mission.Id;
            activity.Status = ActivityStatusEnum.Generated;
            activity.Consultant = new UserActivityDto()
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                ProfilePicUrl = user.ProfilePicUrl,
                SignatureDate = DateTime.UtcNow,
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

            azureStorageRepoMock.Setup(x => x.UploadAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Uri("http://hellotrine.com/test.jpg"));

            userRepoMock
                .Setup(x => x.ApiUsersByIdGetAsync(activity.Consultant.Id, It.IsAny<string>()))
                .ReturnsAsync(user);
            activityRepoMock
                .Setup(x => x.ApiActivitiesByIdGetAsync(activity.Id, It.IsAny<string>()))
                .ReturnsAsync(_mapper.Map<ActivityDto>(activity));
            missionRepoMock
                .Setup(x => x.ApiMissionsByIdGetAsync(activity.MissionId, It.IsAny<string>()))
                .ReturnsAsync(mission);

            // Etat attendu de l'activity APRES signature
            ActivityDto expectedUpdatedActivityReport = activity;
            expectedUpdatedActivityReport.Commercial.SignatureDate = null;
            expectedUpdatedActivityReport.Customer.SignatureDate = null;
            expectedUpdatedActivityReport.Consultant.SignatureDate = DateTime.UtcNow;
            expectedUpdatedActivityReport.Status = ActivityStatusEnum.Generated;
            activityRepoMock
                .Setup(x => x.ApiActivitiesByIdPatchAsync(activity.Id, It.IsAny<ActivityDto>(), It.IsAny<string>()))
                .ReturnsAsync(_mapper.Map<ActivityDto>(expectedUpdatedActivityReport));

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object,
                missionRepoMock.Object, orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object,
                azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            ActivityDto updatedActivityReport = await service.SignActivityReport(activity.Id, user.Id, null);

            // Assert
            updatedActivityReport.Should().NotBeNull();
            updatedActivityReport.Status.Should().Be(ActivityStatusEnum.ConsultantSigned);
            updatedActivityReport.Consultant.SignatureDate.Should().BeCloseTo(DateTime.UtcNow, 1000);
        }

        [Fact]
        public async Task SignActivityReportAsCustomer_NominalCase_ExpectUpdatedActivityReport()
        {
            // Arrange
            var activity = new Fixture().Create<ActivityDto>();
            activity.Status = ActivityStatusEnum.ConsultantSigned;
            var user = new Fixture().Create<UserDto>();
            activity.Customer = new UserActivityDto()
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                ProfilePicUrl = user.ProfilePicUrl,
                SignatureDate = DateTime.UtcNow
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

            azureStorageRepoMock.Setup(x => x.UploadAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Uri("http://hellotrine.com/test.jpg"));

            userRepoMock
                .Setup(x => x.ApiUsersByIdGetAsync(activity.Customer.Id, It.IsAny<string>()))
                .ReturnsAsync(user);
            activityRepoMock
                .Setup(x => x.ApiActivitiesByIdGetAsync(activity.Id, It.IsAny<string>()))
                .ReturnsAsync(_mapper.Map<ActivityDto>(activity));

            // Etat attendu de l'activity APRES signature
            activity.Commercial.SignatureDate = null;
            activity.Customer.SignatureDate = DateTime.UtcNow;
            activity.Consultant.SignatureDate = DateTime.UtcNow;
            activityRepoMock
                .Setup(x => x.ApiActivitiesByIdPatchAsync(activity.Id, It.IsAny<ActivityDto>(), It.IsAny<string>()))
                .ReturnsAsync(_mapper.Map<ActivityDto>(activity));

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
                orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            ActivityDto updatedActivityReport = await service.SignActivityReport(activity.Id, user.Id, null);

            // Assert
            updatedActivityReport.Should().NotBeNull();
            updatedActivityReport.Status.Should().Be(ActivityStatusEnum.CustomerSigned);
            updatedActivityReport.Customer.SignatureDate.Should().BeCloseTo(DateTime.UtcNow, 1000);
        }

        [Theory]
        [InlineData("1337")]
        [InlineData("1339")]
        public async Task SignActivityReportAsCommercialOrCustomer_WhenConsultantDidntSign_ExpectBusinessException(string userId)
        {
            // Arrange
            var activity = new Fixture().Create<ActivityDto>();

            activity.Commercial = new UserActivityDto() { Id = "1337" };
            activity.Customer = new UserActivityDto() { Id = "1339" };
            activity.Consultant.SignatureDate = null;

            var user = new Fixture().Create<UserDto>();
            user.Id = userId;

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

            userRepoMock
                .Setup(x => x.ApiUsersByIdGetAsync(user.Id, It.IsAny<string>()))
                .ReturnsAsync(user);
            activityRepoMock
                .Setup(x => x.ApiActivitiesByIdGetAsync(activity.Id, It.IsAny<string>()))
                .ReturnsAsync(_mapper.Map<ActivityDto>(activity));

            activity.Commercial.SignatureDate = null;
            activity.Customer.SignatureDate = null;
            activity.Consultant.SignatureDate = null;
            activityRepoMock
                .Setup(x => x.ApiActivitiesByIdPatchAsync(activity.Id, It.IsAny<ActivityDto>(), It.IsAny<string>()))
                .ReturnsAsync(_mapper.Map<ActivityDto>(activity));

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object,
                missionRepoMock.Object, orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.SignActivityReport(activity.Id, user.Id, null));

            // Assert
            exc.Message.Should().Be(ErrorMessages.userCannotSignYet);
        }

        [Fact]
        public async Task SignActivity_WhenNotInvolvedInTheMission_ExpectBusinessException()
        {
            // Arrange
            var activity = new Fixture().Create<ActivityDto>();
            activity.Commercial = new UserActivityDto() { Id = "1337" };
            activity.Consultant = new UserActivityDto() { Id = "1338" };
            activity.Customer = new UserActivityDto() { Id = "1339" };

            var user = new Fixture().Create<UserDto>();
            user.Id = "toto";

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

            userRepoMock
                .Setup(x => x.ApiUsersByIdGetAsync(user.Id, It.IsAny<string>()))
                .ReturnsAsync(user);
            activityRepoMock
                .Setup(x => x.ApiActivitiesByIdGetAsync(activity.Id, It.IsAny<string>()))
                .ReturnsAsync(_mapper.Map<ActivityDto>(activity));

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
                orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.SignActivityReport(activity.Id, user.Id, null));

            // Assert
            exc.Message.Should().Be(ErrorMessages.userCannotSignYet);
        }

        [Fact]
        public async Task SignActivity_WhenUserDoesntExist_ExpectBusinessException()
        {
            // Arrange
            var activity = new Fixture().Create<ActivityDto>();
            UserDto user = null;

            var activityRepoMock = new Mock<IActivityRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var missionRepoMock = new Mock<IMissionRepository>();
            var eventRepoMock = new Mock<IEventRepository>();
            var orgaRepoMock = new Mock<IOrganizationRepository>();
            var mailRepoMock = new Mock<IMailRepository>();
            var notifRepoMock = new Mock<INotificationRepository>();
            var configurationMock = new Mock<IConfiguration>();
            var azureStorageRepoMock = new Mock<IAzureStorageRepository>();
            var internalNotificationRepository = new Mock<IInternalNotificationRepository>();

            //eventRepoMock
            //    .Verify(x => x.ApiEventsPostAsync(It.IsAny<ModelEventDto>(), It.IsAny<string>()));
            userRepoMock
                .Setup(x => x.ApiUsersByIdGetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(value: user);
            activityRepoMock
                .Setup(x => x.ApiActivitiesByIdGetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(_mapper.Map<ActivityDto>(activity));

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
                orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.SignActivityReport(It.IsAny<string>(), It.IsAny<string>(), null));

            // Assert
            exc.Message.Should().Be(ErrorMessages.userDoesntExist);
        }

        [Fact]
        public async Task SignActivity_WhenActivityDoesntExist_ExpectBusinessException()
        {
            // Arrange
            ActivityDto activity = null;
            UserDto user = new Fixture().Create<UserDto>();

            var activityRepoMock = new Mock<IActivityRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var missionRepoMock = new Mock<IMissionRepository>();
            var eventRepoMock = new Mock<IEventRepository>();
            var orgaRepoMock = new Mock<IOrganizationRepository>();
            var mailRepoMock = new Mock<IMailRepository>();
            var notifRepoMock = new Mock<INotificationRepository>();
            var configurationMock = new Mock<IConfiguration>();
            var azureStorageRepoMock = new Mock<IAzureStorageRepository>();
            var internalNotificationRepository = new Mock<IInternalNotificationRepository>();

            //eventRepoMock
            //    .Verify(x => x.ApiEventsPostAsync(It.IsAny<ModelEventDto>(), It.IsAny<string>()));
            userRepoMock
                .Setup(x => x.ApiUsersByIdGetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(value: user);
            activityRepoMock
                .Setup(x => x.ApiActivitiesByIdGetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(value: activity);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
                orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.SignActivityReport(It.IsAny<string>(), It.IsAny<string>(), null));

            // Assert
            exc.Message.Should().Be(ErrorMessages.activityDoesntExist);
        }
    }
}
