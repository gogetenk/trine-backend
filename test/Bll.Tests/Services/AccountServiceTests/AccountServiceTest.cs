using System.Threading.Tasks;
using Assistance.Event.Dal.Repositories;
using Assistance.Operational.Bll.Impl.Builders;
using Assistance.Operational.Bll.Impl.Errors;
using Assistance.Operational.Bll.Impl.Services;
using Assistance.Operational.Dal.Repositories;
using AutoFixture;
using Dto;
using FluentAssertions;
using Moq;
using Sogetrel.Sinapse.Framework.Exceptions;
using Xunit;

namespace Assistance.Operational.Bll.Tests.Services.AccountServiceTests
{
    public class AccountServiceTest : TestBase
    {
        private const string _userApiVersion = "1.0";

        public AccountServiceTest() : base()
        {
        }

        #region Company

        //[Fact]
        //public async Task RegisterCompanyTest_NominalCase_ExpectMissionId()
        //{
        //    //Arrange
        //    var user = new Fixture().Create<UserDto>();
        //    var company = new Fixture().Create<RegisterCompanyRequestDto>();
        //    string createdId = "created";


        //    var companyRepoMock = new Mock<ICompanyRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var authRepoMock = new Mock<IAuthenticationRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var inviteRepoMock = new Mock<IInviteRepository>();
        //    var internalNotifMock = new Mock<IInternalNotificationRepository>();

        //    companyRepoMock.Setup(x => x.ApiCompaniesSearchGetAsync(company.Siret, null, It.IsAny<string>())).ReturnsAsync(value: null);
        //    companyRepoMock.Setup(x => x.ApiCompaniesPostAsync(It.IsAny<Company>(), It.IsAny<string>())).ReturnsAsync(createdId);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(company.UserId, It.IsAny<string>())).ReturnsAsync(user);

        //    var service = new AccountService(_loggerMock.Object, _mapper, userRepoMock.Object,  eventRepoMock.Object, _configuration.Object, mailRepoMock.Object, inviteRepoMock.Object, orgaRepoMock.Object, internalNotifMock.Object);

        //    //Act
        //    var results = await service.RegisterCompany(company);

        //    //Assert
        //    results.Should().NotBeNull();
        //    results.Should().Be(createdId);
        //}

        //[Fact]
        //public async Task RegisterCompanyTest_WhenCompanyAlreadyExist_ExpectBusinessException()
        //{
        //    //Arrange
        //    var companyToRegister = new Fixture().Create<RegisterCompanyRequestDto>();
        //    var company = new Fixture().Create<Company>();

        //    var companyRepoMock = new Mock<ICompanyRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var authRepoMock = new Mock<IAuthenticationRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var inviteRepoMock = new Mock<IInviteRepository>();
        //    var internalNotifMock = new Mock<IInternalNotificationRepository>();

        //    companyRepoMock.Setup(x => x.ApiCompaniesSearchGetAsync(company.Siret, null, null)).ReturnsAsync(company);

        //    var service = new AccountService(_loggerMock.Object, _mapper, userRepoMock.Object,  eventRepoMock.Object, _configuration.Object, mailRepoMock.Object, inviteRepoMock.Object, orgaRepoMock.Object, internalNotifMock.Object);

        //    //Act / Assert
        //    var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.RegisterCompany(companyToRegister));
        //    exc.Message.Should().Be(ErrorMessages.companyAlreadyExistErrorMessage);
        //}

        //[Fact]
        //public async Task RegisterCompanyTest_WithNullSiret_ExpectBusinessException()
        //{
        //    //Arrange
        //    var company = new Fixture().Create<RegisterCompanyRequestDto>();
        //    company.Siret = null;

        //    var companyRepoMock = new Mock<ICompanyRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var authRepoMock = new Mock<IAuthenticationRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var inviteRepoMock = new Mock<IInviteRepository>();
        //    var internalNotifMock = new Mock<IInternalNotificationRepository>();

        //    var service = new AccountService(_loggerMock.Object, _mapper, userRepoMock.Object,  eventRepoMock.Object, _configuration.Object, mailRepoMock.Object, inviteRepoMock.Object, orgaRepoMock.Object, internalNotifMock.Object);

