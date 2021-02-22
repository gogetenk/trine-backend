using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assistance.Event.Dal.Repositories;
using Assistance.Operational.Bll.Impl.Builders;
using Assistance.Operational.Bll.Impl.Errors;
using Assistance.Operational.Bll.Impl.Helpers;
using Assistance.Operational.Bll.Impl.Text;
using Assistance.Operational.Bll.Services;
using Assistance.Operational.Dal.Repositories;
using Assistance.Operational.Model;
using AutoMapper;
using Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Sogetrel.Sinapse.Framework.Bll.Services;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Bll.Impl.Services
{
    public class ActivityService : ServiceBase, IActivityService
    {
        private readonly IMapper _mapper;
        private readonly IActivityRepository _activityRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IMissionRepository _missionRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IMailRepository _mailRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageRepository _azureStorageRepository;
        private readonly IInternalNotificationRepository _internalNotificationRepository;

        private const string _activityApiVersion = "1.0";
        private const string _userApiVersion = "1.0";
        private const string _missionApiVersion = "1.0";
        private const string _organizationApiVersion = "1.0";

        public ActivityService(
            ILogger<ServiceBase> logger,
            IMapper mapper,
            IActivityRepository activityRepository,
            IUserRepository userRepository,
            IEventRepository eventRepository,
            IMissionRepository missionRepository,
            IOrganizationRepository organizationRepository,
            IMailRepository mailRepository,
            INotificationRepository notificationRepository,
            IConfiguration configuration,
            IAzureStorageRepository azureStorageRepository,
            IInternalNotificationRepository internalNotificationRepository
            ) : base(logger)
        {
            _mapper = mapper;
            _activityRepository = activityRepository;
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _missionRepository = missionRepository;
            _organizationRepository = organizationRepository;
            _mailRepository = mailRepository;
            _notificationRepository = notificationRepository;
            _configuration = configuration;
            _azureStorageRepository = azureStorageRepository;
            _internalNotificationRepository = internalNotificationRepository;
        }

        public async Task<List<ActivityDto>> GetFromMission(string userId, string missionId)
        {
            //    var activities = await _activityRepository.ApiActivitiesMissionsByMissionIdGetAsync(missionId, _activityApiVersion);
            //    var activitiesDto = _mapper.Map<List<ActivityDto>>(activities);

            //    var results = new List<ActivityDto>();
            //    activitiesDto.ForEach(activityDto =>
            //    {
            //        if (activityDto.Consultant.Id == userId)
            //        {
            //            activityDto.CanModify = (activityDto.Status == ActivityStatusEnum.Generated);
            //            activityDto.CanSign = (activityDto.Status == ActivityStatusEnum.Generated);
            //        }
            //        else if (activityDto.Customer.Id == userId)
            //        {
            //            activityDto.CanModify = false;
            //            activityDto.CanSign = (activityDto.Status == ActivityStatusEnum.ConsultantSigned && activityDto.Consultant.SignatureDate.HasValue);
            //        }
            //        else
            //        {
            //            activityDto.CanModify = false;
            //            activityDto.CanSign = false;
            //        }
            //        results.Add(activityDto);
            //    });
            //    return results;
            return null;
        }

        public async Task<List<ActivityDto>> GetFromUser(string userId)
        {
            var activities = await _activityRepository.ApiActivitiesUsersByUserIdGetAsync(userId, _activityApiVersion);
            var activitiesDto = _mapper.Map<List<ActivityDto>>(activities);
            var results = new List<ActivityDto>();
            activitiesDto.ForEach(activityDto =>
            {
                if (activityDto.Customer != null)
                    activityDto.Customer.CanSign = activityDto.Status == ActivityStatusEnum.ConsultantSigned && activityDto.Customer.Id == userId;
                if (activityDto.Consultant != null)
                    activityDto.Consultant.CanSign = activityDto.Status == ActivityStatusEnum.Generated && activityDto.Consultant.Id == userId;

                results.Add(activityDto);
            });
            return results.OrderByDescending(x => x.StartDate).ToList();
        }

        public async Task<ActivityDto> Generate()
        {
            var startDt = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime lastDayOfMonth = startDt.AddMonths(1).AddDays(-1);
            var generatedActivity = _mapper.Map<ActivityDto>(await _activityRepository.ApiActivitiesByStartDateendDateGetAsync(startDt, lastDayOfMonth, _activityApiVersion));
            generatedActivity.Status = ActivityStatusEnum.Generated;
            return generatedActivity;
        }

        public async Task<ActivityDto> CreateActivityFromMissionAndPeriod(string missionId, DateTime period)
        {
            // Getting the days included in the activity
            var startDt = new DateTime(period.Year, period.Month, 1);
            DateTime lastDayOfMonth = startDt.AddMonths(1).AddDays(-1);
            var generatedActivity = _mapper.Map<ActivityDto>(await _activityRepository.ApiActivitiesByStartDateendDateGetAsync(startDt, lastDayOfMonth, _activityApiVersion));
            if (generatedActivity is null)
                throw new BusinessException($"The activity grid could not be created from period {period.ToString()}.");

            generatedActivity.Status = ActivityStatusEnum.Generated;
            generatedActivity.MissionId = missionId;

            // Creating the consolidated entity
            var mission = _mapper.Map<MissionDto>(await _missionRepository.ApiMissionsByIdGetAsync(missionId, _missionApiVersion));
            if (mission is null)
                throw new BusinessException($"The mission {missionId} could not be found.");

            if (mission.Customer != null)
            {
                generatedActivity.Customer = CreateUserActivity(mission.Customer);
            }
            if (mission.Commercial != null)
            {
                generatedActivity.Commercial = CreateUserActivity(mission.Commercial);
            }
            if (mission.Consultant != null)
            {
                generatedActivity.Consultant = CreateUserActivity(mission.Consultant);
            }

            var createdActivity = _mapper.Map<ActivityDto>(await _activityRepository.ApiActivitiesPostAsync(_mapper.Map<ActivityDto>(generatedActivity), _activityApiVersion));
            if (createdActivity is null)
                throw new BusinessException("An error occured while creating this activity.");

            return createdActivity;
        }

        public async Task<ActivityDto> CreateActivityFromEmailAndPeriod(string currentUserId, string customerEmail, DateTime period)
        {
            // Getting the days included in the activity
            var startDt = new DateTime(period.Year, period.Month, 1);
            DateTime lastDayOfMonth = startDt.AddMonths(1).AddDays(-1);
            var generatedActivity = _mapper.Map<ActivityDto>(await _activityRepository.ApiActivitiesByStartDateendDateGetAsync(startDt, lastDayOfMonth, _activityApiVersion));
            if (generatedActivity is null)
                throw new BusinessException($"The activity grid could not be created from period {period.ToString()}.");

            generatedActivity.Status = ActivityStatusEnum.Generated;

            // Getting the customer if he is already a trine user
            var customer = await _userRepository.GetByEmail(customerEmail);
            if (customer != null)
                generatedActivity.Customer = CreateUserActivity(_mapper.Map<UserMissionDto>(customer));
            else // If the customer is not subscribed yet, we create a dummy user with his email
                generatedActivity.Customer = new UserActivityDto()
                {
                    Email = customerEmail,
                    ProfilePicUrl = "http://www.gravatar.com/avatar/?d=identicon"
                };

            // Attaching consultant to the activity
            var consultant = await _userRepository.ApiUsersByIdGetAsync(currentUserId);
            generatedActivity.Consultant = _mapper.Map<UserActivityDto>(consultant);

            var createdActivity = _mapper.Map<ActivityDto>(await _activityRepository.ApiActivitiesPostAsync(_mapper.Map<ActivityDto>(generatedActivity), _activityApiVersion));
            if (createdActivity is null)
                throw new BusinessException("An error occured while creating this activity.");

            return createdActivity;
        }

        public async Task<ActivityDto> CreateActivityFromPeriod(string currentUserId, DateTime period)
        {
            // Getting the days included in the activity
            var startDt = new DateTime(period.Year, period.Month, 1);
            DateTime lastDayOfMonth = startDt.AddMonths(1).AddDays(-1);
            var generatedActivity = _mapper.Map<ActivityDto>(await _activityRepository.ApiActivitiesByStartDateendDateGetAsync(startDt, lastDayOfMonth, _activityApiVersion));
            if (generatedActivity is null)
                throw new BusinessException($"The activity grid could not be created from period {period.ToString()}.");

            generatedActivity.Status = ActivityStatusEnum.Generated;

            // Attaching consultant to the activity
            var consultant = await _userRepository.ApiUsersByIdGetAsync(currentUserId);
            generatedActivity.Consultant = _mapper.Map<UserActivityDto>(consultant);
            generatedActivity.Consultant.CanSign = true;

            var createdActivity = _mapper.Map<ActivityDto>(await _activityRepository.ApiActivitiesPostAsync(_mapper.Map<ActivityDto>(generatedActivity), _activityApiVersion));
            if (createdActivity is null)
                throw new BusinessException("An error occured while creating this activity.");

            return createdActivity;
        }

        public async Task<ActivityDto> SaveActivity(ActivityDto activity)
        {
            //    // Saving the activity
            //    var customer = await _userRepository.GetByEmail(customerEmail);
            //    if (customer != null)
            //        generatedActivity.Customer = CreateUserActivity(_mapper.Map<UserMissionDto>(customer));
            //    else // If the customer is not subscribed yet, we create a dummy user with his email
            //        generatedActivity.Customer = new UserActivityDto()
            //        {
            //            Email = customerEmail,
            //            ProfilePicUrl = "http://www.gravatar.com/avatar/?d=identicon"
            //        };

            //    // Attaching consultant to the activity
            //    var consultant = await _userRepository.ApiUsersByIdGetAsync(currentUserId);
            //    generatedActivity.Consultant = _mapper.Map<UserActivityDto>(consultant);

            //    var createdActivity = _mapper.Map<ActivityDto>(await _activityRepository.ApiActivitiesPostAsync(_mapper.Map<ActivityDto>(generatedActivity), _activityApiVersion));
            //    if (createdActivity is null)
            //        throw new BusinessException("An error occured while creating this activity.");

            //    return createdActivity;
            return null;
        }

        public async Task<ActivityDto> ShareActivityWithEmail(string currentUserId, string activityId, string customerEmail)
        {
            // Getting the days included in the activity
            var activity = _mapper.Map<ActivityDto>(await _activityRepository.ApiActivitiesByIdGetAsync(activityId));
            if (activity is null)
                throw new BusinessException($"Activity not found");

            // Verifying that the current user is the consultant 
            if (currentUserId != activity.Consultant.Id)
                throw new BusinessException("You have to be the consultant in order to share your activity report.");

            // Getting the customer if he is already a trine user
            var customer = await _userRepository.GetByEmail(customerEmail);

            object emailContentTemplate;

            if (customer != null)
            {
                activity.Customer = CreateUserActivity(_mapper.Map<UserMissionDto>(customer));
                activity.Customer.CanSign = true;

                // Then we update the activity
                activity = _mapper.Map<ActivityDto>(await _activityRepository.ApiActivitiesByIdPatchAsync(activity.Id, activity));
                if (activity is null)
                    throw new BusinessException("An error occured while updating this activity.");

                // If the customer is already subscribed, we just send him a regular notification email (without token)
                var consultantNameToShow = string.IsNullOrEmpty(activity.Consultant.Firstname) ? $"{activity.Consultant.Email}" : $"{activity.Consultant.Firstname} {activity.Consultant.Lastname.ToUpper()}";

                emailContentTemplate = new
                {
                    period = activity.CreationDate.ToString("MMMM yyyy"),
                    fullname = consultantNameToShow,
                    publicLink = $"{_configuration["WebApp:Url"]}{string.Format(_configuration["WebApp:SignActivityPath"], activity.Id)}"
                };

            }
            else
            {
                // If the customer is not subscribed yet, we create a dummy user with his email
                activity.Customer = new UserActivityDto()
                {
                    Email = customerEmail,
                    ProfilePicUrl = "http://www.gravatar.com/avatar/?d=identicon",
                    CanSign = true
                };

                // Then we update the activity
                activity = _mapper.Map<ActivityDto>(await _activityRepository.ApiActivitiesByIdPatchAsync(activity.Id, activity));
                if (activity is null)
                    throw new BusinessException("An error occured while updating this activity.");

                // And then we send an email to the customer
                long.TryParse(_configuration["AuthenticationSettings:TokenExpirationInSeconds"], out long exp);
                var token = new TokenBuilder()
                    .WithExpiration(exp)
                    .WithKey(_configuration["AuthenticationSettings:Key"])
                    .WithEmailClaim(customerEmail)
                    .WithActivityIdClaim(activity.Id)
                    .GetToken();

                var consultantNameToShowInMail = string.IsNullOrEmpty(activity.Consultant.Firstname) ? $"{activity.Consultant.Email}" : $"{activity.Consultant.Firstname} {activity.Consultant.Lastname.ToUpper()}";
                emailContentTemplate = new
                {
                    period = activity.CreationDate.ToString("MMMM yyyy", new CultureInfo("fr-FR")),
                    fullname = consultantNameToShowInMail,
                    publicLink = $"{_configuration["WebApp:Url"]}{string.Format(_configuration["WebApp:SignInvitationPath"], activity.Id, token)}"
                };
            }

            // Sending the email
            await _mailRepository.SendAsync(
                customerEmail,
                _configuration["Mail:Templates:ShareActivityInvitation"],
                emailContentTemplate,
                null,
                null,
                null);

            return activity;
        }

        private UserActivityDto CreateUserActivity(UserMissionDto user)
        {
            return new UserActivityDto()
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                ProfilePicUrl = user.ProfilePicUrl,
                Email = user.Email
            };
        }

        public async Task<byte[]> ExportActivity(string id)
        {
            var activity = await _activityRepository.ApiActivitiesByIdGetAsync(id, _activityApiVersion);
            if (activity == null)
                throw new BusinessException(ErrorMessages.activityDoesntExist);

            //if (activity.Status != ActivityStatusEnum.CustomerSigned)
            //    throw new BusinessException("The activity has to be signed by customer and consultant before exporting.");

            //if (activity.Customer is null || activity.Consultant is null)
            //    throw new BusinessException("Error : the activity should have a customer and a consultant attached.");

            return ExcelGenerator.GenerateCommonTemplate(activity);
        }

        //public async Task<ActivityDto> Create(CreateActivityRequestDto dto)
        //{
        //    var mission = await _missionRepository.ApiMissionsByIdGetAsync(dto.MissionId);
        //    if (mission is null)
        //    {
        //        throw new BusinessException($"Mission {dto.MissionId} does not exists");
        //    }

        //    //TODO: hotfix a delete, juste pour trouver une solution pour mongo qui ne comprend pas l'UTC
        //    if (dto.Days != null)
        //    {
        //        List<GridDayDto> days = new List<GridDayDto>();
        //        foreach (var day in dto.Days)
        //        {
        //            DateTime utcDay = day.Day;
        //            utcDay = DateTime.SpecifyKind(day.Day, DateTimeKind.Utc).AddHours(12);
        //            day.Day = utcDay;
        //            days.Add(day);
        //        }
        //        dto.Days = days;
        //    }


        //        var missionDto = _mapper.Map<MissionDto>(mission);

        //        var activityDto = new ActivityDto()
        //        {
        //            MissionId = dto.MissionId,
        //            Days = dto.Days,
        //            CreationDate = DateTime.UtcNow,
        //            Status = ActivityStatusEnum.Generated
        //        };

        //        activityDto.Days = dto.Days;
        //            var firstValue = dto.Days.First();
        //        var lastValue = dto.Days.Last();
        //            if (firstValue is null || lastValue is null)
        //            {
        //                throw new BusinessException("days enumeration has incorrect values");
        //    }

        //    activityDto.StartDate = firstValue.Day;
        //            activityDto.EndDate = lastValue.Day;

        //            if (missionDto.Consultant != null)
        //            {
        //                var consultant = await _userRepository.ApiUsersByIdGetAsync(missionDto.Consultant.Id);
        //    activityDto.Consultant = new UserActivityDto()
        //    {
        //        Id = missionDto.Consultant.Id,
        //                    Firstname = consultant.Firstname,
        //                    Lastname = consultant.Lastname,
        //                    ProfilePicUrl = consultant.ProfilePicUrl
        //                };
        //}

        //            if (missionDto.Commercial != null)
        //            {
        //                var commercial = await _userRepository.ApiUsersByIdGetAsync(missionDto.Commercial.Id);
        //activityDto.Commercial = new UserActivityDto()
        //{
        //    Id = missionDto.Commercial.Id,
        //                    Firstname = commercial.Firstname,
        //                    Lastname = commercial.Lastname,
        //                    ProfilePicUrl = commercial.ProfilePicUrl
        //                };
        //            }

        //            if (missionDto.Customer != null)
        //            {
        //                var customer = await _userRepository.ApiUsersByIdGetAsync(missionDto.Customer.Id);
        //activityDto.Customer = new UserActivityDto()
        //{
        //    Id = missionDto.Customer.Id,
        //                    Firstname = customer.Firstname,
        //                    Lastname = customer.Lastname,
        //                    ProfilePicUrl = customer.ProfilePicUrl
        //                };
        //            }

        //            var newActivity = await _activityRepository.ApiActivitiesPostAsync(_mapper.Map<ActivityDto>(activityDto), _activityApiVersion);
        //            return _mapper.Map<ActivityDto>(newActivity);
        //        }

        public async Task DeleteActivity(string id)
        {
            await _activityRepository.ApiActivitiesByIdDeleteAsync(id, _activityApiVersion);
        }

        public async Task<ActivityDto> GetFromMissionAndMonth(string userId, string missionId, DateTime date)
        {
            //    var mission = await _missionRepository.ApiMissionsByIdGetAsync(missionId, _missionApiVersion);
            //    var activity = await _activityRepository.ApiActivitiesMissionsByMissionIdByDateGetAsync(missionId, date, _activityApiVersion);

            //    if (mission.EndDate > DateTime.UtcNow && activity is null)
            //    {
            //        activity = await CreateActivityFromMissionAndPeriod(missionId, date);
            //    }

            //    var activityDto = _mapper.Map<ActivityDto>(activity);
            //    if (activityDto != null)
            //    {
            //        if (mission.Consultant.Id == userId)
            //        {
            //            activityDto.CanModify = (activityDto.Status == ActivityStatusEnum.Generated || activityDto.Status == ActivityStatusEnum.ModificationsRequired);
            //            activityDto.CanSign = (activityDto.Status == ActivityStatusEnum.Generated || activityDto.Status == ActivityStatusEnum.ModificationsRequired);
            //        }
            //        else if (mission.Customer.Id == userId)
            //        {
            //            activityDto.CanModify = false;
            //            activityDto.CanSign = (activityDto.Status == ActivityStatusEnum.ConsultantSigned && activityDto.Consultant.SignatureDate.HasValue);
            //        }
            //        else
            //        {
            //            activityDto.CanModify = false;
            //            activityDto.CanSign = false;
            //        }
            //    }
            //    return activityDto;
            return null;
        }

        public async Task<ActivityDto> SignActivityReport(string activityId, string userId, string extension = "", string contentType = "", byte[] file = null, string signatureUri = "")
        {
            var activityTask = _activityRepository.ApiActivitiesByIdGetAsync(activityId, _activityApiVersion);
            var userTask = _userRepository.ApiUsersByIdGetAsync(userId, _userApiVersion);
            var activity = _mapper.Map<ActivityDto>(await activityTask);
            if (activity == null)
                throw new BusinessException(ErrorMessages.activityDoesntExist);

            // First, we update the customer infos if needed (he may have subscribed recently)
            activity = await CheckAndUpdateCustomer(activity);

            var user = _mapper.Map<UserDto>(await userTask);
            if (user == null)
                throw new BusinessException(ErrorMessages.userDoesntExist);

            if (activity.Commercial != null && user.Id == activity.Commercial.Id && activity.Status == ActivityStatusEnum.CustomerSigned)
            {
                activity = await SignAsCommercial(activity, user);
            }
            else if (activity.Consultant != null && user.Id == activity.Consultant.Id && (activity.Status == ActivityStatusEnum.Generated || activity.Status == ActivityStatusEnum.ModificationsRequired))
            {
                activity = await SignAsConsultant(activity, user, extension, contentType, file, signatureUri);
            }
            else if (activity.Customer != null && user.Id == activity.Customer.Id && activity.Status == ActivityStatusEnum.ConsultantSigned)
            {
                activity = await SignAsCustomer(activity, user, extension, contentType, file, signatureUri);
            }
            else
            {
                throw new BusinessException(ErrorMessages.userCannotSignYet);
            }

            return activity;
        }

        /// <summary>
        /// For onboarding : creating and signing the activity at the same time
        /// </summary>
        public async Task<ActivityDto> CreateAndSignActivity(ActivityDto activity, string currentUserId)
        {
            // Setting signed state
            activity.Status = ActivityStatusEnum.ConsultantSigned;
            activity.CreationDate = DateTime.UtcNow;
            activity.StartDate = activity.StartDate.ToUniversalTime();
            activity.EndDate = activity.EndDate.ToUniversalTime();

            // Attaching consultant (which just log on / registered)
            var consultant = await _userRepository.ApiUsersByIdGetAsync(currentUserId);
            if (consultant is null)
                throw new BusinessException("The user doesn't exist");

            activity.Consultant = new UserActivityDto()
            {
                Id = currentUserId,
                Firstname = consultant.Firstname,
                Lastname = consultant.Lastname,
                ProfilePicUrl = consultant.ProfilePicUrl,
                Email = consultant.Email,
                SignatureDate = DateTime.UtcNow,
                LastUpdateDate = DateTime.UtcNow,
            };

            // Save the activity in db
            var updated = await _activityRepository.ApiActivitiesPostAsync(activity);
            return updated;
        }

        public async Task CreateFillActivityNotifications()
        {
            // Get all active missions of all organizations
            var missions = await _missionRepository.ApiMissionsActiveByActiveGetAsync(true);
            if (missions is null || !missions.Any())
                return;

            // Triple : Mission Id, Consultant Id, Days worked
            List<Tuple<string, string, float>> missionsAndConsultantAndWork = missions.Select(x => new Tuple<string, string, float>(x.Id, x.Consultant.Id, 0f)).ToList();
            if (missionsAndConsultantAndWork is null || !missionsAndConsultantAndWork.Any())
                return;

            // Check their activities to know how much they worked this month on each mission.
            // If they didn't specify their work, we let them navigate to the create activity page by themselves (using action button of the notification)
            var consultantsIds = missionsAndConsultantAndWork.Select(x => x.Item2).Distinct().ToList();
            var activities = await _activityRepository.ApiActivitiesUsersByUserIdsByDateGetAsync(consultantsIds, DateTime.UtcNow, _activityApiVersion);
            var activitiesDto = _mapper.Map<List<ActivityDto>>(activities);
            if (activitiesDto is null || !activitiesDto.Any())
                return;

            var notifTitle = "C'est la fin du mois !";
            var notifContent = $"Il faut penser à remplir son CRA ! Vous avez au moins une mission qui nécessite votre attention.";

            List<Task> notificationTasks = new List<Task>();

            // Send the message so that all template registrations that contain "messageParam"
            // receive the notifications. This includes APNS, GCM, WNS, and MPNS template registrations.
            Dictionary<string, string> templateParams = new Dictionary<string, string>();
            templateParams["messageParam"] = notifTitle;
            templateParams["messageContent"] = notifContent;
            var sendPushTask = _notificationRepository.SendTemplatedNotification(templateParams, consultantsIds);
            notificationTasks.Add(sendPushTask);

            foreach (var activity in activitiesDto)
            {
                if (activity.Days is null || activity.Consultant is null || !activity.Days.Any())
                    break;

                //if (!missionsAndConsultantAndWork.Contains(activity.ConsultantId))
                //    break;

                var workedDays = CalculateWorkedDays(activity.Days);

                //foreach(var tuple in missionsAndConsultantAndWork)
                //{

                //}

                //missionsAndConsultantAndWork.Where(x => x.Item2 == activity.ConsultantId && x.Item1 == activity.MissionId).ToList().ForEach(x => x.Item3 = workedDays);

                //Create all the "action" notification events based on : user, days worked, mission
                EventModel notification = new EventModel()
                {
                    Created = DateTime.UtcNow,
                    EventType = EventModel.EventTypeEnum.ACTION,
                    Title = notifTitle,
                    Details = workedDays,
                    Subtitle = $"{UppercaseFirst(DateTime.UtcNow.ToString("MMMM yyyy"))}",
                    MainContent = notifContent,
                    ContextId = activity.MissionId,
                    UserId = activity.Consultant.Id,
                    TargetId = activity.Consultant.Id,
                    ContextType = EventModel.ContextEnum.ACTIVITY,
                    IsEnabled = true
                };

                // TODO : implémenter les mails de notif de CRA (juste créer template SendGrid)
                //notificationTasks.Add(_mailRepository.SendAsync());
                notificationTasks.Add(_eventRepository.ApiEventsPostAsync(_mapper.Map<EventDto>(notification)));
            }

            await Task.WhenAll(notificationTasks.Where(x => x != null).ToList());
        }

        #region private methods

        private string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        private async Task<ActivityDto> SignAsConsultant(ActivityDto activity, UserDto user, string extension, string contentType, byte[] file = null, string signatureUri = "")
        {
            if (activity.Consultant is null || activity.Consultant.Id != user.Id)
            {
                throw new BusinessException(ErrorMessages.userNotPermittedToSign);
            }

            var filename = $"user-{user.Id}-activity-{activity.Id}.{extension}";
            if (file != null)
            {
                var uploadedFileUri = await _azureStorageRepository.UploadAsync(file, filename, contentType);
                activity.Consultant.SignatureUri = uploadedFileUri.ToString();
            }
            else if (!string.IsNullOrEmpty(signatureUri))
            {
                activity.Consultant.SignatureUri = signatureUri;
            }

            // If the customer is already registered as a trine user
            if (activity.Customer != null && !string.IsNullOrEmpty(activity.Customer.Id))
            {
                // Push
                //Dictionary<string, string> templateParams = new Dictionary<string, string>();
                //templateParams["messageParam"] = "CRA à signer";
                //templateParams["messageContent"] = "L'un de vos consultants vient de vous soumettre son CRA pour signature numérique. 📝";
                //// Send the push notification.
                //var sendPushNotifTask = await _notificationRepository.SendTemplatedNotification(templateParams, new List<string>() { activity.Customer.Id });
                //await Task.WhenAll(sendEventNotifTask, sendPushNotifTask);

                var totalWorkedDays = activity.Days.Sum(a => ActivityHelper.GetNumericWorkedPart(a.WorkedPart));
                var customer = await _userRepository.ApiUsersByIdGetAsync(activity.Customer.Id, _userApiVersion);
                if (customer != null)
                {
                    var templateId = _configuration["Mail:Templates:SignActivity"];
                    string monthName = activity.StartDate.ToString("MMMM", CultureInfo.GetCultureInfo("fr-FR"));

                    var customerToken = TokenHelper.BuildToken(_configuration, customer);
                    var link = $"{_configuration["WebApp:Url"]}/app/login?user_id={customer.Id}&token={customerToken.AccessToken}&redirect_uri=/app/dashboard";

                    var dict = new
                    {
                        totalWorkedDays,
                        company = activity.Commercial != null ? $"{activity.Commercial.Firstname} {activity.Commercial.Lastname}" : "---",
                        period = monthName,
                        customer = new
                        {
                            firstName = char.ToUpper(customer.Firstname[0]) + customer.Firstname.Substring(1),
                            lastName = char.ToUpper(customer.Lastname[0]) + customer.Lastname.Substring(1)
                        },
                        consultant = new
                        {
                            firstName = string.IsNullOrEmpty(activity.Consultant.Firstname) ? $"{activity.Consultant.Email}" : $"{activity.Consultant.Firstname} {activity.Consultant.Lastname.ToUpper()}"
                        },
                        link
                    };

                    await _mailRepository.SendAsync(customer.Email, templateId, dict, $"{user.Firstname} ${user.Lastname} vient de valider un CRA");
                }
            } // If the user is not registered yet
            else
            {
                //TODO: Que faire ? On a même pas le mail du client à ce niveau

                //var totalWorkedDays = activity.Days.Sum(a => ActivityHelper.GetNumericWorkedPart(a.WorkedPart));
                //var templateId = _configuration["Mail:Templates:SignActivity"];
                //string monthName = activity.StartDate.ToString("MMMM", CultureInfo.GetCultureInfo("fr-FR"));
                //var link = _configuration["WebApp:Url:SignInvitationPath"];

                //var dict = new
                //{
                //    totalWorkedDays,
                //    company = activity.Commercial != null ? $"{activity.Commercial.Firstname} {activity.Commercial.Lastname}" : "---",
                //    period = monthName,
                //    customer = new
                //    {
                //        firstName = activity.Customer.Email
                //    },
                //    consultant = new
                //    {
                //        firstName = string.IsNullOrEmpty(activity.Consultant.Firstname) ? $"{activity.Consultant.Email}" : $"{activity.Consultant.Firstname} {activity.Consultant.Lastname.ToUpper()}"
                //    },
                //    link
                //};

                //await _mailRepository.SendAsync(activity.Customer.Email, templateId, dict, $"{user.Firstname} ${user.Lastname} vient de valider un CRA");
            }

            activity.Consultant.SignatureDate = DateTime.UtcNow;
            activity.Status = ActivityStatusEnum.ConsultantSigned;

            var updatedActivity = await _activityRepository.ApiActivitiesByIdPatchAsync(activity.Id, _mapper.Map<ActivityDto>(activity), _activityApiVersion);
            if (updatedActivity == null)
            {
                throw new BusinessException(ErrorMessages.errorOccuredWhileSigningActivity);
            }

            if (activity.Customer != null)
            {
                var eventDto = new EventDto()
                {
                    Created = DateTime.UtcNow,
                    EventType = EventDto.EventTypeEnum.ACTION,
                    Title = "Signature d'un CRA",
                    MainContent = $"{user.Firstname} {user.Lastname} vient de signer son rapport d'activité de {activity.StartDate.ToString("MMM YYYY")}",
                    ContextId = activity.Id,
                    UserId = user.Id,
                    TargetId = activity.Customer.Id ?? activity.Customer.Email,
                    ContextType = EventDto.ContextEnum.ACTIVITY,
                    Icon1 = user.ProfilePicUrl,
                    IsEnabled = true
                };
                await _eventRepository.ApiEventsPostAsync(eventDto);
            }

            if (activity.Commercial != null)
            {
                var eventDto = new EventDto()
                {
                    Created = DateTime.UtcNow,
                    EventType = EventDto.EventTypeEnum.ACTION,
                    Title = "Signature d'un CRA",
                    UserId = user.Id,
                    MainContent = $"{user.Firstname} {user.Lastname} vient de signer son rapport d'activité de {activity.StartDate.ToString("MMM YYYY")}",
                    ContextId = activity.Id,
                    TargetId = activity.Commercial.Id,
                    ContextType = EventDto.ContextEnum.ACTIVITY,
                    Icon1 = user.ProfilePicUrl,
                    IsEnabled = true
                };
                await _eventRepository.ApiEventsPostAsync(eventDto);
            }
            return updatedActivity;
        }

        private async Task<bool> SendExcelFileByEmail(ActivityDto activity, string contentType)
        {
            var consultant = await _userRepository.ApiUsersByIdGetAsync(activity.Consultant.Id, _userApiVersion);
            if (consultant != null && !string.IsNullOrEmpty(consultant.Email))
            {
                var month = activity.StartDate.ToString("MMMM");
                var filename = $"rapport_activite_{month}.xlsx";
                var activityFile = await ExportActivity(activity.Id);
                var attachment = new MailAttachment() { File = new MemoryStream(activityFile), Name = filename, Type = contentType };
                var templateName = _configuration["Mail:Templates:ActivityHasBeenSignedByCustomer"];
                if (!string.IsNullOrEmpty(templateName))
                {
                    var period = activity.StartDate.ToString("MMMM yyyy", new CultureInfo("fr-FR"));
                    var templateData = new
                    {
                        consultant = activity.Consultant.Firstname,
                        customer = $"{activity.Customer.Firstname} {activity.Customer.Lastname}",
                        period
                    };

                    var success = await _mailRepository.SendAsync(consultant.Email, templateName, subject: $"Votre rapport d'activité {period} est disponible",
                                            templateData: templateData,
                                            attachment: attachment);

                    return success;
                }
            }
            return false;
        }

        private async Task<ActivityDto> SignAsCustomer(ActivityDto activity, UserDto user, string extension, string contentType, byte[] file = null, string signatureUri = "")
        {
            // Customer has to wait for the consultant to sign first
            if (activity.Consultant is null || activity.Consultant.SignatureDate == null || activity.Consultant.SignatureDate == default(DateTime))
                throw new BusinessException(ErrorMessages.youCannotSignActivityAtThisTime);

            if (activity.Customer is null || activity.Customer.Id != user.Id)
                throw new BusinessException(ErrorMessages.userNotPermittedToSign);

            if (file != null)
            {
                var uploadedFileUri = await _azureStorageRepository.UploadAsync(file, $"user-{user.Id}-activity-{activity.Id}.{extension}", contentType);
                activity.Customer.SignatureUri = uploadedFileUri.ToString();
            }
            else if (!string.IsNullOrEmpty(signatureUri))
            {
                activity.Customer.SignatureUri = signatureUri;
            }

            // We assign the identity of the user to the activity
            // Attaching consultant (which just log on / registered
            activity.Customer = new UserActivityDto()
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                ProfilePicUrl = user.ProfilePicUrl,
                Email = user.Email,
                SignatureDate = DateTime.UtcNow,
                LastUpdateDate = DateTime.UtcNow,
            };

            // We update the status of the activity
            activity.Status = ActivityStatusEnum.CustomerSigned;

            // Save the activity in db
            var updatedActivity = await _activityRepository.ApiActivitiesByIdPatchAsync(activity.Id, _mapper.Map<ActivityDto>(activity), _activityApiVersion);
            if (updatedActivity == null)
                throw new BusinessException(ErrorMessages.errorOccuredWhileSigningActivity);

            // TODO: a refacto car actuellement la generation d'excel necessite une mission
            //await SendExcelFileByEmail(updatedActivity, contentType);
            return updatedActivity;
        }

        private async Task<ActivityDto> SignAsCommercial(ActivityDto activity, UserDto user)
        {
            // Commercial has to wait for the consultant and customer to sign first
            if ((activity.Consultant is null ||
                activity.Consultant.SignatureDate == null ||
                activity.Consultant.SignatureDate == default(DateTime))
                ||
                (activity.Customer is null ||
                activity.Customer.SignatureDate == null ||
                activity.Customer.SignatureDate == default(DateTime)))
            {
                throw new BusinessException(ErrorMessages.youCannotSignActivityAtThisTime);
            }

            if (activity.Commercial is null || activity.Commercial.Id != user.Id)
            {
                throw new BusinessException(ErrorMessages.userNotPermittedToSign);
            }

            activity.Commercial.SignatureDate = DateTime.UtcNow;

            var updatedActivity = await _activityRepository.ApiActivitiesByIdPatchAsync(activity.Id, _mapper.Map<ActivityDto>(activity), _activityApiVersion);
            if (updatedActivity == null)
            {
                throw new BusinessException(ErrorMessages.errorOccuredWhileSigningActivity);
            }

            return updatedActivity;
        }

        private float CalculateWorkedDays(List<GridDayDto> days)
        {
            float result = 0;

            foreach (var day in days)
            {
                switch (day.WorkedPart)
                {
                    case DayPartEnum.Full:
                        result += 1f;
                        break;
                    case DayPartEnum.Afternoon:
                    case DayPartEnum.Morning:
                        result += 0.5f;
                        break;
                    case DayPartEnum.None:
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        #endregion

        public async Task<ActivityDto> GetById(string userId, string activityId)
        {
            var activity = await _activityRepository.ApiActivitiesByIdGetAsync(activityId, _activityApiVersion);
            if (activity == null)
                throw new BusinessException("The activity does not exist.");

            //var mission = await _missionRepository.ApiMissionsByIdGetAsync(activity.MissionId, _missionApiVersion);
            //if (mission == null)
            //    throw new BusinessException("The mission does not exist anymore.");

            var activityDto = _mapper.Map<ActivityDto>(activity);

            // TODO: il faudrait pouvoir récupérer l'id des organizations ou il est admin dans le jwt pour éviter
            // de faire une requete suplémentaire ci-dessous

            //var organization = await _organizationRepository.ApiOrganizationsByIdGetAsync(mission.OrganizationId, _organizationApiVersion);
            //var organizationDto = _mapper.Map<OrganizationDto>(organization);
            //var isAdmin = organizationDto.Members.Any(m => m.UserId == userId && m.Role == OrganizationMemberDto.RoleEnum.ADMIN);

            //if (activity.Consultant.Id == userId) /* isAdmin || */
            //{
            //    activityDto.CanModify = (activityDto.Status == ActivityStatusEnum.Generated);
            //    activityDto.CanSign = (activityDto.Status == ActivityStatusEnum.Generated);
            //}
            //else if (!string.IsNullOrEmpty(activity.Customer.Id) && activity.Customer.Id == userId)
            //{
            //    activityDto.CanModify = false;
            //    activityDto.CanSign = activityDto.Status == ActivityStatusEnum.ConsultantSigned && activityDto.Consultant.SignatureDate.HasValue;
            //}
            //else
            //{
            //    activityDto.CanModify = false;
            //    activityDto.CanSign = false;
            //}

            if (activity.Customer != null)
                activity.Customer.CanSign = activity.Status == ActivityStatusEnum.ConsultantSigned && activity.Customer.Id == userId;
            if (activity.Consultant != null)
                activity.Consultant.CanSign = activity.Status == ActivityStatusEnum.Generated && activity.Consultant.Id == userId;

            return activityDto;
        }

        public async Task<ActivityDto> UpdateActivity(string userId, string activityId, ActivityDto activity)
        {
            if (activity.Consultant.Id == userId)
            {
                activity.Consultant.LastUpdateDate = DateTime.UtcNow;
            }
            else if (activity.Customer.Id == userId)
            {
                activity.Customer.LastUpdateDate = DateTime.UtcNow;
            }
            else if (activity.Commercial.Id == userId)
            {
                activity.Commercial.LastUpdateDate = DateTime.UtcNow;
            }

            var activityDto = await _activityRepository.ApiActivitiesByIdPatchAsync(activityId, activity);
            var userDto = await _userRepository.ApiUsersByIdGetAsync(userId);
            string message = $"{UppercaseFirst(userDto.Firstname)} {UppercaseFirst(userDto.Lastname)} vient de mettre à jour son CRA.";

            if (activity.Commercial != null)
            {
                var eventDto = new EventDto()
                {
                    IsEnabled = true,
                    ContextId = activityId,
                    ContextType = EventDto.ContextEnum.ACTIVITY,
                    Created = DateTime.UtcNow,
                    MainContent = message,
                    TargetId = activity.Commercial.Id,
                    Title = "Mise à jour du rapport d'activité",
                    EventType = EventDto.EventTypeEnum.ACTION,
                    Icon1 = userDto.ProfilePicUrl,
                    UserId = userDto.Id
                };
                await _eventRepository.ApiEventsPostAsync(eventDto);
            }

            if (activity.Customer != null)
            {
                var eventDto = new EventDto()
                {
                    IsEnabled = true,
                    ContextId = activityId,
                    ContextType = EventDto.ContextEnum.ACTIVITY,
                    Created = DateTime.UtcNow,
                    MainContent = message,
                    TargetId = activity.Customer.Id,
                    Title = "Mise à jour du rapport d'activité",
                    EventType = EventDto.EventTypeEnum.ACTION,
                    Icon1 = userDto.ProfilePicUrl,
                    UserId = userDto.Id
                };
                await _eventRepository.ApiEventsPostAsync(eventDto);
            }

            _internalNotificationRepository.SendAsync(message);

            return activityDto;
        }


        public async Task<ActivityDto> UpdateActivity2(string currentUserId, string activityId, ActivityDto activity)
        {
            // First, we update the customer infos if needed (he may have subscribed recently)
            activity = await CheckAndUpdateCustomer(activity);

            if (activity.Consultant.Id == currentUserId)
            {
                activity.Consultant.LastUpdateDate = DateTime.UtcNow;
            }
            else if (activity.Customer.Id == currentUserId)
            {
                activity.Customer.LastUpdateDate = DateTime.UtcNow;
            }

            var activityDto = await _activityRepository.ApiActivitiesByIdPatchAsync(activityId, activity);
            var userDto = await _userRepository.ApiUsersByIdGetAsync(currentUserId);
            string message = $"{UppercaseFirst(userDto.Firstname)} {UppercaseFirst(userDto.Lastname)} vient de mettre à jour son CRA.";

            _internalNotificationRepository.SendAsync(message);

            return activityDto;
        }

        private async Task<ActivityDto> CheckAndUpdateCustomer(ActivityDto activity)
        {
            if (activity.Customer != null && string.IsNullOrEmpty(activity.Customer.Id))
            {
                var customer = await _userRepository.GetByEmail(activity.Customer.Email);
                if (customer != null)
                    activity.Customer = CreateUserActivity(_mapper.Map<UserMissionDto>(customer));
            }

            return activity;
        }

        public async Task RefuseWithModificationProposal(string activityId, string refusingUserId, List<GridDayDto> modifications, string comment = "")
        {
            var activity = await _activityRepository.ApiActivitiesByIdGetAsync(activityId);
            var getUserTask = _userRepository.ApiUsersByIdGetAsync(refusingUserId);

            if (activity is null)
                throw new BusinessException(ErrorMessages.activityDoesntExist);

            // The refusing user must be nor a customer or a commercial
            if ((activity.Customer != null && activity.Customer.Id != refusingUserId) && (activity.Commercial != null && activity.Commercial.Id != refusingUserId))
                throw new BusinessException(ErrorMessages.onlyCustomerAndCommercialCanRequireActivityModification);

            var activityDto = _mapper.Map<ActivityDto>(activity);
            if (activityDto.Status != ActivityStatusEnum.ConsultantSigned)
                throw new BusinessException(ErrorMessages.consultantMustHaveSignedActivity);

            activityDto.Customer.SignatureDate = null;
            activityDto.Status = ActivityStatusEnum.ModificationsRequired;

            if (activityDto.ModificationProposals is null)
                activityDto.ModificationProposals = new List<ModificationProposalDto>();

            activityDto.ModificationProposals.Add(new ModificationProposalDto()
            {
                Comment = comment,
                CreationDate = DateTime.UtcNow,
                CurrentStatus = ModificationProposalDto.Status.Rejected,
                UserId = refusingUserId,
                Modifications = modifications
            });

            await _activityRepository.ApiActivitiesByIdPatchAsync(activityId, _mapper.Map<ActivityDto>(activityDto));

            var refusingUser = await getUserTask;
            if (refusingUser is null)
                throw new BusinessException(ErrorMessages.userCannotBeNull);

            var eventDto = new EventDto()
            {
                Title = string.Format(UserMessages.refusedActivityModificationsEventTitle, refusingUser.Firstname, refusingUser.Lastname, activityDto.StartDate.Month, activityDto.EndDate.Year),
                ContextId = activityDto.MissionId,
                ContextType = EventDto.ContextEnum.MISSION
            };
            await _eventRepository.ApiEventsPostAsync(_mapper.Map<EventDto>(eventDto));
        }

        public async Task AcceptLastModificationProposal(string activityId, string userId)
        {
            var getUserTask = _userRepository.ApiUsersByIdGetAsync(userId);
            var activity = await _activityRepository.ApiActivitiesByIdGetAsync(activityId);
            var activityDto = _mapper.Map<ActivityDto>(activity);

            if (activityDto is null)
                throw new BusinessException(ErrorMessages.activityDoesntExist);

            if (activityDto.Consultant is null || activityDto.Consultant.Id != userId)
                throw new BusinessException(ErrorMessages.badUserRole);

            if (activityDto.Status != ActivityStatusEnum.ModificationsRequired)
                throw new BusinessException(ErrorMessages.badActivityStatus);

            if (activityDto.ModificationProposals == null || !activityDto.ModificationProposals.Any())
                throw new BusinessException(ErrorMessages.activityModificationsNotExist);

            var lastModificationProposal = activityDto.ModificationProposals.Last();
            foreach (var modification in lastModificationProposal.Modifications)
            {
                var day = activityDto.Days.FirstOrDefault(d => d.Day == modification.Day);
                if (day is null)
                    continue;

                day.WorkedPart = modification.WorkedPart; // TODO
            }
            activityDto.Status = ActivityStatusEnum.ConsultantSigned;
            activityDto.Consultant.SignatureDate = DateTime.UtcNow;

            // Updating activity 
            await _activityRepository.ApiActivitiesByIdPatchAsync(activityId, _mapper.Map<ActivityDto>(activityDto));

            var user = await getUserTask;
            var modificationAuthor = await _userRepository.ApiUsersByIdGetAsync(lastModificationProposal.UserId);
            if (modificationAuthor is null)
                throw new BusinessException(ErrorMessages.authorDoesntExist);

            // Sending notifications
            var mailData = new
            {
                user = new
                {
                    firstName = user.Firstname,
                    lastName = user.Lastname,
                    requestDate = lastModificationProposal.CreationDate.ToShortDateString()
                }
            };
            var eventDto = new EventDto()
            {
                Title = string.Format(UserMessages.modifiedActivityEventTitle, activityDto.StartDate.Month, activityDto.StartDate.Year),
                Subtitle = string.Format(UserMessages.modifiedActivityEventSubtitle, user.Firstname, user.Lastname, modificationAuthor.Firstname, modificationAuthor.Lastname),
                ContextId = activityDto.MissionId,
                ContextType = EventDto.ContextEnum.MISSION
            };

            var mailTask = _mailRepository.SendAsync(modificationAuthor.Email, _configuration["Mail:Templates:ActivityModificationProposalHasBeenAccepted"], mailData);
            var createEventTask = _eventRepository.ApiEventsPostAsync(_mapper.Map<EventDto>(eventDto));
            await Task.WhenAll(mailTask, createEventTask);
        }

        public async Task<ActivityDto> GetByToken(string token)
        {
            // Deserializing the jwt 
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
                throw new BusinessException("Inconsistent token.");

            handler.ValidateToken(token, new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthenticationSettings:Key"]))
            }, out SecurityToken validatedJwt);

            var jwt = handler.ReadJwtToken(token);
            var claims = jwt.Claims?.ToList();
            var activityId = claims?.FirstOrDefault(x => x.Type == "activityId")?.Value;
            var customerEmail = claims?.FirstOrDefault(x => x.Type == "customerEmail")?.Value;

            if (string.IsNullOrEmpty(activityId))
                throw new BusinessException("Inconsistent token : the activityId claim is not present.");

            if (string.IsNullOrEmpty(customerEmail))
                throw new BusinessException("Inconsistent token : the customerEmail claim is not present.");

            // Getting the activity
            var activity = await _activityRepository.ApiActivitiesByIdGetAsync(activityId);
            if (activity is null)
                throw new BusinessException("This activity doesn't exist.");

            if (activity.Customer == null || activity.Customer.Email != customerEmail)
                throw new BusinessException("You are not entitled to see this activity.");

            // Setting the CanSign property
            activity.Customer.CanSign = activity.Status == ActivityStatusEnum.ConsultantSigned && activity.Customer.Email == customerEmail;
            return activity;
        }
    }
}
