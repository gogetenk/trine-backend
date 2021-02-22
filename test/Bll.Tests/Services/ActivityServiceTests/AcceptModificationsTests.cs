using System;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Event.Dal.Repositories;
using Assistance.Operational.Bll.Impl.Errors;
using Assistance.Operational.Bll.Impl.Services;
using Assistance.Operational.Bll.Impl.Text;
using Assistance.Operational.Dal.Repositories;
using Assistance.Operational.Model;
using AutoFixture;
using Dto;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Sogetrel.Sinapse.Framework.Exceptions;
using Xunit;

namespace Assistance.Operational.Bll.Tests.Services.ActivityServiceTests
{
    public class AcceptModificationsTests : TestBase
    {
        [Fact]
        public async Task AcceptLastModificationProposal_NominalCase()
        {
            // Arrange
            var user = new Fixture().Create<UserDto>();
            var mission = new Fixture().Create<MissionDto>();
            var activity = new Fixture().Create<ActivityDto>();
            activity.Status = ActivityStatusEnum.ModificationsRequired;
            activity.MissionId = mission.Id;
            activity.Consultant = new UserActivityDto()
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                ProfilePicUrl = user.ProfilePicUrl,
                SignatureDate = DateTime.UtcNow,
            };
            var modifProposal = activity.ModificationProposals.Last();
            modifProposal.Modifications.FirstOrDefault().Day = new DateTime(2019, 03, 14);
            activity.Days.Last().Day = new DateTime(2019, 03, 14);
            var modifAuthor = new Fixture().Create<UserDto>();
            modifAuthor.Id = modifProposal.UserId;

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
                .Setup(x => x.ApiActivitiesByIdGetAsync(activity.Id, It.IsAny<string>()))
                .ReturnsAsync(_mapper.Map<ActivityDto>(activity));
            userRepoMock
                .Setup(x => x.ApiUsersByIdGetAsync(activity.Consultant.Id, It.IsAny<string>()))
                .ReturnsAsync(user);
            userRepoMock
                .Setup(x => x.ApiUsersByIdGetAsync(modifAuthor.Id, It.IsAny<string>()))
                .ReturnsAsync(modifAuthor);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object,
                missionRepoMock.Object, orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object,
                azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            await service.AcceptLastModificationProposal(activity.Id, user.Id);

            // Assert
            mailRepoMock.Verify(x => x.SendAsync(modifAuthor.Email, It.IsAny<string>(), It.IsAny<object>(), "", "", It.IsAny<MailAttachment>()));
            var expectedEventTitle = string.Format(UserMessages.modifiedActivityEventTitle, activity.StartDate.Month, activity.StartDate.Year);
            var expectedEventSubtitle = string.Format(UserMessages.modifiedActivityEventSubtitle, user.Firstname, user.Lastname, modifAuthor.Firstname, modifAuthor.Lastname);
            eventRepoMock.Verify(x => x.ApiEventsPostAsync(It.Is<EventDto>(y => y.Title == expectedEventTitle && y.Subtitle == expectedEventSubtitle), null));
        }