        //    //Act / Assert
        //    var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.RegisterCompany(company));
        //    exc.Message.Should().Be(ErrorMessages.siretCannotBeNull);
        //}

        //[Theory]
        //[InlineData("", "userId", "rcs", "tva")]
        //[InlineData("name", "", "rcs", "tva")]
        //[InlineData("name", "userId", "", "tva")]
        //[InlineData("name", "userId", "rcs", "")]
        //[InlineData(null, null, null, null)]
        //public async Task RegisterCompanyTest_WithNullProps_ExpectSpecificationException(string name, string userId, string rcs, string tva)
        //{
        //    //Arrange
        //    var company = new Fixture().Create<RegisterCompanyRequestDto>();
        //    company.Name = name;
        //    company.UserId = userId;
        //    company.Rcs = rcs;
        //    company.TvaNumber = tva;

        //    var companyRepoMock = new Mock<ICompanyRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var authRepoMock = new Mock<IAuthenticationRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var inviteRepoMock = new Mock<IInviteRepository>();
        //    var internalNotifMock = new Mock<IInternalNotificationRepository>();

        //    var service = new AccountService(_loggerMock.Object, _mapper, userRepoMock.Object, eventRepoMock.Object, _configuration.Object, mailRepoMock.Object, inviteRepoMock.Object, orgaRepoMock.Object, internalNotifMock.Object);

        //    //Act / Assert
        //    var exc = await Assert.ThrowsAsync<SpecificationException>(async () => await service.RegisterCompany(company));
        //    exc.Description.Should().Be(ErrorMessages.companyBadFormat);
        //}

        #endregion


        #region User

        //[Fact]
        //public async Task AuthenticateUserTest_NominalCase_ExpectMissionId()
        //{
        //    //Arrange
        //    var login = new Fixture().Create<UserCredentialsDto>();
        //    var user = new Fixture().Create<UserDto>();
        //    var token = new Fixture().Create<Token>();

        //    var companyRepoMock = new Mock<ICompanyRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var authRepoMock = new Mock<IAuthenticationRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var inviteRepoMock = new Mock<IInviteRepository>();
        //    var internalNotifMock = new Mock<IInternalNotificationRepository>();

        //    userRepoMock.Setup(x => x.GetByEmailAndPassword(login.Email, login.Password)).ReturnsAsync(user);
        //    //authRepoMock.Setup(x => x.ApiAuthenticationPostAsync(It.IsAny<UserCredentials>(), null)).ReturnsAsync(token);
        //    _configuration.SetupGet(x => x["AuthenticationSettings:TokenExpirationInSeconds"]).Returns("10");
        //    _configuration.SetupGet(x => x["AuthenticationSettings:Key"]).Returns("ceci est une clef introubable (théoriquement)");
        //    _configuration.SetupGet(x => x["AuthenticationSettings:Issuer"]).Returns("10");

        //    var service = new AccountService(_loggerMock.Object, _mapper, userRepoMock.Object, eventRepoMock.Object, _configuration.Object, mailRepoMock.Object, inviteRepoMock.Object, orgaRepoMock.Object, internalNotifMock.Object);

        //    //Act
        //    var dto = await service.AuthenticateUser(login);

        //    //Assert
        //    token.Should().NotBeNull();
        //}

        //[Fact]
        //public async Task RegisterUserTest_WhenCustomer_ExpectConsistentToken()
        //{
        //    //Arrange
        //    var guidCode = Guid.NewGuid();
        //    var userToRegister = new Fixture().Create<RegisterUserRequestDto>();
        //    userToRegister.Role = RegisterUserRequestDto.GlobalRoleEnum.Admin;
        //    var user = new Fixture().Create<UserDto>();
        //    var guest = new Fixture().Create<UserDto>();
        //    var token = new Fixture().Create<Token>();

        //    var invite = new Fixture().Create<InviteDto>();
        //    invite.InviterId = user.Id;
        //    invite.Code = guidCode;
        //    invite.Expires = DateTime.UtcNow.AddDays(1);

