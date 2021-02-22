using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Event.Dal.Repositories;
using Assistance.Operational.Bll.Impl.Errors;
using Assistance.Operational.Bll.Impl.Services;
using Assistance.Operational.Bll.Impl.Text;
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
    public class RefuseModificationProposalTests : TestBase
    {
        [Fact]
        public async Task RefuseWithModificationProposal_NominalCase()
        {
            // Arrange
            var user = new Fixture().Create<UserDto>();
            var modifs = new Fixture().Create<List<GridDayDto>>();
            var comment = new Fixture().Create<string>();
            var mission = new Fixture().Create<MissionDto>();
            var activity = new Fixture().Create<ActivityDto>();
            activity.Status = ActivityStatusEnum.ConsultantSigned;
            activity.MissionId = mission.Id;
            activity.Customer = new UserActivityDto()
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                ProfilePicUrl = user.ProfilePicUrl,
                SignatureDate = DateTime.UtcNow
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
                .Setup(x => x.ApiUsersByIdGetAsync(user.Id, It.IsAny<string>()))
                .ReturnsAsync(user);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object,
                missionRepoMock.Object, orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object,
                azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            await service.RefuseWithModificationProposal(activity.Id, user.Id, modifs);

            // Assert
            var expectedEventTitle = string.Format(UserMessages.refusedActivityModificationsEventTitle, user.Firstname, user.Lastname, activity.StartDate.Month, activity.EndDate.Year);
            eventRepoMock.Verify(x => x.ApiEventsPostAsync(It.Is<EventDto>(y => y.Title == expectedEventTitle), null));
        }

        [Fact]
        public async Task RefuseWithModificationProposal_WhenModificationsAreNull_NominalCase()
        {
            // Arrange
            var user = new Fixture().Create<UserDto>();
            var modifs = new Fixture().Create<List<GridDayDto>>();
            var comment = new Fixture().Create<string>();
            var mission = new Fixture().Create<MissionDto>();
            var activity = new Fixture().Create<ActivityDto>();
            activity.Status = ActivityStatusEnum.ConsultantSigned;
            activity.MissionId = mission.Id;
            activity.Customer = new UserActivityDto()
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                ProfilePicUrl = user.ProfilePicUrl,
                SignatureDate = DateTime.UtcNow
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
                .Setup(x => x.ApiUsersByIdGetAsync(user.Id, It.IsAny<string>()))
                .ReturnsAsync(user);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object,
                missionRepoMock.Object, orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object,
                azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            await service.RefuseWithModificationProposal(activity.Id, user.Id, modifs);

            // Assert
            var expectedEventTitle = string.Format(UserMessages.refusedActivityModificationsEventTitle, user.Firstname, user.Lastname, activity.StartDate.Month, activity.EndDate.Year);
            eventRepoMock.Verify(x => x.ApiEventsPostAsync(It.Is<EventDto>(y => y.Title == expectedEventTitle), null));
        }

        [Fact]
        public async Task RefuseWithModificationProposal_WhenActivityIsNull_ExpectBusinessException()
        {
            // Arrange
            var user = new Fixture().Create<UserDto>();
            var mission = new Fixture().Create<MissionDto>();
            var modifs = new Fixture().Create<List<GridDayDto>>();
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
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.RefuseWithModificationProposal(activity.Id, user.Id, modifs));

            // Assert
            exc.Message.Should().Be(ErrorMessages.activityDoesntExist);
        }

        [Fact]
        public async Task RefuseWithModificationProposal_WhenRefusingUserIsNull_ExpectBusinessException()
        {
            // Arrange
            var user = new Fixture().Create<UserDto>();
            var mission = new Fixture().Create<MissionDto>();
            var modifs = new Fixture().Create<List<GridDayDto>>();
            var activity = new Fixture().Create<ActivityDto>();
            activity.Status = ActivityStatusEnum.ConsultantSigned;
            activity.MissionId = mission.Id;
            activity.Customer = new UserActivityDto()
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                ProfilePicUrl = user.ProfilePicUrl,
                SignatureDate = DateTime.UtcNow
            };
            var modifProposal = activity.ModificationProposals.Last();
            modifProposal.Modifications.FirstOrDefault().Day = new DateTime(2019, 03, 14);
            activity.Days.Last().Day = new DateTime(2019, 03, 14);


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
                .ReturnsAsync(value: null);


            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object,
                missionRepoMock.Object, orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object,
                azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.RefuseWithModificationProposal(activity.Id, user.Id, modifs));

            // Assert
            exc.Message.Should().Be(ErrorMessages.userCannotBeNull);
        }

        [Fact]
        public async Task RefuseWithModificationProposal_WhenActivityIsNotCSignedByConsultant_ExpectBusinessException()
        {
            // Arrange
            var user = new Fixture().Create<UserDto>();
            var mission = new Fixture().Create<MissionDto>();
            var modifs = new Fixture().Create<List<GridDayDto>>();
            var activity = new Fixture().Create<ActivityDto>();
            activity.Status = ActivityStatusEnum.Generated;
            activity.MissionId = mission.Id;
            activity.Commercial = new UserActivityDto()
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
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.RefuseWithModificationProposal(activity.Id, user.Id, modifs));

            // Assert
            exc.Message.Should().Be(ErrorMessages.consultantMustHaveSignedActivity);
        }

        [Fact]
        public async Task RefuseWithModificationProposal_WhenRefusedConsultant_ExpectBusinessException()
        {
            // Arrange
            var user = new Fixture().Create<UserDto>();
            var mission = new Fixture().Create<MissionDto>();
            var modifs = new Fixture().Create<List<GridDayDto>>();
            var activity = new Fixture().Create<ActivityDto>();
            activity.Status = ActivityStatusEnum.Generated;
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
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.RefuseWithModificationProposal(activity.Id, user.Id, modifs));

            // Assert
            exc.Message.Should().Be(ErrorMessages.onlyCustomerAndCommercialCanRequireActivityModification);
        }
    }
}