        [Fact]
        public async Task AcceptLastModificationProposal_WhenNoActivity_ExpectBusinessException()
        {
            // Arrange
            var user = new Fixture().Create<UserDto>();
            var mission = new Fixture().Create<MissionDto>();
            var activity = new Fixture().Create<ActivityDto>();
            activity.Status = ActivityStatusEnum.ModificationsRequired;
            activity.MissionId = mission.Id;
            activity.Consultant = new UserActivityDto()
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                ProfilePicUrl = user.ProfilePicUrl,
                SignatureDate = DateTime.UtcNow,
            };
            var modifProposal = activity.ModificationProposals.Last();
            modifProposal.Modifications.FirstOrDefault().Day = new DateTime(2019, 03, 14);
            activity.Days.Last().Day = new DateTime(2019, 03, 14);
            var modifAuthor = new Fixture().Create<UserDto>();
            modifAuthor.Id = modifProposal.UserId;

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
                .Setup(x => x.ApiActivitiesByIdGetAsync(activity.Id, It.IsAny<string>()))
                .ReturnsAsync(value: null);
            userRepoMock
                .Setup(x => x.ApiUsersByIdGetAsync(activity.Consultant.Id, It.IsAny<string>()))
                .ReturnsAsync(user);
            userRepoMock
                .Setup(x => x.ApiUsersByIdGetAsync(modifAuthor.Id, It.IsAny<string>()))
                .ReturnsAsync(modifAuthor);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object,
                missionRepoMock.Object, orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object,
                azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.AcceptLastModificationProposal(activity.Id, user.Id));

            // Assert
            exc.Message.Should().Be(ErrorMessages.activityDoesntExist);
        }

        [Fact]
        public async Task AcceptLastModificationProposal_WhenConsultantIsNull_ExpectBusinessException()
        {
            // Arrange
            var user = new Fixture().Create<UserDto>();
            var mission = new Fixture().Create<MissionDto>();
            var activity = new Fixture().Create<ActivityDto>();
            activity.Status = ActivityStatusEnum.ModificationsRequired;
            activity.MissionId = mission.Id;
            activity.Consultant = null;
            var modifProposal = activity.ModificationProposals.Last();
            modifProposal.Modifications.FirstOrDefault().Day = new DateTime(2019, 03, 14);
            activity.Days.Last().Day = new DateTime(2019, 03, 14);
            var modifAuthor = new Fixture().Create<UserDto>();
            modifAuthor.Id = modifProposal.UserId;

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
                .Setup(x => x.ApiActivitiesByIdGetAsync(activity.Id, It.IsAny<string>()))
                .ReturnsAsync(_mapper.Map<ActivityDto>(activity));
            userRepoMock
                .Setup(x => x.ApiUsersByIdGetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(user);
            userRepoMock
                .Setup(x => x.ApiUsersByIdGetAsync(modifAuthor.Id, It.IsAny<string>()))
                .ReturnsAsync(modifAuthor);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object,
                missionRepoMock.Object, orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object,
                azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.AcceptLastModificationProposal(activity.Id, user.Id));

            // Assert
            exc.Message.Should().Be(ErrorMessages.badUserRole);
        }

        [Fact]
        public async Task AcceptLastModificationProposal_WhenUserIsNotTheConsultant_ExpectBusinessException()
        {
            // Arrange
            var user = new Fixture().Create<UserDto>();
            var mission = new Fixture().Create<MissionDto>();
            var activity = new Fixture().Create<ActivityDto>();
            activity.Status = ActivityStatusEnum.ModificationsRequired;
            activity.MissionId = mission.Id;
            activity.Consultant = new UserActivityDto()
            {
                Id = "1337",
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                ProfilePicUrl = user.ProfilePicUrl,
                SignatureDate = DateTime.UtcNow,
            };
            var modifProposal = activity.ModificationProposals.Last();
            modifProposal.Modifications.FirstOrDefault().Day = new DateTime(2019, 03, 14);
            activity.Days.Last().Day = new DateTime(2019, 03, 14);
            var modifAuthor = new Fixture().Create<UserDto>();
            modifAuthor.Id = modifProposal.UserId;

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
                .Setup(x => x.ApiActivitiesByIdGetAsync(activity.Id, It.IsAny<string>()))
                .ReturnsAsync(_mapper.Map<ActivityDto>(activity));
            userRepoMock
                .Setup(x => x.ApiUsersByIdGetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(user);
            userRepoMock
                .Setup(x => x.ApiUsersByIdGetAsync(modifAuthor.Id, It.IsAny<string>()))
                .ReturnsAsync(modifAuthor);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object,
                missionRepoMock.Object, orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object,
                azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.AcceptLastModificationProposal(activity.Id, user.Id));

            // Assert
            exc.Message.Should().Be(ErrorMessages.badUserRole);
        }

        [Fact]
        public async Task AcceptLastModificationProposal_WhenActivityHasntModificationsRequiredStatus_ExpectBusinessException()
        {
            // Arrange
            var user = new Fixture().Create<UserDto>();
            var mission = new Fixture().Create<MissionDto>();
            var activity = new Fixture().Create<ActivityDto>();
            activity.Status = ActivityStatusEnum.Generated; // bad status
            activity.MissionId = mission.Id;
            activity.Consultant = new UserActivityDto()
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                ProfilePicUrl = user.ProfilePicUrl,
                SignatureDate = DateTime.UtcNow,
            };
            var modifProposal = activity.ModificationProposals.Last();
            modifProposal.Modifications.FirstOrDefault().Day = new DateTime(2019, 03, 14);
            activity.Days.Last().Day = new DateTime(2019, 03, 14);
            var modifAuthor = new Fixture().Create<UserDto>();
            modifAuthor.Id = modifProposal.UserId;

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
                .Setup(x => x.ApiActivitiesByIdGetAsync(activity.Id, It.IsAny<string>()))
                .ReturnsAsync(_mapper.Map<ActivityDto>(activity));
            userRepoMock
                .Setup(x => x.ApiUsersByIdGetAsync(activity.Consultant.Id, It.IsAny<string>()))
                .ReturnsAsync(user);
            userRepoMock
                .Setup(x => x.ApiUsersByIdGetAsync(modifAuthor.Id, It.IsAny<string>()))
                .ReturnsAsync(modifAuthor);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object,
                missionRepoMock.Object, orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object,
                azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.AcceptLastModificationProposal(activity.Id, user.Id));

            // Assert
            exc.Message.Should().Be(ErrorMessages.badActivityStatus);
        }

        [Fact]
        public async Task AcceptLastModificationProposal_WhenNoModificationProposals_ExpectBusinessException()
        {
            // Arrange
            var user = new Fixture().Create<UserDto>();
            var mission = new Fixture().Create<MissionDto>();
            var activity = new Fixture().Create<ActivityDto>();
            activity.Status = ActivityStatusEnum.ModificationsRequired;
            activity.MissionId = mission.Id;
            activity.Consultant = new UserActivityDto()
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                ProfilePicUrl = user.ProfilePicUrl,
                SignatureDate = DateTime.UtcNow,
            };
            activity.ModificationProposals = null;

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
                .Setup(x => x.ApiActivitiesByIdGetAsync(activity.Id, It.IsAny<string>()))
                .ReturnsAsync(_mapper.Map<ActivityDto>(activity));
            userRepoMock
                .Setup(x => x.ApiUsersByIdGetAsync(activity.Consultant.Id, It.IsAny<string>()))
                .ReturnsAsync(user);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object,
                missionRepoMock.Object, orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object,
                azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.AcceptLastModificationProposal(activity.Id, user.Id));

            // Assert
            exc.Message.Should().Be(ErrorMessages.activityModificationsNotExist);
        }

        [Fact]
        public async Task AcceptLastModificationProposal_WhenModificationAuthorIsNull_ExpectBusinessException()
        {
            // Arrange
            var user = new Fixture().Create<UserDto>();
            var mission = new Fixture().Create<MissionDto>();
            var activity = new Fixture().Create<ActivityDto>();
            activity.Status = ActivityStatusEnum.ModificationsRequired;
            activity.MissionId = mission.Id;
            activity.Consultant = new UserActivityDto()
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                ProfilePicUrl = user.ProfilePicUrl,
                SignatureDate = DateTime.UtcNow,
            };
            var modifProposal = activity.ModificationProposals.Last();
            modifProposal.Modifications.FirstOrDefault().Day = new DateTime(2019, 03, 14);
            activity.Days.Last().Day = new DateTime(2019, 03, 14);
            var modifAuthor = new Fixture().Create<UserDto>();
            modifAuthor.Id = modifProposal.UserId;

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
                .Setup(x => x.ApiActivitiesByIdGetAsync(activity.Id, It.IsAny<string>()))
                .ReturnsAsync(_mapper.Map<ActivityDto>(activity));
            userRepoMock
                .Setup(x => x.ApiUsersByIdGetAsync(activity.Consultant.Id, It.IsAny<string>()))
                .ReturnsAsync(user);
            userRepoMock
                .Setup(x => x.ApiUsersByIdGetAsync(modifAuthor.Id, It.IsAny<string>()))
                .ReturnsAsync(value: null);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object,
                missionRepoMock.Object, orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object,
                azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.AcceptLastModificationProposal(activity.Id, user.Id));

            // Assert
            exc.Message.Should().Be(ErrorMessages.authorDoesntExist);
        }
    }
}
