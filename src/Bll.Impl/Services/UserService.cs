using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Operational.Bll.Impl.Builders;
using Assistance.Operational.Bll.Impl.Helpers;
using Assistance.Operational.Bll.Services;
using Assistance.Operational.Dal.Repositories;
using AutoMapper;
using Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sogetrel.Sinapse.Framework.Bll.Services;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Bll.Impl.Services
{
    public class UserService : ServiceBase, IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IConfiguration _configuration;
        private readonly IMailRepository _mailRepository;
        private readonly IMissionRepository _missionRepository;
        private readonly IActivityRepository _activityRepository;

        private const string _userApiVersion = "1.0";
        private const string _hostUri = "https://app-assistance.azurewebsites.net";

        public UserService(ILogger<ServiceBase> logger, IMapper mapper, IUserRepository userRepository, IConfiguration configuration,
            IMailRepository mailRepository, IMissionRepository missionRepository, IActivityRepository activityRepository) : base(logger)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _configuration = configuration;
            _mailRepository = mailRepository;
            _missionRepository = missionRepository;
            _activityRepository = activityRepository;
        }

        public async Task<List<UserDto>> SearchUsers(string email = null, string firstname = null, string lastname = null, string companyName = null)
        {
            //Todo: Pourquoi il n'y a plus de CompanyName ? A verifier
            var results = await _userRepository.ApiUsersSearchGetAsync(email, firstname, lastname, _userApiVersion);

            if (results == null)
                return new List<UserDto>();

            var users = _mapper.Map<List<UserDto>>(results);
            return users;
        }

        public async Task<List<UserAdminDto>> GetAllDataUsers()
        {
            var results = new List<UserAdminDto>();
            var users = await _userRepository.ApiUsersGetAsync();
            var allMissions = await _missionRepository.ApiMissionsActiveByActiveGetAsync(true);
            var allActivities = await _activityRepository.ApiActivitiesGetAsync();

            users.ForEach(user =>
            {
                var missions = allMissions.Where(m => (m.Consultant != null && m.Consultant.Id == user.Id)
                || (m.Customer != null && m.Customer.Id == user.Id)
                || (m.Commercial != null && m.Commercial.Id == user.Id)).ToList();

                var activities = allActivities.Where(a => (a.Consultant != null && a.Consultant.Id == user.Id)
                || (a.Customer != null && a.Customer.Id == user.Id)
                || (a.Commercial != null && a.Commercial.Id == user.Id)).ToList();

                var token = TokenHelper.BuildToken(_configuration, user);
                user.Password = null;
                results.Add(new UserAdminDto()
                {
                    User = user,
                    Token = token,
                    Missions = missions,
                    Activities = activities
                });
            });

            return results;
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            var users = await _userRepository.ApiUsersGetAsync();

            if (users == null)
                return new List<UserDto>();

            users.ForEach(user =>
            {
                user.Password = null;
            });

            return users;
        }

        public async Task DeleteUser(string id)
        {
            await _userRepository.ApiUsersByIdDeleteAsync(id);
        }

        public async Task<TokenDto> RequestToken(string userId)
        {
            var user = await _userRepository.ApiUsersByIdGetAsync(userId);
            if (user is null)
            {
                throw new BusinessException("User not found");
            }

            return TokenHelper.BuildToken(_configuration, user);
        }

        public async Task UpdateUserAsync(string id, UserDto user)
        {
            await _userRepository.ApiUsersPutAsync(id, user);
        }

        public async Task UpdateRequiredData(string id, string password, string phoneNumber)
        {
            var user = await _userRepository.ApiUsersByIdGetAsync(id);
            user.PhoneNumber = phoneNumber;
            user.Password = password;
            await _userRepository.ApiUsersPutAsync(id, user);
        }

        public async Task<bool> UpdatePassword(string userId, string newPassword)
        {
            try
            {
                var user = await _userRepository.ApiUsersByIdGetAsync(userId, _userApiVersion);
                if (user is null)
                {
                    throw new BusinessException("User not found");
                }

                var exp = _configuration.GetSection("PasswordRecoveryToken")["Expiration"];
                long.TryParse(exp, out long expiration);

                // If the user exists, we send a token by mail
                user.Password = newPassword;
                var token = TokenBuilder.Build()
                    .WithKey(_configuration.GetSection("PasswordRecoveryToken")["Key"])
                    .WithIssuer(_configuration.GetSection("PasswordRecoveryToken")["Issuer"])
                    .WithExpiration(expiration)
                    .WithUserBasedClaims(user)
                    .GetToken();

                var templateId = _configuration["Mail:Templates:PasswordRecovery"];

                var dynamicTemplateData = new
                {
                    recoverPasswordUri = $"{_hostUri}/api/accounts/users/password/{token}",
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

        public async Task<string> CreateUser(ExternalUserDto dto)
        {
            // Creating the user
            var user = await _userRepository.ApiUsersPostAsync(_mapper.Map<UserDto>(dto));

            // Trying to sync previous data like pending activities
            var activities = await _activityRepository.GetByEmail(dto.Email);
            if (activities != null && activities.Any())
            {
                // If we find some activities waiting for this user to subscribe, we update them with the new user data
                activities.ForEach(async x =>
                {
                    if (x.Customer.Email == dto.Email)
                        x.Customer = _mapper.Map<UserActivityDto>(user);

                    if (x.Consultant.Email == dto.Email)
                        x.Consultant = _mapper.Map<UserActivityDto>(user);

                    await _activityRepository.ApiActivitiesByIdPatchAsync(x.Id, x);
                });
            }

            return user.Id;
        }
    }
}