        //    token.UserId = user.Id;
        //    token.ExpiresIn = long.MaxValue;
        //    var member = new OrganizationMemberDto()
        //    {
        //        Firstname = user.Firstname,
        //        Lastname = user.Lastname,
        //        JoinedAt = DateTime.UtcNow,
        //        Role = OrganizationMemberDto.RoleEnum.GUEST,
        //        UserId = user.Id
        //    };

        //    var companyRepoMock = new Mock<ICompanyRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var authRepoMock = new Mock<IAuthenticationRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var inviteRepoMock = new Mock<IInviteRepository>();
        //    var internalNotifMock = new Mock<IInternalNotificationRepository>();

        //    userRepoMock.Setup(x => x.GetByEmailAndPassword(userToRegister.Email, userToRegister.Password)).ReturnsAsync(user);
        //    userRepoMock.Setup(x => x.ApiUsersSearchGetAsync(userToRegister.Email, null, null, It.IsAny<string>())).ReturnsAsync(value: null);
        //    userRepoMock.Setup(x => x.ApiUsersSearchGetAsync(invite.GuestEmail, null, null, null)).ReturnsAsync(new List<UserDto>() { guest });
        //    userRepoMock.Setup(x => x.ApiUsersPostAsync(It.IsAny<UserDto>(), It.IsAny<string>())).ReturnsAsync(user.Id);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(user.Id, It.IsAny<string>())).ReturnsAsync(user);
        //    authRepoMock.Setup(x => x.ApiAuthenticationPostAsync(It.IsAny<UserCredentials>(), It.IsAny<string>())).ReturnsAsync(token);
        //    inviteRepoMock.Setup(x => x.ApiInvitesByCodeGetAsync(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(invite);
        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersPutAsync(It.IsAny<string>(), It.IsAny<OrganizationMemberDto>(), null)).ReturnsAsync(member);
        //    _configuration.SetupGet(x => x["AuthenticationSettings:TokenExpirationInSeconds"]).Returns("10");
        //    _configuration.SetupGet(x => x["AuthenticationSettings:Key"]).Returns("ceci est une clef introuvable (théoriquement)");
        //    _configuration.SetupGet(x => x["AuthenticationSettings:Issuer"]).Returns("10");

        //    var service = new AccountService(_loggerMock.Object, _mapper, userRepoMock.Object, eventRepoMock.Object, _configuration.Object, mailRepoMock.Object, inviteRepoMock.Object, orgaRepoMock.Object, internalNotifMock.Object);

        //    //Act
        //    var results = await service.RegisterUser(userToRegister);

        //    //Assert
        //    results.Should().NotBeNull();
        //    results.Token.Should().NotBeNull();
        //    results.Token.UserId.Should().Be(user.Id);
        //}

        //[Fact]
        //public async Task RegisterUserTest_WhenInviteCodeIsNull_ExpectBusinessException()
        //{
        //    //Arrange
        //    var guidCode = Guid.NewGuid();
        //    var userToRegister = new Fixture().Create<RegisterUserRequestDto>();
        //    var user = new Fixture().Create<UserDto>();
        //    var token = new Fixture().Create<Token>();

        //    var invite = new Fixture().Create<Invite>();
        //    invite.InviterId = user.Id;
        //    invite.Code = guidCode;
        //    invite.Expires = DateTime.UtcNow.AddDays(1);

        //    token.UserId = user.Id;
        //    token.ExpiresIn = long.MaxValue;
        //    var member = new OrganizationMemberDto()
        //    {
        //        Firstname = user.Firstname,
        //        Lastname = user.Lastname,
        //        JoinedAt = DateTime.UtcNow,
        //        Role = OrganizationMemberDto.RoleEnum.GUEST,
        //        UserId = user.Id
        //    };

        //    var companyRepoMock = new Mock<ICompanyRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var authRepoMock = new Mock<IAuthenticationRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var inviteRepoMock = new Mock<IInviteRepository>();
        //    var internalNotifMock = new Mock<IInternalNotificationRepository>();

