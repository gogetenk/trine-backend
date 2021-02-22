using System;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Event.Dal.Repositories;
using Assistance.Operational.Bll.Impl.Errors;
using Assistance.Operational.Bll.Impl.Services;
using Assistance.Operational.Dal.Repositories;
using AutoFixture;
using Dto;
using FluentAssertions;
using Moq;
using Sogetrel.Sinapse.Framework.Exceptions;
using Xunit;

namespace Assistance.Operational.Bll.Tests.Services.MissionServiceTests
{
    public class MissionServiceTest : TestBase
    {
        private const string _userApiVersion = "1.0";

        public MissionServiceTest() : base()
        {
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]

        public async Task GetMissionDataTest_NominalCase_ExpectOk(bool isActive, bool isTripartite)
        {
            //Arrange
            var user = new Fixture().Create<UserDto>();
            var users = new Fixture().CreateMany<UserDto>().ToList();
            var missions = new Fixture().CreateMany<MissionDto>().ToList();

            missions.ForEach(x =>
            {
                if (isActive)
                    x.CanceledDate = default(DateTime);
                else
                    x.CanceledDate = DateTime.UtcNow;

                x.IsTripartite = isTripartite;
            });

            var userIds = missions.Where(x => !string.IsNullOrEmpty(x.Customer.Id)).Select(x => x.Customer.Id).Distinct().ToList();
            userIds.AddRange(missions.Where(x => !string.IsNullOrEmpty(x.Commercial.Id)).Select(x => x.Commercial.Id).Distinct().ToList());
            userIds.AddRange(missions.Where(x => !string.IsNullOrEmpty(x.Consultant.Id)).Select(x => x.Consultant.Id).Distinct().ToList());
            userIds = userIds.Distinct().ToList();

            var service = CreatemissionServiceWith
             (
                missionRepoMock =>
                {
                    missionRepoMock.Setup(x => x.ApiMissionsByUserIdByActiveGetAsync(user.Id, isActive, It.IsAny<string>())).ReturnsAsync(missions);
                    return missionRepoMock;
                },
                userRepoMock =>
                {
                    userRepoMock.Setup(x => x.ApiUsersIdsGetAsync(userIds, It.IsAny<string>())).ReturnsAsync(users);
                    return userRepoMock;
                }
             );

            // Act 
            var results = await service.GetMissionsFromUser(user.Id, isActive);

            // Assert
            results.Should().NotBeNull();
        }

        [Fact]
        public async Task CancelMissionTest_NominalCase_ExpectOk()
        {
            // Arrange
            var id = new Fixture().Create<string>();
            var mission = new Fixture().Create<MissionDto>();
            mission.CanceledDate = DateTime.Now;
            mission.Status = MissionDto.StatusEnum.CONFIRMED;

            var service = CreatemissionServiceWith
            (
               missionRepoMock =>
               {
                   missionRepoMock.Setup(x => x.ApiMissionsByIdGetAsync(id, It.IsAny<string>())).ReturnsAsync(mission);
                   missionRepoMock.Setup(x => x.ApiMissionsPutAsync(id, It.IsAny<MissionDto>(), It.IsAny<string>())).ReturnsAsync(mission);
                   return missionRepoMock;
               }
            );

            // Act
            var createdMission = await service.CancelMission(id);

            // Assert
            createdMission.Should().NotBeNull();
            createdMission.Status.Should().Be(_mapper.Map<MissionDto.StatusEnum>(mission.Status));
            createdMission.CanceledDate.Should().Be(mission.CanceledDate);
        }


        //[Fact]
        //public async Task CreateMissionTest_WhenTripartite_ExpectOk()
        //{
        //    CreateFixtures(out MissionDto mission, out UserDto consultant, out UserDto commercial, out UserDto customer);

