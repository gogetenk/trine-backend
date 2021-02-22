using System;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Event.Dal.Repositories;
using Assistance.Operational.Bll.Impl.Builders;
using Assistance.Operational.Bll.Impl.Errors;
using Assistance.Operational.Bll.Impl.Helpers;
using Assistance.Operational.Bll.Impl.Specifications;
using Assistance.Operational.Bll.Services;
using Assistance.Operational.Dal.Repositories;
using Assistance.Operational.Model;
using AutoMapper;
using Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sogetrel.Sinapse.Framework.Bll.Services;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Bll.Impl.Services
{
    public class AccountService : ServiceBase, IAccountService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IMailRepository _mailRepository;
        private readonly IInviteRepository _inviteRepository;
        private readonly IOrganizationRepository _OrganizationDtoRepository;
        private readonly IInternalNotificationRepository _internalNotificationRepository;
        private readonly IConfiguration _configuration;

        // TODO : Mettre ailleurs !
        private const string _recoverPasswordUri = "https://app-assistance.azurewebsites.net";
        private const string _recoverPasswordEndpoint = "api/accounts/users/password";
        private const string _eventApiVersion = "1.0";
        private const string _userApiVersion = "1.0";
        private const string _authenticationApiVersion = "1.0";


        public AccountService(
            ILogger<ServiceBase> logger,
            IMapper mapper,
            IUserRepository userRepository,
            IEventRepository eventRepository,
            IConfiguration configuration,
            IMailRepository mailRepository,
            IInviteRepository inviteRepository,
            IOrganizationRepository OrganizationDtoRepository,
            IInternalNotificationRepository internalNotificationRepository) : base(logger)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _mailRepository = mailRepository;
            _inviteRepository = inviteRepository;
            _OrganizationDtoRepository = OrganizationDtoRepository;
            _internalNotificationRepository = internalNotificationRepository;
            _configuration = configuration;
        }

        public async Task<TokenDto> AuthenticateUser(UserCredentialsDto login)
        {
            var user = await GetUserByCredentials(login);
            if (user is null)
                return null;

            user.LastLoginDate = DateTime.UtcNow;
            await _userRepository.ApiUsersPutAsync(user.Id, user);
            _internalNotificationRepository.SendAsync($"👋🏼 {user.Firstname} {user.Lastname} vient de se connecter. ({DateTime.UtcNow.ToString()})");
            var token = TokenHelper.BuildToken(_configuration, user);
            return token;
        }

        public async Task<string> RegisterCompany(RegisterCompanyRequestDto dto)
        {
            //    if (string.IsNullOrEmpty(dto.Siret))
            //        throw new BusinessException(ErrorMessages.siretCannotBeNull);

            //    // Verify company doesn't exist
            //    if (await CheckSiret(dto.Siret) != null)
            //        throw new BusinessException(ErrorMessages.companyAlreadyExistErrorMessage);

            //    var spec = new ValidateCompanyRegistrationRequestSpecification();
            //    if (!spec.IsSatisfiedBy(dto))
            //        throw spec.GetErrors(dto);

            //    var companyToCreate = _mapper.Map<Company>(dto);
            //    var id = await _companyRepository.ApiCompaniesPostAsync(companyToCreate, _userApiVersion);

            //    if (string.IsNullOrEmpty(id))
            //        throw new BusinessException(ErrorMessages.companyAlreadyExistErrorMessage);

            //    // Linking user to company
            //    var user = await _userRepository.ApiUsersByIdGetAsync(dto.UserId, _userApiVersion);
            //    if (user.Company != null)
            //        return id;

            //    user.Company = _mapper.Map<CompanyDto>(companyToCreate);
            //    await _userRepository.ApiUsersPutAsync(user.Id, user, _userApiVersion);

            //    var createdCompanyEvent = new EventModel()
            //    {
            //        Created = DateTime.UtcNow,
            //        EventType = EventModel.EventTypeEnum.ACTION,
            //        ContextId = companyToCreate.Id,
            //        Title = $"Company {companyToCreate.Name} created."
            //    };
            //    await _eventRepository.ApiEventsPostAsync(_mapper.Map<EventDto>(createdCompanyEvent), _eventApiVersion);

            //    return id;
            return null;
        }

        public async Task<CreatedUserDto> RegisterUser(RegisterUserRequestDto request)
        {
            //UserDto userToCreate = await CheckRequestAndPrepareUser(request);
            //var id = (await _userRepository.ApiUsersPostAsync(userToCreate, _userApiVersion)).Id;
            //var createdAccount = new CreatedUserDto();

            //// We log the new user on so he doesn't have to get a token by himself afterward
            //var login = new UserCredentials(userToCreate.Email, userToCreate.Password);
            //var token = await AuthenticateUser(_mapper.Map<UserCredentialsDto>(login));
            //createdAccount.Token = _mapper.Map<TokenDto>(token);
            //await CreateEventAndSendEmail(userToCreate, id);
            //return createdAccount;
            return null;
        }

        private async Task SendBackOfficeMailAndNotification(string subject, string content)
        {
            var isMailSuccess = _mailRepository.SendAsync(_configuration["Mail:DefaultAddress"], null, null, subject, content, null);
            var isNotificationSuccess = _internalNotificationRepository.SendAsync($"{subject} \\n {content}");
            await Task.WhenAll(isMailSuccess, isNotificationSuccess);

            if (!isMailSuccess.Result || isMailSuccess.IsCanceled)
                Logger.LogCritical("An error occured while sending the email");
            if (isNotificationSuccess.IsCanceled)
                Logger.LogCritical("An error occured while sending the notification", isNotificationSuccess.Exception);
        }

        public async Task<CompanyDto> CheckSiret(string siret)
        {
            //var result = await _companyRepository.ApiCompaniesSearchGetAsync(siret);
            //return _mapper.Map<CompanyDto>(result);
            return null;
        }

        public async Task<UserDto> CheckMail(string email)
        {
            var results = await _userRepository.ApiUsersSearchGetAsync(email: email);
            if (results == null || !results.Any())
                return null;

            return _mapper.Map<UserDto>(results.FirstOrDefault());
        }

        public async Task<UserDto> GetUserById(string id)
        {
            var user = _mapper.Map<UserDto>(await _userRepository.ApiUsersByIdGetAsync(id));
            return user;
        }

        public async Task<bool> SendPasswordRecoveryEmail(ForgotPasswordDto dto)
        {
            // Trying to get the user
            try
            {
                var user = await _userRepository.GetByEmailAndPassword(dto.Email, dto.OldPassword);
                if (user is null)
                    throw new BusinessException("User not found");

                var exp = _configuration.GetSection("PasswordRecoveryToken")["Expiration"];
                long.TryParse(exp, out long expiration);

                // If the user exists, we send a token by mail
                user.Password = dto.NewPassword;
                var token = TokenBuilder.Build()
                    .WithKey(_configuration.GetSection("PasswordRecoveryToken")["Key"])
                    .WithIssuer(_configuration.GetSection("PasswordRecoveryToken")["Issuer"])
                    .WithExpiration(expiration)
                    .WithUserBasedClaims(user)
                    .GetToken();

                var templateId = _configuration["Mail:Templates:PasswordRecovery"];

                var dynamicTemplateData = new
                {
                    recoverPasswordUri = $"{_recoverPasswordUri}/{_recoverPasswordEndpoint}/{token}",
                    user = new
                    {
                        firstName = user.Firstname,
                        lastName = user.Lastname
                    }
                };

                var isSuccess = await _mailRepository.SendAsync(user.Email, templateId: templateId, templateData: dynamicTemplateData);
                return isSuccess;
            }
            catch
            {
                return false;
            }
        }

        public async Task SendEmailInvitations(SubscriptionInvitationDto request, string userId)
        {
            if (!request.Emails.Any())
                return;

            var inviter = await _userRepository.ApiUsersByIdGetAsync(userId);
            var subject = $"-- **Nouvelle invitation**";
            var content = $"**Administrateur :** {inviter.Firstname} {inviter.Lastname} \\n **Id :** {inviter.Id} \\n **Emails :** {string.Join($", \\n ", request.Emails)}";

            await SendBackOfficeMailAndNotification(subject, content);
        }

        public async Task<string> UpdateUserPassword(string token)
        {
            var reader = new TokenReader();
            if (!reader.Verify(token))
                throw new TechnicalException(ErrorMessages.tokenUnsupported);

            var claims = reader.GetClaims(token);
            var userId = claims.FirstOrDefault(x => x.Type == "Id")?.Value;
            var userEmail = claims.FirstOrDefault(x => x.Type == "Email")?.Value;
            var newPassword = claims.FirstOrDefault(x => x.Type == "NewPassword")?.Value;
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(newPassword))
                throw new BusinessException(ErrorMessages.tokenError);

            // Checking the mail and the id
            var user = _mapper.Map<UserModel>(await _userRepository.ApiUsersByIdGetAsync(userId, _userApiVersion));
            if (user is null || user.Email != userEmail)
                throw new BusinessException(ErrorMessages.tokenIdentityCompromised);

            user.Password = newPassword;
            // Updating the user's password
            await _userRepository.ApiUsersPutAsync(user.Id, _mapper.Map<UserDto>(user), _userApiVersion);

            return user.Email;
        }

        #region Private methods

        private async Task<UserDto> CheckRequestAndPrepareUser(RegisterUserRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email))
                throw new BusinessException(ErrorMessages.emailCannotBeNull);

            // Verify user doesn't exist
            if ((await CheckMail(dto.Email)) != null)
                throw new BusinessException(ErrorMessages.userAlreadyExistErrorMessage);

            var spec = new ValidateUserRegistrationRequestSpecification();
            if (!spec.IsSatisfiedBy(dto))
                throw spec.GetErrors(dto);

            var userToCreate = new UserDto
            {
                Email = dto.Email,
                Firstname = dto.Firstname,
                Lastname = dto.Lastname,
                Password = dto.Password,
                GlobalRole = (UserDto.GlobalRoleEnum)dto.Role
            };
            return userToCreate;
        }

        private async Task CreateEventAndSendEmail(UserDto userToCreate, string id)
        {
            // Events and mail sending
            var createdUserEvent = new EventModel()
            {
                Created = DateTime.UtcNow,
                EventType = EventModel.EventTypeEnum.ACTION,
                ContextId = id,
                Title = $"User {userToCreate.Firstname} {userToCreate.Lastname.ToUpper()} registered."
            };
            var sendNotificationTask = _eventRepository.ApiEventsPostAsync(_mapper.Map<EventDto>(createdUserEvent), _eventApiVersion);
            var templateId = _configuration["Mail:Templates:AccountCreation"];
            var templateData = new { link = $"{_configuration["WebApp:Url"]}/app" };
            var sendMailTask = _mailRepository.SendAsync(userToCreate.Email, templateId, templateData);
            await Task.WhenAll(sendNotificationTask, sendMailTask);
        }

        public async Task<UserDto> GetUserByCredentials(UserCredentialsDto userCredentials)
        {
            if (string.IsNullOrEmpty(userCredentials.Email) && string.IsNullOrEmpty(userCredentials.Password))
                throw new BusinessException("The credentials are badly formated");

            return await _userRepository.GetByEmailAndPassword(userCredentials.Email, userCredentials.Password);
        }



        #endregion
    }
}