        //    userRepoMock.Setup(x => x.ApiUsersSearchGetAsync(userToRegister.Email, null, null, null)).ReturnsAsync(value: null);
        //    userRepoMock.Setup(x => x.ApiUsersPostAsync(It.IsAny<UserDto>(), It.IsAny<string>())).ReturnsAsync(user.Id);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(user.Id, It.IsAny<string>())).ReturnsAsync(user);
        //    authRepoMock.Setup(x => x.ApiAuthenticationPostAsync(It.IsAny<UserCredentials>(), It.IsAny<string>())).ReturnsAsync(token);
        //    inviteRepoMock.Setup(x => x.ApiInvitesByCodeGetAsync(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(value: null);
        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersPutAsync(It.IsAny<string>(), It.IsAny<OrganizationMemberDto>(), null)).ReturnsAsync(member);

        //    var service = new AccountService(_loggerMock.Object, _mapper, userRepoMock.Object,  eventRepoMock.Object, _configuration.Object, mailRepoMock.Object, inviteRepoMock.Object, orgaRepoMock.Object, internalNotifMock.Object);

        //    //Act
        //    var results = await Assert.ThrowsAsync<BusinessException>(() => service.RegisterUser(userToRegister));

        //    //Assert
        //    results.Should().NotBeNull();
        //    results.Message.Should().Be(ErrorMessages.invitationDoesntExist);
        //}

        //[Fact]
        //public async Task RegisterUserTest_WhenInviteCodeIsRevoked_ExpectBusinessException()
        //{
        //    //Arrange
        //    var guidCode = Guid.NewGuid();
        //    var userToRegister = new Fixture().Create<RegisterUserRequestDto>();
        //    var user = new Fixture().Create<UserDto>();
        //    var token = new Fixture().Create<Token>();

        //    var invite = new Fixture().Create<InviteDto>();
        //    invite.InviterId = user.Id;
        //    invite.Code = guidCode;
        //    invite.CurrentStatus = InviteDto.Status.Canceled;
        //    invite.Expires = DateTime.UtcNow.AddDays(1);

        //    token.UserId = user.Id;
        //    token.ExpiresIn = long.MaxValue;
        //    var member = new OrganizationMemberDto()
        //    {
        //        Firstname = user.Firstname,
        //        Lastname = user.Lastname,
        //        JoinedAt = DateTime.UtcNow,
        //        Role = OrganizationMemberDto.RoleEnum.GUEST,
        //        UserId = user.Id
        //    };

        //    var companyRepoMock = new Mock<ICompanyRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var authRepoMock = new Mock<IAuthenticationRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var inviteRepoMock = new Mock<IInviteRepository>();
        //    var internalNotifMock = new Mock<IInternalNotificationRepository>();

        //    userRepoMock.Setup(x => x.ApiUsersSearchGetAsync(userToRegister.Email, null, null, null)).ReturnsAsync(value: null);
        //    userRepoMock.Setup(x => x.ApiUsersPostAsync(It.IsAny<UserDto>(), It.IsAny<string>())).ReturnsAsync(user.Id);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(user.Id, It.IsAny<string>())).ReturnsAsync(user);
        //    authRepoMock.Setup(x => x.ApiAuthenticationPostAsync(It.IsAny<UserCredentials>(), It.IsAny<string>())).ReturnsAsync(token);
        //    inviteRepoMock.Setup(x => x.ApiInvitesByCodeGetAsync(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(invite);
        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersPutAsync(It.IsAny<string>(), It.IsAny<OrganizationMemberDto>(), null)).ReturnsAsync(member);

        //    var service = new AccountService(_loggerMock.Object, _mapper, userRepoMock.Object,  eventRepoMock.Object, _configuration.Object, mailRepoMock.Object, inviteRepoMock.Object, orgaRepoMock.Object, internalNotifMock.Object);

        //    //Act
        //    var results = await Assert.ThrowsAsync<BusinessException>(() => service.RegisterUser(userToRegister));

        //    //Assert
        //    results.Should().NotBeNull();
        //    results.Message.Should().Be(ErrorMessages.inviteRevoked);
        //}

        //[Fact]
        //public async Task RegisterUserTest_WhenInviteIsExpired_ExpectBusinessException()
        //{
        //    //Arrange
        //    var guidCode = Guid.NewGuid();
        //    var userToRegister = new Fixture().Create<RegisterUserRequestDto>();
        //    var user = new Fixture().Create<UserDto>();
        //    var token = new Fixture().Create<Token>();

        //    var invite = new Fixture().Create<InviteDto>();
        //    invite.InviterId = user.Id;
        //    invite.Code = guidCode;
        //    invite.Expires = DateTime.UtcNow.AddDays(-1);

        //    token.UserId = user.Id;
        //    token.ExpiresIn = long.MaxValue;
        //    var member = new OrganizationMemberDto()
        //    {
        //        Firstname = user.Firstname,
        //        Lastname = user.Lastname,
        //        JoinedAt = DateTime.UtcNow,
        //        Role = OrganizationMemberDto.RoleEnum.GUEST,
        //        UserId = user.Id
        //    };

        //    var companyRepoMock = new Mock<ICompanyRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var authRepoMock = new Mock<IAuthenticationRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var inviteRepoMock = new Mock<IInviteRepository>();
        //    var internalNotifMock = new Mock<IInternalNotificationRepository>();

        //    userRepoMock.Setup(x => x.ApiUsersSearchGetAsync(userToRegister.Email, null, null, null)).ReturnsAsync(value: null);
        //    userRepoMock.Setup(x => x.ApiUsersPostAsync(It.IsAny<UserDto>(), It.IsAny<string>())).ReturnsAsync(user.Id);
        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(user.Id, It.IsAny<string>())).ReturnsAsync(user);
        //    authRepoMock.Setup(x => x.ApiAuthenticationPostAsync(It.IsAny<UserCredentials>(), It.IsAny<string>())).ReturnsAsync(token);
        //    inviteRepoMock.Setup(x => x.ApiInvitesByCodeGetAsync(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(invite);
        //    orgaRepoMock.Setup(x => x.ApiOrganizationsByOrganizationIdMembersPutAsync(It.IsAny<string>(), It.IsAny<OrganizationMemberDto>(), null)).ReturnsAsync(member);

        //    var service = new AccountService(_loggerMock.Object, _mapper, userRepoMock.Object,  eventRepoMock.Object, _configuration.Object, mailRepoMock.Object, inviteRepoMock.Object, orgaRepoMock.Object, internalNotifMock.Object);

        //    //Act
        //    var results = await Assert.ThrowsAsync<BusinessException>(() => service.RegisterUser(userToRegister));

        //    //Assert
        //    results.Should().NotBeNull();
        //    results.Message.Should().Be(ErrorMessages.inviteExpired);
        //}

        //[Fact]
        //public async Task RegisterUserTest_WithNullEmail_ExpectBusinessException()
        //{
        //    //Arrange
        //    var user = new Fixture().Create<RegisterUserRequestDto>();
        //    user.Email = null;

        //    var companyRepoMock = new Mock<ICompanyRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var authRepoMock = new Mock<IAuthenticationRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var inviteRepoMock = new Mock<IInviteRepository>();
        //    var internalNotifMock = new Mock<IInternalNotificationRepository>();

        //    var service = new AccountService(_loggerMock.Object, _mapper, userRepoMock.Object, eventRepoMock.Object, _configuration.Object, mailRepoMock.Object, inviteRepoMock.Object, orgaRepoMock.Object, internalNotifMock.Object);

        //    // TEst

        //    //Act / Assert
        //    var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.RegisterUser(user));
        //    exc.Message.Should().Be(ErrorMessages.emailCannotBeNull);
        //}

        //[Theory]
        //[InlineData("", "lastname", "password")]
        //[InlineData("firstname", "", "password")]
        //[InlineData("firstname", "lastname", "")]
        //[InlineData(null, null, null)]
        //public async Task RegisterUserTest_WithNullProps_ExpectSpecificationException(string firstname, string lastname, string password)
        //{
        //    //Arrange
        //    var user = new Fixture().Create<RegisterUserRequestDto>();
        //    user.Firstname = firstname;
        //    user.Lastname = lastname;
        //    user.Password = password;

        //    var companyRepoMock = new Mock<ICompanyRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var authRepoMock = new Mock<IAuthenticationRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var inviteRepoMock = new Mock<IInviteRepository>();
        //    var internalNotifMock = new Mock<IInternalNotificationRepository>();

        //    var service = new AccountService(_loggerMock.Object, _mapper, userRepoMock.Object, eventRepoMock.Object, _configuration.Object, mailRepoMock.Object, inviteRepoMock.Object, orgaRepoMock.Object, internalNotifMock.Object);

        //    //Act / Assert
        //    var exc = await Assert.ThrowsAsync<SpecificationException>(async () => await service.RegisterUser(user));
        //    exc.Description.Should().Be(ErrorMessages.registerUserBadFormat);
        //}

        [Fact]
        public async Task GetUserByIdTest_NominalCase_ExpectUser()
        {
            //Arrange
            var user = new Fixture().Create<UserDto>();

            var companyRepoMock = new Mock<ICompanyRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var eventRepoMock = new Mock<IEventRepository>();
            var authRepoMock = new Mock<IAuthenticationRepository>();
            var mailRepoMock = new Mock<IMailRepository>();
            var orgaRepoMock = new Mock<IOrganizationRepository>();
            var inviteRepoMock = new Mock<IInviteRepository>();
            var internalNotifMock = new Mock<IInternalNotificationRepository>();

            userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(user.Id, null)).ReturnsAsync(user);
            var service = new AccountService(_loggerMock.Object, _mapper, userRepoMock.Object, eventRepoMock.Object, _configuration.Object, mailRepoMock.Object, inviteRepoMock.Object, orgaRepoMock.Object, internalNotifMock.Object);

            //Act 
            var result = await service.GetUserById(user.Id);

            //Assert
            result.Id.Should().Be(user.Id);
        }

        //[Fact]
        //public async Task UpdateUserPassword_NominalCase_ExpectPasswordUpdated()
        //{
        //    //Arrange
        //    var user = new Fixture().Create<UserDto>();
        //    user.Password = "newPassword";

        //    var token = TokenBuilder.Build()
        //            .WithKey("MTMzN2lsb3ZlYXBwbGVzeyhdQF4ifiNAXg==")
        //            .WithIssuer("http://trineapp.io")
        //            .WithExpiration(3600)
        //            .WithUserBasedClaims(user)
        //            .GetToken();

        //    var companyRepoMock = new Mock<ICompanyRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var authRepoMock = new Mock<IAuthenticationRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var inviteRepoMock = new Mock<IInviteRepository>();
        //    var internalNotifMock = new Mock<IInternalNotificationRepository>();

        //    var userModel = _mapper.Map<UserDto>(user);

        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(user.Id, It.IsAny<string>())).ReturnsAsync(userModel);
        //    userRepoMock.Setup(x => x.ApiUsersPutAsync(user.Id, _mapper.Map<UserDto>(user), It.IsAny<string>()));

        //    var service = new AccountService(_loggerMock.Object, _mapper, userRepoMock.Object,  eventRepoMock.Object, _configuration.Object, mailRepoMock.Object, inviteRepoMock.Object, orgaRepoMock.Object, internalNotifMock.Object);

        //    //Act 
        //    var email = await service.UpdateUserPassword(token);

        //    //Assert
        //    email.Should().Be(userModel.Mail);
        //}

        [Fact]
        public async Task UpdateUserPassword_WithEmptyClaimsToken_ExpectBusinessException()
        {
            //Arrange
            var user = new Fixture().Create<UserDto>();
            user.Password = "newPassword";

            var token = TokenBuilder.Build()
                    .WithKey("MTMzN2lsb3ZlYXBwbGVzeyhdQF4ifiNAXg==")
                    .WithIssuer("http://trineapp.io")
                    .WithExpiration(3600)
                    .GetToken();

            var companyRepoMock = new Mock<ICompanyRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var eventRepoMock = new Mock<IEventRepository>();
            var authRepoMock = new Mock<IAuthenticationRepository>();
            var mailRepoMock = new Mock<IMailRepository>();
            var orgaRepoMock = new Mock<IOrganizationRepository>();
            var inviteRepoMock = new Mock<IInviteRepository>();
            var internalNotifMock = new Mock<IInternalNotificationRepository>();

            userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(user.Id, It.IsAny<string>())).ReturnsAsync(_mapper.Map<UserDto>(user));
            userRepoMock.Setup(x => x.ApiUsersPutAsync(user.Id, _mapper.Map<UserDto>(user), It.IsAny<string>()));

            var service = new AccountService(_loggerMock.Object, _mapper, userRepoMock.Object, eventRepoMock.Object, _configuration.Object, mailRepoMock.Object, inviteRepoMock.Object, orgaRepoMock.Object, internalNotifMock.Object);

            //Act 
            var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.UpdateUserPassword(token));
            exc.Message.Should().Be(ErrorMessages.tokenError);
        }

        [Fact]
        public async Task UpdateUserPassword_WithBadTokenFormat_ExpectTechnicalException()
        {
            //Arrange
            var user = new Fixture().Create<UserDto>();
            user.Password = "newPassword";

            var token = TokenBuilder.Build()
                    .WithKey("MTMzN2lsb3ZlYXBwbGVzeyhdQF4ifiNAXg==")
                    .WithIssuer("http://trineapp.io")
                    .WithExpiration(3600)
                    .GetToken();
            token += ".wrong";

            var companyRepoMock = new Mock<ICompanyRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var eventRepoMock = new Mock<IEventRepository>();
            var authRepoMock = new Mock<IAuthenticationRepository>();
            var mailRepoMock = new Mock<IMailRepository>();
            var orgaRepoMock = new Mock<IOrganizationRepository>();
            var inviteRepoMock = new Mock<IInviteRepository>();
            var internalNotifMock = new Mock<IInternalNotificationRepository>();

            userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(user.Id, It.IsAny<string>())).ReturnsAsync(_mapper.Map<UserDto>(user));
            userRepoMock.Setup(x => x.ApiUsersPutAsync(user.Id, _mapper.Map<UserDto>(user), It.IsAny<string>()));

            var service = new AccountService(_loggerMock.Object, _mapper, userRepoMock.Object, eventRepoMock.Object, _configuration.Object, mailRepoMock.Object, inviteRepoMock.Object, orgaRepoMock.Object, internalNotifMock.Object);

            //Act 
            var exc = await Assert.ThrowsAsync<TechnicalException>(async () => await service.UpdateUserPassword(token));
            exc.Message.Should().Be(ErrorMessages.tokenUnsupported);
        }

        //[Fact]
        //public async Task UpdateUserPassword_WithUserEmailMismatch_ExpectBusinessException()
        //{
        //    //Arrange
        //    var user = new Fixture().Create<UserDto>();
        //    user.Password = "newPassword";
        //    var badUser = user;

        //    var token = TokenBuilder.Build()
        //            .WithKey("MTMzN2lsb3ZlYXBwbGVzeyhdQF4ifiNAXg==")
        //            .WithIssuer("http://trineapp.io")
        //            .WithExpiration(3600)
        //            .WithUserBasedClaims(user)
        //            .GetToken();

        //    badUser.Mail = "notthesameemail";

        //    var companyRepoMock = new Mock<ICompanyRepository>();
        //    var userRepoMock = new Mock<IUserRepository>();
        //    var eventRepoMock = new Mock<IEventRepository>();
        //    var authRepoMock = new Mock<IAuthenticationRepository>();
        //    var mailRepoMock = new Mock<IMailRepository>();
        //    var orgaRepoMock = new Mock<IOrganizationRepository>();
        //    var inviteRepoMock = new Mock<IInviteRepository>();
        //    var internalNotifMock = new Mock<IInternalNotificationRepository>();

        //    userRepoMock.Setup(x => x.ApiUsersByIdGetAsync(user.Id, It.IsAny<string>())).ReturnsAsync(_mapper.Map<UserDto>(badUser));
        //    userRepoMock.Setup(x => x.ApiUsersPutAsync(user.Id, _mapper.Map<UserDto>(user), It.IsAny<string>()));

        //    var service = new AccountService(_loggerMock.Object, _mapper, userRepoMock.Object,  eventRepoMock.Object, _configuration.Object, mailRepoMock.Object, inviteRepoMock.Object, orgaRepoMock.Object, internalNotifMock.Object);

        //    //Act 
        //    var exc = await Assert.ThrowsAsync<BusinessException>(async () => await service.UpdateUserPassword(token));
        //    exc.Message.Should().Be(ErrorMessages.tokenIdentityCompromised);
        //}

        #endregion

    }
}