        //    var missionRequest = new Fixture().Create<CreateMissionRequestDto>();
        //    missionRequest.IsTripartite = true;
        //    missionRequest.Consultant.Id = consultant.Id;
        //    missionRequest.Commercial.Id = commercial.Id;
        //    missionRequest.Customer.Id = customer.Id;

        //    var consultantMember = new OrganizationMemberDto() { UserId = consultant.Id };
        //    var commercialMember = new OrganizationMemberDto() { UserId = commercial.Id };
        //    var customerMember = new OrganizationMemberDto() { UserId = customer.Id };

        //    var loggerMock = new Mock<ILogger<MissionService>>();
        //    var configMock = new Mock<IConfiguration>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var missionRepoMock = new Mock<IMissionRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var notifRepoMock = new Mock<INotificationRepository>();
        //    var internalNotificationRepoMock = new Mock<IInternalNotificationRepository>();

        //    missionRepoMock.Setup(x => x.ApiMissionsPostAsync(It.IsAny<MissionDto>(), It.IsAny<string>())).ReturnsAsync(mission);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(customer.Id, It.IsAny<string>())).ReturnsAsync(customer);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(commercial.Id, It.IsAny<string>())).ReturnsAsync(commercial);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(consultant.Id, It.IsAny<string>())).ReturnsAsync(consultant);
        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersPutAsync(consultantMember.UserId, It.IsAny<OrganizationMemberDto>(), It.IsAny<string>()))
        //        .Returns(Task.FromResult(consultantMember));
        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersPutAsync(commercialMember.UserId, It.IsAny<OrganizationMemberDto>(), It.IsAny<string>()))
        //        .Returns(Task.FromResult(commercialMember));
        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersPutAsync(customerMember.UserId, It.IsAny<OrganizationMemberDto>(), It.IsAny<string>()))
        //        .Returns(Task.FromResult(customerMember));
        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersByUserIdGetAsync(missionRequest.OrganizationId, consultant.Id, It.IsAny<string>()))
        //       .Returns(Task.FromResult(consultantMember));
        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersByUserIdGetAsync(missionRequest.OrganizationId, commercial.Id, It.IsAny<string>()))
        //       .Returns(Task.FromResult(commercialMember));
        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersByUserIdGetAsync(missionRequest.OrganizationId, customer.Id, It.IsAny<string>()))
        //       .Returns(Task.FromResult(customerMember));

        //    var service = new MissionService(
        //        loggerMock.Object,
        //        _mapper,
        //        configMock.Object,
        //        missionRepoMock.Object,
        //        userRepoMock.Object,
        //        eventRepoMock.Object,
        //        mailRepoMock.Object,
        //        orgaRepoMock.Object,
        //        notifRepoMock.Object,
        //        internalNotificationRepoMock.Object);

        //    //Act
        //    var createdMission = await service.CreateMission(missionRequest);

        //    //Assert
        //    createdMission.Should().NotBeNull();
        //    createdMission.Id.Should().Be(mission.Id);
        //}

        //[Fact]
        //public async Task CreateMissionTest_WhenNotTripartite_ExpectOk()
        //{
        //    CreateFixtures(out MissionDto mission, out UserDto consultant, out UserDto commercial, out UserDto customer);

        //    var missionRequest = new Fixture().Create<CreateMissionRequestDto>();
        //    missionRequest.IsTripartite = false;
        //    missionRequest.Consultant = _mapper.Map<UserDto>(consultant);
        //    missionRequest.Commercial = null;
        //    missionRequest.Customer = _mapper.Map<UserDto>(customer);

        //    var consultantMember = new OrganizationMemberDto() { UserId = consultant.Id };
        //    var customerMember = new OrganizationMemberDto() { UserId = customer.Id };

        //    var loggerMock = new Mock<ILogger<MissionService>>();
        //    var configMock = new Mock<IConfiguration>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var invoiceRepoMock = new Mock<IInvoiceRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var missionRepoMock = new Mock<IMissionRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var notifRepoMock = new Mock<INotificationRepository>();
        //    var internalNotificationRepoMock = new Mock<IInternalNotificationRepository>();

        //    missionRepoMock.Setup(x => x.ApiMissionsPostAsync(It.IsAny<MissionDto>(), It.IsAny<string>())).ReturnsAsync(mission);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(customer.Id, It.IsAny<string>())).ReturnsAsync(customer);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(commercial.Id, It.IsAny<string>())).ReturnsAsync(commercial);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(consultant.Id, It.IsAny<string>())).ReturnsAsync(consultant);
        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersPutAsync(consultantMember.UserId, It.IsAny<OrganizationMemberDto>(), It.IsAny<string>()))
        //        .Returns(Task.FromResult(consultantMember));

        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersPutAsync(customerMember.UserId, It.IsAny<OrganizationMemberDto>(), It.IsAny<string>()))
        //        .Returns(Task.FromResult(customerMember));
        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersByUserIdGetAsync(missionRequest.OrganizationId, consultant.Id, It.IsAny<string>()))
        //       .Returns(Task.FromResult(consultantMember));

        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersByUserIdGetAsync(missionRequest.OrganizationId, customer.Id, It.IsAny<string>()))
        //       .Returns(Task.FromResult(customerMember));

        //    var service = new MissionService(
        //        loggerMock.Object,
        //        _mapper,
        //        configMock.Object,
        //        missionRepoMock.Object,
        //        userRepoMock.Object,
        //        eventRepoMock.Object,
        //        mailRepoMock.Object,
        //        orgaRepoMock.Object,
        //        notifRepoMock.Object,
        //        internalNotificationRepoMock.Object);

        //    //Act
        //    var createdMission = await service.CreateMission(missionRequest);

        //    //Assert
        //    createdMission.Should().NotBeNull();
        //    createdMission.Id.Should().Be(mission.Id);
        //}

        //[Fact]
        //public async Task CreateMissionTest_WhenConsultantIsNotRegisteredToTrine_ExpectAddedAndInvited()
        //{
        //    CreateFixtures(out MissionDto mission, out UserDto consultant, out UserDto commercial, out UserDto customer);
        //    consultant.Email = "monEmail";
        //    string id = "tijohuosouhoi";

        //    var missionRequest = new Fixture().Create<CreateMissionRequestDto>();
        //    missionRequest.IsTripartite = false;
        //    missionRequest.Consultant = _mapper.Map<UserDto>(consultant);
        //    missionRequest.Commercial = null;
        //    missionRequest.Customer = _mapper.Map<UserDto>(customer);

        //    var consultantMember = new OrganizationMemberDto() { UserId = consultant.Id };
        //    var customerMember = new OrganizationMemberDto() { UserId = customer.Id };
        //    var member = new OrganizationMemberDto()
        //    {
        //        UserId = consultant.Id,
        //        Firstname = "",
        //        Lastname = "",
        //        Icon = null,
        //        Role = OrganizationMemberDto.RoleEnum.MEMBER,
        //        JoinedAt = It.IsAny<DateTime>()
        //    };

        //    var loggerMock = new Mock<ILogger<MissionService>>();
        //    var configMock = new Mock<IConfiguration>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var invoiceRepoMock = new Mock<IInvoiceRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var missionRepoMock = new Mock<IMissionRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var notifRepoMock = new Mock<INotificationRepository>();
        //    var internalNotificationRepoMock = new Mock<IInternalNotificationRepository>();

        //    missionRepoMock.Setup(x => x.ApiMissionsPostAsync(It.IsAny<MissionDto>(), It.IsAny<string>())).ReturnsAsync(mission);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(consultant.Id, It.IsAny<string>())).ReturnsAsync(consultant);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(customer.Id, It.IsAny<string>())).ReturnsAsync(customer);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(commercial.Id, It.IsAny<string>())).ReturnsAsync(commercial);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(id, It.IsAny<string>())).ReturnsAsync(consultant);
        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersPutAsync(consultantMember.UserId, It.IsAny<OrganizationMemberDto>(), It.IsAny<string>()))
        //        .Returns(Task.FromResult(consultantMember));

        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersPutAsync(customerMember.UserId, It.IsAny<OrganizationMemberDto>(), It.IsAny<string>()))
        //        .Returns(Task.FromResult(customerMember));
        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersByUserIdGetAsync(missionRequest.OrganizationId, consultant.Id, It.IsAny<string>()))
        //       .Returns(Task.FromResult(consultantMember));
        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersByUserIdGetAsync(missionRequest.OrganizationId, customer.Id, It.IsAny<string>()))
        //       .Returns(Task.FromResult(customerMember));
        //    userRepoMock
        //        .Setup(x => x.ApiUsersPostAsync(It.IsAny<UserDto>(), It.IsAny<string>()))
        //        .ReturnsAsync(JsonConvert.SerializeObject(id));
        //    orgaRepoMock
        //        .Setup(x => x.ApiOrganizationsByOrganizationIdMembersPutAsync(missionRequest.OrganizationId, It.Is<OrganizationMemberDto>(y => y.Lastname == "" && y.Firstname == "" && y.UserId == id), It.IsAny<string>()))
        //        .ReturnsAsync(_mapper.Map<OrganizationMemberDto>(member));

        //    var service = new MissionService(loggerMock.Object, _mapper, configMock.Object, missionRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, invoiceRepoMock.Object,
        //        mailRepoMock.Object, orgaRepoMock.Object, notifRepoMock.Object, internalNotificationRepoMock.Object);


        //    //Act
        //    var createdMission = await service.CreateMission(missionRequest);

        //    //Assert
        //    createdMission.Should().NotBeNull();
        //    createdMission.Id.Should().Be(mission.Id);
        //}

        //[Fact]
        //public async Task CreateMissionTest_WhenConsultantIsNotAMember_ExpectInvited()
        //{
        //    CreateFixtures(out MissionDto mission, out UserDto consultant, out UserDto commercial, out UserDto customer);
        //    consultant = new UserDto();
        //    consultant.Email = "monEmail";
        //    string id = "tijohuosouhoi";

        //    var missionRequest = new Fixture().Create<CreateMissionRequestDto>();
        //    missionRequest.IsTripartite = false;
        //    missionRequest.Consultant = _mapper.Map<UserDto>(consultant);
        //    missionRequest.Commercial = null;
        //    missionRequest.Customer = _mapper.Map<UserDto>(customer);

        //    var consultantMember = new OrganizationMemberDto() { UserId = consultant.Id };
        //    var customerMember = new OrganizationMemberDto() { UserId = customer.Id };
        //    var member = new OrganizationMemberDto()
        //    {
        //        UserId = consultant.Id,
        //        Firstname = "",
        //        Lastname = "",
        //        Icon = null,
        //        Role = OrganizationMemberDto.RoleEnum.MEMBER,
        //        JoinedAt = It.IsAny<DateTime>()
        //    };
        //    var searchResults = new Fixture().Create<List<UserDto>>();
        //    var loggerMock = new Mock<ILogger<MissionService>>();
        //    var configMock = new Mock<IConfiguration>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var invoiceRepoMock = new Mock<IInvoiceRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var missionRepoMock = new Mock<IMissionRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var notifRepoMock = new Mock<INotificationRepository>();
        //    var internalNotificationRepoMock = new Mock<IInternalNotificationRepository>();

        //    missionRepoMock.Setup(x => x.ApiMissionsPostAsync(It.IsAny<MissionDto>(), It.IsAny<string>())).ReturnsAsync(mission);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(customer.Id, It.IsAny<string>())).ReturnsAsync(customer);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(commercial.Id, It.IsAny<string>())).ReturnsAsync(commercial);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(searchResults.FirstOrDefault().Id, It.IsAny<string>())).ReturnsAsync(consultant);
        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersPutAsync(consultantMember.UserId, It.IsAny<OrganizationMemberDto>(), It.IsAny<string>()))
        //        .Returns(Task.FromResult(consultantMember));

        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersPutAsync(customerMember.UserId, It.IsAny<OrganizationMemberDto>(), It.IsAny<string>()))
        //        .Returns(Task.FromResult(customerMember));
        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersByUserIdGetAsync(missionRequest.OrganizationId, consultant.Id, It.IsAny<string>()))
        //       .Returns(Task.FromResult(consultantMember));
        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersByUserIdGetAsync(missionRequest.OrganizationId, customer.Id, It.IsAny<string>()))
        //       .Returns(Task.FromResult(customerMember));
        //    userRepoMock
        //        .Setup(x => x.ApiUsersPostAsync(It.IsAny<UserDto>(), It.IsAny<string>()))
        //        .ReturnsAsync(JsonConvert.SerializeObject(id));
        //    userRepoMock
        //        .Setup(x => x.ApiUsersSearchGetAsync(consultant.Email, null, null, It.IsAny<string>()))
        //        .ReturnsAsync(searchResults);
        //    orgaRepoMock
        //        .Setup(x => x.ApiOrganizationsByOrganizationIdMembersPutAsync(missionRequest.OrganizationId, It.Is<OrganizationMemberDto>(y => y.Lastname == searchResults.FirstOrDefault().Lastname && y.Firstname == searchResults.FirstOrDefault().Firstname && y.UserId == searchResults.FirstOrDefault().Id), It.IsAny<string>()))
        //        .ReturnsAsync(_mapper.Map<OrganizationMemberDto>(member));

        //    var service = new MissionService(loggerMock.Object, _mapper, configMock.Object, missionRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, invoiceRepoMock.Object,
        //        mailRepoMock.Object, orgaRepoMock.Object, notifRepoMock.Object, internalNotificationRepoMock.Object);


        //    //Act
        //    var createdMission = await service.CreateMission(missionRequest);

        //    //Assert
        //    createdMission.Should().NotBeNull();
        //    createdMission.Id.Should().Be(mission.Id);
        //}

        [Fact]
        public async Task CreateMissionTest_WhenConsultantIsNull_ExpectBusinessException()
        {
            CreateFixtures(out MissionDto mission, out UserDto consultant, out UserDto commercial, out UserDto customer);

            var missionRequest = new Fixture().Create<CreateMissionRequestDto>();
            missionRequest.IsTripartite = false;
            missionRequest.Consultant = null;
            missionRequest.Commercial = null;
            missionRequest.Customer = _mapper.Map<UserDto>(customer);

            MissionService service = GenerateService(mission, consultant, commercial, customer);

            //Act
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.CreateMission(missionRequest));

            //Assert
            exc.Message.Should().NotBeNull();
            exc.Message.Should().Be(ErrorMessages.atLeastTwoUsersNeeded);
        }

        [Fact]
        public async Task CreateMissionTest_WhenCustomerIsNull_ExpectBusinessException()
        {
            CreateFixtures(out MissionDto mission, out UserDto consultant, out UserDto commercial, out UserDto customer);

            var missionRequest = new Fixture().Create<CreateMissionRequestDto>();
            missionRequest.IsTripartite = false;
            missionRequest.Consultant = _mapper.Map<UserDto>(consultant);
            missionRequest.Commercial = null;
            missionRequest.Customer = null;

            MissionService service = GenerateService(mission, consultant, commercial, customer);

            //Act
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.CreateMission(missionRequest));

            //Assert
            exc.Message.Should().NotBeNull();
            exc.Message.Should().Be(ErrorMessages.atLeastTwoUsersNeeded);
        }

        [Fact]
        public async Task CreateMissionTest_WhenCommercialIsNullAndIsTripartite_ExpectBusinessException()
        {
            CreateFixtures(out MissionDto mission, out UserDto consultant, out UserDto commercial, out UserDto customer);

            var missionRequest = new Fixture().Create<CreateMissionRequestDto>();
            missionRequest.IsTripartite = true;
            missionRequest.Consultant = _mapper.Map<UserDto>(consultant);
            missionRequest.Commercial = null;
            missionRequest.Consultant = _mapper.Map<UserDto>(customer);

            MissionService service = GenerateService(mission, consultant, commercial, customer);

            //Act
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.CreateMission(missionRequest));

            //Assert
            exc.Message.Should().NotBeNull();
            exc.Message.Should().Be(ErrorMessages.atLeastTwoUsersNeeded);
        }

        [Fact]
        public async Task CreateMissionTest_WhenUserApiReturnsNull_ExpectBusinessException()
        {
            CreateFixtures(out MissionDto mission, out UserDto consultant, out UserDto commercial, out UserDto customer);

            var missionRequest = new Fixture().Create<CreateMissionRequestDto>();
            missionRequest.IsTripartite = true;
            missionRequest.Consultant.Id = consultant.Id;
            missionRequest.Commercial.Id = commercial.Id;
            missionRequest.Customer.Id = customer.Id;

            var service = CreatemissionServiceWith
            (
               missionRepoMock =>
               {
                   missionRepoMock.Setup(x => x.ApiMissionsPostAsync(It.IsAny<MissionDto>(), It.IsAny<string>())).ReturnsAsync(mission);
                   return missionRepoMock;
               },
               userRepoMock =>
               {
                   userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(customer.Id, It.IsAny<string>())).ReturnsAsync(value: null);
                   userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(commercial.Id, It.IsAny<string>())).ReturnsAsync(value: null);
                   userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(consultant.Id, It.IsAny<string>())).ReturnsAsync(value: null);
                   return userRepoMock;
               }
            );

            //Act
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.CreateMission(missionRequest));

            //Assert
            exc.Message.Should().NotBeNull();
            exc.Message.Should().Be(ErrorMessages.errorOccuredWhileGettingUsers);
        }


        [Fact]
        public async Task GetMissionTest_NominalCase()
        {
            CreateFixtures(out MissionDto mission, out UserDto consultant, out UserDto commercial, out UserDto customer);

            var service = CreatemissionServiceWith
            (
               missionRepoMock =>
               {
                   missionRepoMock.Setup(x => x.ApiMissionsByIdGetAsync(mission.Id, It.IsAny<string>())).ReturnsAsync(mission);
                   return missionRepoMock;
               },
               userRepoMock =>
               {
                   userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(customer.Id, It.IsAny<string>())).ReturnsAsync(customer);
                   userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(commercial.Id, It.IsAny<string>())).ReturnsAsync(commercial);
                   userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(consultant.Id, It.IsAny<string>())).ReturnsAsync(consultant);
                   return userRepoMock;
               }
            );

            //Act
            var result = await service.GetMissionById(mission.Id);

            //Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetMissionTest_WhenMissionDoesntExist_ExpectNull()
        {
            CreateFixtures(out MissionDto mission, out UserDto consultant, out UserDto commercial, out UserDto customer);

            var service = CreatemissionServiceWith
            (
               missionRepoMock =>
               {
                   missionRepoMock.Setup(x => x.ApiMissionsByIdGetAsync("toto", It.IsAny<string>())).ReturnsAsync(value: null);
                   return missionRepoMock;
               }
            );

            //Act
            var result = await service.GetMissionById("toto");

            //Assert
            result.Should().BeNull();
        }



        #region ContractPreview

        //[Theory]
        //[InlineData(true)]
        //[InlineData(false)]
        //public async Task GetContractPreviewTest_NominalCase_ExpectOk(bool isTripartite)
        //{
        //    CreateFixtures(out MissionDto mission, out UserDto consultant, out UserDto commercial, out UserDto customer);

        //    var missionRequest = new Fixture().Create<CreateMissionRequestDto>();
        //    missionRequest.IsTripartite = isTripartite;
        //    missionRequest.Consultant.Id = consultant.Id;
        //    missionRequest.Customer.Id = customer.Id;

        //    if (isTripartite)
        //        missionRequest.Commercial.Id = commercial.Id;
        //    else
        //        missionRequest.Commercial.Id = null;

        //    MissionService service = GenerateService(mission, consultant, commercial, customer);

        //    //Act
        //    var contract = await service.GetContractPreview(missionRequest);

        //    //Assert
        //    contract.Should().NotBeNull();
        //    contract.Content.Should().Contain(consultant.Lastname);
        //    contract.Content.Should().Contain(customer.Lastname);

        //    if (isTripartite)
        //        contract.Content.Should().Contain(commercial.Lastname);
        //    else
        //        contract.Content.Should().NotContain(commercial.Lastname);
        //}

        //[Theory]
        //[InlineData(true, "Commercial.Id", "Consultant.Id", "Customer.Id")]
        //[InlineData(false, "Commercial.Id", "Consultant.Id", "Customer.Id")]
        //public async Task GetContractPreviewTest_WithNullProps_ExpectBusinessException(bool isTripartite, string Commercial.Id, string Consultant.Id, string Customer.Id)
        //{
        //    //Arrange
        //    string missionId;
        //    UserDto consultant, commercial, customer;
        //    CreateFixtures(out missionId, out consultant, out commercial, out customer);

        //    var missionRequest = new Fixture().Create<CreateMissionRequestDto>();
        //    missionRequest.IsTripartite = isTripartite;
        //    missionRequest.Consultant.Id = consultant.Id;
        //    missionRequest.Customer.Id = customer.Id;

        //    if (isTripartite)
        //        missionRequest.Commercial.Id = commercial.Id;
        //    else
        //        missionRequest.Commercial.Id = null;

        //    var userRepoMock = new Mock<IUserRepository>();
        //    var missionRepoMock = new Mock<IMissionApi>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var invoiceRepoMock = new Mock<IInvoiceRepository>();
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(customer.Id, It.IsAny<string>())).ReturnsAsync(customer);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(commercial.Id, It.IsAny<string>())).ReturnsAsync(commercial);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(consultant.Id, It.IsAny<string>())).ReturnsAsync(consultant);
        //    missionRepoMock.Setup(x => x.ApiMissionsPostAsync(It.IsAny<MissionDto>(), It.IsAny<string>())).ReturnsAsync(missionId);

        //    var service = new MissionService(_loggerMock.Object, _mapper, missionRepoMock.Object, userRepoMock.Object, eventRepoMock.Object, invoiceRepoMock.Object);

        //    //Act
        //    var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.GetContractPreview(missionRequest));

        //    //Assert
        //    exc.Should().NotBeNull();
        //    exc.Message.Should().Be(ErrorMessages.errorOccuredWhileGettingUsers);
        //}

        #endregion

        private void CreateFixtures(out MissionDto mission, out UserDto consultant, out UserDto commercial, out UserDto customer)
        {
            mission = new Fixture().Create<MissionDto>();
            consultant = new Fixture().Create<UserDto>();
            commercial = new Fixture().Create<UserDto>();
            customer = new Fixture().Create<UserDto>();
            mission.Consultant.Id = consultant.Id;
            mission.Commercial.Id = commercial.Id;
            mission.Customer.Id = customer.Id;
        }

        private MissionService GenerateService(MissionDto mission, UserDto consultant, UserDto commercial, UserDto customer)
        {
            return CreatemissionServiceWith
            (
                missionRepoMock =>
                {
                    missionRepoMock.Setup(x => x.ApiMissionsPostAsync(It.IsAny<MissionDto>(), It.IsAny<string>())).ReturnsAsync(mission);
                    return missionRepoMock;
                },
                userRepoMock =>
                {
                    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(customer.Id, It.IsAny<string>())).ReturnsAsync(customer);
                    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(commercial.Id, It.IsAny<string>())).ReturnsAsync(commercial);
                    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(consultant.Id, It.IsAny<string>())).ReturnsAsync(consultant);
                    return userRepoMock;
                },
                orgaRepoMock =>
                {
                    //userRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersPutAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Task<OrganizationMemberDto>());
                    return orgaRepoMock;
                }
            );
        }

        private MissionService CreatemissionServiceWith(Func<Mock<IMissionRepository>, Mock<IMissionRepository>> missionFunc)
        {
            var eventRepoMock = new Mock<IEventRepository>();
            var mailRepoMock = new Mock<IMailRepository>();
            var missionRepoMock = new Mock<IMissionRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var orgaRepoMock = new Mock<IOrganizationRepository>();
            var notifRepoMock = new Mock<INotificationRepository>();
            var internalNotificationRepoMock = new Mock<IInternalNotificationRepository>();

            var missionRepository = missionFunc(missionRepoMock);
            var service = new MissionService(
                _loggerMock.Object,
                _mapper,
                _configurationMock.Object,
                missionRepoMock.Object,
                userRepoMock.Object,
                eventRepoMock.Object,
                mailRepoMock.Object,
                orgaRepoMock.Object,
                notifRepoMock.Object,
                internalNotificationRepoMock.Object);
            return service;
        }

        private MissionService CreatemissionServiceWith(Func<Mock<IMissionRepository>, Mock<IMissionRepository>> missionFunc, Func<Mock<IUserRepository>, Mock<IUserRepository>> userFunc)
        {
            var eventRepoMock = new Mock<IEventRepository>();
            var mailRepoMock = new Mock<IMailRepository>();
            var missionRepoMock = new Mock<IMissionRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var orgaRepoMock = new Mock<IOrganizationRepository>();
            var notifRepoMock = new Mock<INotificationRepository>();
            var internalNotificationRepoMock = new Mock<IInternalNotificationRepository>();

            var missionRepository = missionFunc(missionRepoMock);
            var userApi = userFunc(userRepoMock);
            var service = new MissionService(
                _loggerMock.Object,
                _mapper,
                _configurationMock.Object,
                missionRepoMock.Object,
                userRepoMock.Object,
                eventRepoMock.Object,
                mailRepoMock.Object,
                orgaRepoMock.Object,
                notifRepoMock.Object,
                internalNotificationRepoMock.Object);
            return service;
        }

        private MissionService CreatemissionServiceWith(Func<Mock<IMissionRepository>, Mock<IMissionRepository>> missionFunc, Func<Mock<IUserRepository>, Mock<IUserRepository>> userFunc, Func<Mock<IOrganizationRepository>, Mock<IOrganizationRepository>> orgaFunc)
        {
            var eventRepoMock = new Mock<IEventRepository>();
            var invoiceRepoMock = new Mock<IInvoiceRepository>();
            var mailRepoMock = new Mock<IMailRepository>();
            var missionRepoMock = new Mock<IMissionRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var orgaRepoMock = new Mock<IOrganizationRepository>();
            var notifRepoMock = new Mock<INotificationRepository>();
            var internalNotificationRepoMock = new Mock<IInternalNotificationRepository>();

            var missionRepository = missionFunc(missionRepoMock);
            var orgaApi = orgaFunc(orgaRepoMock);
            var userApi = userFunc(userRepoMock);
            var service = new MissionService(
                _loggerMock.Object,
                _mapper,
                _configurationMock.Object,
                missionRepoMock.Object,
                userRepoMock.Object,
                eventRepoMock.Object,
                mailRepoMock.Object,
                orgaRepoMock.Object,
                notifRepoMock.Object,
                internalNotificationRepoMock.Object);
            return service;
        }

    }
}
