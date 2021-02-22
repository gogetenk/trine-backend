using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Event.Dal.Repositories;
using Assistance.Operational.Bll.Impl.Services;
using Assistance.Operational.Dal.Repositories;
using Assistance.Operational.Dto;
using AutoFixture;
using Dto;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Assistance.Operational.Bll.Tests.Services.ActivityServiceTests
{
    public class GetActivityTest : TestBase
    {
        public GetActivityTest() : base()
        {
        }

        //[Fact]
        //public async Task GetFromMission_NominalCase_ExpectActivities()
        //{
        //    // Arrange
        //    string missionId = "toto";
        //    string userId = "123";
        //    var activityList = new Fixture().CreateMany<ActivityDto>().ToList();
        //    activityList.ForEach(x => x.MissionId = missionId);

        //    var activityRepoMock = new Mock<IActivityRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var missionRepoMock = new Mock<IMissionRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var notifRepoMock = new Mock<INotificationRepository>();
        //    var configurationMock = new Mock<IConfiguration>();
        //    var azureStorageRepoMock = new Mock<IAzureStorageRepository>();
        //    var internalNotificationRepository = new Mock<IInternalNotificationRepository>();

        //    //eventRepoMock
        //    //    .Verify(x => x.ApiEventsPostAsync(It.IsAny<ModelEventDto>(), It.IsAny<string>()));
        //    activityRepoMock
        //        .Setup(x => x.ApiActivitiesMissionsByMissionIdGetAsync(missionId, It.IsAny<string>()))
        //        .ReturnsAsync(_mapper.Map<List<ActivityDto>>(activityList));

        //    var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
        //        orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

        //    // Act
        //    var activities = await service.GetFromMission(userId, missionId);

        //    // Assert
        //    activities.Should().NotBeNull();
        //    activities.Count.Should().Be(activityList.Count);
        //}

        [Fact]
        public async Task GetFromUser_NominalCase_ExpectActivities()
        {
            // Arrange
            string userId = "toto";
            var activityList = new Fixture().CreateMany<ActivityDto>().ToList();
            activityList.ForEach(x => x.Commercial.Id = userId);

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

            //eventRepoMock
            //    .Verify(x => x.ApiEventsPostAsync(It.IsAny<ModelEventDto>(), It.IsAny<string>()));
            activityRepoMock
                .Setup(x => x.ApiActivitiesUsersByUserIdGetAsync(userId, It.IsAny<string>()))
                .ReturnsAsync(_mapper.Map<List<ActivityDto>>(activityList));

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object,
                missionRepoMock.Object, orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var activities = await service.GetFromUser(userId);

            // Assert
            activities.Should().NotBeNull();
            activities.Count.Should().Be(activityList.Count);
            activities.Should().BeInDescendingOrder(x => x.StartDate);
        }

        [Fact]
        public async Task DeleteActivity_NominalCase_ExpectOk()
        {
            // Arrange
            string userId = "toto";
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
            //eventRepoMock
            //    .Verify(x => x.ApiEventsPostAsync(It.IsAny<ModelEventDto>(), It.IsAny<string>()));
            activityRepoMock
                .Setup(x => x.ApiActivitiesByIdDeleteAsync(userId, It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
                orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            await service.DeleteActivity(userId);
        }

        [Fact]
        public async Task GenerateGrid_NominalCase_ExpectOk()
        {
            // Arrange
            var grid = new Fixture().Create<ActivityDto>();

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
            //eventRepoMock
            //    .Verify(x => x.ApiEventsPostAsync(It.IsAny<ModelEventDto>(), It.IsAny<string>()));
            activityRepoMock
                .Setup(x => x.ApiActivitiesByStartDateendDateGetAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync(_mapper.Map<ActivityDto>(grid));

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
                orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);

            // Act
            var generatedGrid = await service.Generate();
            generatedGrid.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_NominalCase_ExpectOk()
        {
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

            var user = new Fixture().Create<UserDto>();
            userRepoMock.Setup(m => m.ApiUsersByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(user);

            var activity = new Fixture().Create<ActivityDto>();
            activityRepoMock.Setup(m => m.ApiActivitiesByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(activity);

            var mission = new Fixture().Create<MissionDto>();
            mission.Id = activity.MissionId;
            missionRepoMock.Setup(m => m.ApiMissionsByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(mission);

            var organization = new Fixture().Create<OrganizationDto>();
            organization.Id = mission.OrganizationId;
            orgaRepoMock.Setup(o => o.ApiOrganizationsByIdGetAsync(mission.OrganizationId, It.IsAny<string>())).ReturnsAsync(organization);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
                orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);
            var activityDto = await service.GetById(user.Id, activity.Id);

            activityDto.Should().NotBeNull();
        }

        //[Fact]
        //public async Task GetById_AfterConsultantSignature_ExpectCustomerCanModifyActivity()
        //{
        //    var activityRepoMock = new Mock<IActivityRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var missionRepoMock = new Mock<IMissionRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var notifRepoMock = new Mock<INotificationRepository>();
        //    var configurationMock = new Mock<IConfiguration>();
        //    var azureStorageRepoMock = new Mock<IAzureStorageRepository>();
        //    var internalNotificationRepository = new Mock<IInternalNotificationRepository>();

        //    var customer = new Fixture().Create<UserDto>();
        //    userRepoMock.Setup(m => m.ApiUsersByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(customer);

        //    var consultant = new Fixture().Create<UserDto>();
        //    var activityDto = new Fixture().Create<ActivityDto>();
        //    activityDto.Status = ActivityStatusEnum.ConsultantSigned;
        //    activityDto.Consultant = new UserActivityDto() { Id = consultant.Id, Firstname = consultant.Firstname, Lastname = consultant.Lastname, ProfilePicUrl = consultant.ProfilePicUrl, SignatureDate = new DateTime(2019, 1, 1) };
        //    var activity = _mapper.Map<ActivityDto>(activityDto);
        //    activityRepoMock.Setup(m => m.ApiActivitiesByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(activity);

        //    var mission = new Fixture().Create<MissionDto>();
        //    mission.Id = activity.MissionId;
        //    missionRepoMock.Setup(m => m.ApiMissionsByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(mission);

        //    var organization = new Fixture().Create<OrganizationDto>();
        //    organization.Id = mission.OrganizationId;
        //    orgaRepoMock.Setup(o => o.ApiOrganizationsByIdGetAsync(mission.OrganizationId, It.IsAny<string>())).ReturnsAsync(organization);

        //    var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object,
        //        missionRepoMock.Object, orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);
        //    var result = await service.GetById(customer.Id, activity.Id);

        //    result.CanModify.Should().Equals(true);
        //}

        [Fact]
        public async Task GetById_AfterActivityCreation_ExpectConsultantCanModifyAndSignActivity()
        {
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

            var customer = new Fixture().Create<UserDto>();
            userRepoMock.Setup(m => m.ApiUsersByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(customer);

            var consultant = new Fixture().Create<UserDto>();
            var activityDto = new Fixture().Create<ActivityDto>();
            activityDto.Status = ActivityStatusEnum.Generated;
            activityDto.Consultant = new UserActivityDto() { Id = consultant.Id, Firstname = consultant.Firstname, Lastname = consultant.Lastname, ProfilePicUrl = consultant.ProfilePicUrl, SignatureDate = null };
            activityDto.Customer = new UserActivityDto() { Id = customer.Id, Firstname = customer.Firstname, Lastname = customer.Lastname, ProfilePicUrl = customer.ProfilePicUrl, SignatureDate = null };

            var activity = _mapper.Map<ActivityDto>(activityDto);
            activityRepoMock.Setup(m => m.ApiActivitiesByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(activity);

            var mission = new Fixture().Create<MissionDto>();
            mission.Id = activity.MissionId;
            missionRepoMock.Setup(m => m.ApiMissionsByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(mission);

            var organization = new Fixture().Create<OrganizationDto>();
            organization.Id = mission.OrganizationId;
            orgaRepoMock.Setup(o => o.ApiOrganizationsByIdGetAsync(mission.OrganizationId, It.IsAny<string>())).ReturnsAsync(organization);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
                orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);
            var result = await service.GetById(consultant.Id, activity.Id);

            result.Customer.CanSign.Should().Equals(true);
        }

        [Fact]
        public async Task GetById_AfterActivityCreation_ExpectCustomerCannotModifyAndSignActivity()
        {
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
            var customer = new Fixture().Create<UserDto>();
            userRepoMock.Setup(m => m.ApiUsersByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(customer);

            var consultant = new Fixture().Create<UserDto>();
            var activityDto = new Fixture().Create<ActivityDto>();
            activityDto.Status = ActivityStatusEnum.Generated;
            activityDto.Consultant = new UserActivityDto() { Id = consultant.Id, Firstname = consultant.Firstname, Lastname = consultant.Lastname, ProfilePicUrl = consultant.ProfilePicUrl, SignatureDate = null };
            activityDto.Customer = new UserActivityDto() { Id = customer.Id, Firstname = customer.Firstname, Lastname = customer.Lastname, ProfilePicUrl = customer.ProfilePicUrl, SignatureDate = null };

            var activity = _mapper.Map<ActivityDto>(activityDto);
            activityRepoMock.Setup(m => m.ApiActivitiesByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(activity);

            var mission = new Fixture().Create<MissionDto>();
            mission.Id = activity.MissionId;
            missionRepoMock.Setup(m => m.ApiMissionsByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(mission);

            var organization = new Fixture().Create<OrganizationDto>();
            organization.Id = mission.OrganizationId;
            orgaRepoMock.Setup(o => o.ApiOrganizationsByIdGetAsync(mission.OrganizationId, It.IsAny<string>())).ReturnsAsync(organization);

            var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object,
                missionRepoMock.Object, orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);
            var result = await service.GetById(customer.Id, activity.Id);

            result.Customer.CanSign.Should().Equals(false);
        }

        //[Fact]
        //public async Task GetById_AfterClientSignature_ExpectOrganizationAdministratorCanModifyActivity()
        //{
        //    var activityRepoMock = new Mock<IActivityRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var missionRepoMock = new Mock<IMissionRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var notifRepoMock = new Mock<INotificationRepository>();
        //    var configurationMock = new Mock<IConfiguration>();
        //    var azureStorageRepoMock = new Mock<IAzureStorageRepository>();
        //    var internalNotificationRepository = new Mock<IInternalNotificationRepository>();

        //    var currentUser = new Fixture().Create<UserDto>();
        //    var customer = new Fixture().Create<UserDto>();
        //    userRepoMock.Setup(m => m.ApiUsersByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(customer);

        //    var consultant = new Fixture().Create<UserDto>();
        //    var activityDto = new Fixture().Create<ActivityDto>();
        //    activityDto.Status = ActivityStatusEnum.CustomerSigned;
        //    activityDto.Consultant = new UserActivityDto() { Id = consultant.Id, Firstname = consultant.Firstname, Lastname = consultant.Lastname, ProfilePicUrl = consultant.ProfilePicUrl, SignatureDate = new DateTime(2019, 1, 1) };
        //    activityDto.Customer = new UserActivityDto() { Id = customer.Id, Firstname = customer.Firstname, Lastname = customer.Lastname, ProfilePicUrl = customer.ProfilePicUrl, SignatureDate = new DateTime(2019, 1, 1) };

        //    var activity = _mapper.Map<ActivityDto>(activityDto);
        //    activityRepoMock.Setup(m => m.ApiActivitiesByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(activity);

        //    var mission = new Fixture().Create<MissionDto>();
        //    mission.Id = activity.MissionId;
        //    missionRepoMock.Setup(m => m.ApiMissionsByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(mission);

        //    var organization = new Fixture().Create<OrganizationDto>();
        //    organization.Id = mission.OrganizationId;

        //    var organizationMemberDto = _mapper.Map<OrganizationMemberDto>(new OrganizationMemberDto()
        //    {
        //        Firstname = currentUser.Firstname,
        //        Icon = "",
        //        JoinedAt = DateTime.Now.AddMonths(-2),
        //        Lastname = currentUser.Lastname,
        //        Role = OrganizationMemberDto.RoleEnum.ADMIN,
        //        UserId = currentUser.Id
        //    });

        //    organization.Members.Add(organizationMemberDto);
        //    orgaRepoMock.Setup(o => o.ApiOrganizationsByIdGetAsync(mission.OrganizationId, It.IsAny<string>())).ReturnsAsync(organization);

        //    var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
        //        orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);
        //    var result = await service.GetById(customer.Id, activity.Id);

        //    result.CanModify.Should().Equals(true);
        //}

        //[Fact]
        //public async Task GetById_AfterClientSignature_ExpectCustomerCannotModifyActivity()
        //{
        //    var activityRepoMock = new Mock<IActivityRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var missionRepoMock = new Mock<IMissionRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var notifRepoMock = new Mock<INotificationRepository>();
        //    var configurationMock = new Mock<IConfiguration>();
        //    var azureStorageRepoMock = new Mock<IAzureStorageRepository>();
        //    var internalNotificationRepository = new Mock<IInternalNotificationRepository>();

        //    var customer = new Fixture().Create<UserDto>();
        //    userRepoMock.Setup(m => m.ApiUsersByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(customer);

        //    var consultant = new Fixture().Create<UserDto>();
        //    var activityDto = new Fixture().Create<ActivityDto>();
        //    activityDto.Status = ActivityStatusEnum.CustomerSigned;
        //    activityDto.Consultant = new UserActivityDto() { Id = consultant.Id, Firstname = consultant.Firstname, Lastname = consultant.Lastname, ProfilePicUrl = consultant.ProfilePicUrl, SignatureDate = new DateTime(2019, 1, 1) };
        //    activityDto.Customer = new UserActivityDto() { Id = customer.Id, Firstname = customer.Firstname, Lastname = customer.Lastname, ProfilePicUrl = customer.ProfilePicUrl, SignatureDate = new DateTime(2019, 1, 1) };

        //    var activity = _mapper.Map<ActivityDto>(activityDto);
        //    activityRepoMock.Setup(m => m.ApiActivitiesByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(activity);

        //    var mission = new Fixture().Create<MissionDto>();
        //    mission.Id = activity.MissionId;
        //    missionRepoMock.Setup(m => m.ApiMissionsByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(mission);

        //    var organization = new Fixture().Create<OrganizationDto>();
        //    organization.Id = mission.OrganizationId;
        //    orgaRepoMock.Setup(o => o.ApiOrganizationsByIdGetAsync(mission.OrganizationId, It.IsAny<string>())).ReturnsAsync(organization);

        //    var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
        //        orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);
        //    var result = await service.GetById(customer.Id, activity.Id);

        //    result.CanModify.Should().Equals(false);
        //}

        //[Fact]
        //public async Task GetById_AfterClientSignature_ExpectConsultantCannotModifyActivity()
        //{
        //    var activityRepoMock = new Mock<IActivityRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var missionRepoMock = new Mock<IMissionRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var notifRepoMock = new Mock<INotificationRepository>();
        //    var configurationMock = new Mock<IConfiguration>();
        //    var azureStorageRepoMock = new Mock<IAzureStorageRepository>();
        //    var internalNotificationRepository = new Mock<IInternalNotificationRepository>();

        //    var customer = new Fixture().Create<UserDto>();
        //    userRepoMock.Setup(m => m.ApiUsersByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(customer);

        //    var consultant = new Fixture().Create<UserDto>();
        //    var activityDto = new Fixture().Create<ActivityDto>();
        //    activityDto.Status = ActivityStatusEnum.CustomerSigned;
        //    activityDto.Consultant = new UserActivityDto() { Id = consultant.Id, Firstname = consultant.Firstname, Lastname = consultant.Lastname, ProfilePicUrl = consultant.ProfilePicUrl, SignatureDate = new DateTime(2019, 1, 1) };
        //    activityDto.Customer = new UserActivityDto() { Id = customer.Id, Firstname = customer.Firstname, Lastname = customer.Lastname, ProfilePicUrl = customer.ProfilePicUrl, SignatureDate = new DateTime(2019, 1, 1) };

        //    var activity = _mapper.Map<ActivityDto>(activityDto);
        //    activityRepoMock.Setup(m => m.ApiActivitiesByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(activity);

        //    var mission = new Fixture().Create<MissionDto>();
        //    mission.Id = activity.MissionId;
        //    missionRepoMock.Setup(m => m.ApiMissionsByIdGetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(mission);

        //    var organization = new Fixture().Create<OrganizationDto>();
        //    organization.Id = mission.OrganizationId;
        //    orgaRepoMock.Setup(o => o.ApiOrganizationsByIdGetAsync(mission.OrganizationId, It.IsAny<string>())).ReturnsAsync(organization);

        //    var service = new ActivityService(_loggerMock.Object, _mapper, activityRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, missionRepoMock.Object,
        //        orgaRepoMock.Object, mailRepoMock.Object, notifRepoMock.Object, configurationMock.Object, azureStorageRepoMock.Object, internalNotificationRepository.Object);
        //    var result = await service.GetById(customer.Id, activity.Id);

        //    result.CanModify.Should().Equals(false);
        //}
    }
}
