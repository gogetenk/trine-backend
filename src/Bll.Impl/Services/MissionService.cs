using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Event.Dal.Repositories;
using Assistance.Operational.Bll.Impl.Errors;
using Assistance.Operational.Bll.Impl.Fetchers;
using Assistance.Operational.Bll.Impl.Helpers;
using Assistance.Operational.Bll.Services;
using Assistance.Operational.Dal.Repositories;
using Assistance.Operational.Dto;
using Assistance.Operational.Model;
using AutoMapper;
using Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sogetrel.Sinapse.Framework.Bll.Services;
using Sogetrel.Sinapse.Framework.Exceptions;


namespace Assistance.Operational.Bll.Impl.Services
{
    /// <summary>
    /// Service that manages the missions (details, list, creation etc)
    /// </summary>
    public class MissionService : ServiceBase, IMissionService
    {
        private readonly IMapper _mapper;
        private readonly IMissionRepository _missionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IMailRepository _mailRepository;
        private readonly IOrganizationRepository _OrganizationDtoRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IConfiguration _configuration;
        private readonly IInternalNotificationRepository _internalNotificationRepository;

        private const string _userApiVersion = "1.0";
        private const string _missionApiVersion = "1.0";
        private const string _invoiceApiVersion = "1.0";
        private const string _eventApiVersion = "1.0";
        private const string _OrganizationDtoApiVersion = "1.0";

        public MissionService(
            ILogger<ServiceBase> logger,
            IMapper mapper,
            IConfiguration configuration,
            IMissionRepository missionRepository,
            IUserRepository userRepository,
            IEventRepository eventRepository,
            IMailRepository mailRepository,
            IOrganizationRepository OrganizationDtoRepository,
            INotificationRepository notificationRepository,
            IInternalNotificationRepository internalNotificationRepository)
            : base(logger)
        {
            _mapper = mapper;
            _missionRepository = missionRepository;
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _mailRepository = mailRepository;
            _OrganizationDtoRepository = OrganizationDtoRepository;
            _notificationRepository = notificationRepository;
            _configuration = configuration;
            _internalNotificationRepository = internalNotificationRepository;
        }

        /// <summary>
        /// Gets all the missions the user is involved in
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public async Task<List<MissionDto>> GetMissionsFromUser(string userId, bool isActive)
        {
            List<MissionDto> missions = new List<MissionDto>();

            missions = await _missionRepository.ApiMissionsByUserIdByActiveGetAsync(userId, isActive, _missionApiVersion);

            if (missions == null || !missions.Any())
                return null;

            var userIds = missions.Where(x => !string.IsNullOrEmpty(x.Customer.Id)).Select(x => x.Customer.Id).Distinct().ToList();
            userIds.AddRange(missions.Where(x => x.IsTripartite || !string.IsNullOrEmpty(x.Commercial?.Id)).Select(x => x.Commercial.Id).Distinct().ToList());
            userIds.AddRange(missions.Where(x => !string.IsNullOrEmpty(x.Consultant.Id)).Select(x => x.Consultant.Id).Distinct().ToList());
            userIds = userIds.Distinct().ToList();

            var users = await _userRepository.ApiUsersIdsGetAsync(userIds, _userApiVersion);

            if (users == null || !users.Any())
                throw new BusinessException("Users cannot be null");

            //var missionIds = missions.Select(x => x.Id).Distinct().ToList();
            //var invoices = await _invoiceRepository.ApiInvoicesMissionsIdsGetAsync(missionIds, _invoiceApiVersion);
            //var events = await _eventRepository.ApiEventsContextByContextIdGetAsync(missionIds, _eventApiVersion);

            var missionDtos = _mapper.Map<List<MissionDto>>(missions);

            //missionDtos.ForEach(x =>
            //{
            //    x.Commercial = x.IsTripartite ? _mapper.Map<UserDto>(users.FirstOrDefault(y => y.Id == x.Commercial?.Id)) : null;
            //    x.Customer = _mapper.Map<UserDto>(users.FirstOrDefault(y => y.Id == x.Customer?.Id));
            //    x.Consultant = _mapper.Map<UserDto>(users.FirstOrDefault(y => y.Id == x.Consultant?.Id));
            //    //x.Events = _mapper.Map<List<EventDto>>(_eventRepository.ApiEventsMissionByMissionIdGetAsync(x.Id, _eventApiVersion).Result);
            //    //x.Invoices = _mapper.Map<List<InvoiceDto>>(_invoiceRepository.ApiInvoicesMissionByMissionIdGetAsync(x.Id, _invoiceApiVersion).Result);
            //    //x.Invoices = _mapper.Map<List<InvoiceDto>>(invoices?.Where(y => y.MissionId == x.Id).ToList());
            //    //x.Events = _mapper.Map<List<EventDto>>(events?.Where(y => y.MissionId == x.Id).ToList());
            //});

            return missionDtos;
        }

        /// <summary>
        /// Gets the current mission of an user (MVP only ?)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public async Task<MissionDto> GetUserCurrentMission(string userId)
        {
            return await _missionRepository.GetUserCurrentMission(userId);
        }

        private async Task<UserDto> InviteAndReturnUser(UserDto user, string orgaId)
        {
            // Verify if user doesn't exist in Trine
            var results = await _userRepository.ApiUsersSearchGetAsync(user.Email, null, null, _userApiVersion);

            // If the user is not registered in Trine, we invite him to Trine first
            if (results == null || !results.Any())
            {
                // Creates the user in Trine
                user.Firstname = ""; // Workaround for user api search method (cannot find if null)
                user.Lastname = "";
                user.Id = (await _userRepository.ApiUsersPostAsync(user, _userApiVersion)).Id;
            }
            else
            {
                user = _mapper.Map<UserDto>(results.FirstOrDefault());
            }

            // Now that the user is registered into Trine, we invite him to the OrganizationDto
            var member = new OrganizationMemberDto()
            {
                UserId = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Icon = user.ProfilePicUrl,
                Role = OrganizationMemberDto.RoleEnum.MEMBER,
                JoinedAt = DateTime.UtcNow
            };
            var createdMember = await _OrganizationDtoRepository.ApiOrganizationsByOrganizationIdMembersPutAsync(orgaId, _mapper.Map<OrganizationMemberDto>(member), _OrganizationDtoApiVersion);
            if (createdMember is null)
                throw new TechnicalException($"The user id {user?.Id} cannot be added to OrganizationDto {orgaId}");

            return user;
        }

        public async Task<MissionDto> CreateMission(CreateMissionRequestDto request)
        {
            if (request.Customer is null || request.Consultant is null || request.Commercial is null && request.IsTripartite)
                throw new BusinessException(ErrorMessages.atLeastTwoUsersNeeded);

            if (string.IsNullOrEmpty(request.Customer.Email) || string.IsNullOrEmpty(request.Consultant.Email) || string.IsNullOrEmpty(request.Commercial?.Email) && request.IsTripartite)
                throw new BusinessException(ErrorMessages.emailNeeded);

            // Subscribing users that are not yet Trine users or current OrganizationDto members.
            if (string.IsNullOrEmpty(request.Customer.Id))
                request.Customer = await InviteAndReturnUser(request.Customer, request.OrganizationId);
            if (string.IsNullOrEmpty(request.Consultant.Id))
                request.Consultant = await InviteAndReturnUser(request.Consultant, request.OrganizationId);
            if (string.IsNullOrEmpty(request.Commercial?.Id) && request.IsTripartite)
                request.Commercial = await InviteAndReturnUser(request.Commercial, request.OrganizationId);

            // Call User Api to get the concerned users
            var customerData = _userRepository.ApiUsersByIdGetAsync(request.Customer.Id, _userApiVersion);
            var consultantData = _userRepository.ApiUsersByIdGetAsync(request.Consultant.Id, _userApiVersion);
            Task<UserDto> commercialData = null;
            if (!string.IsNullOrEmpty(request.Commercial?.Id) && request.IsTripartite)
                commercialData = _userRepository.ApiUsersByIdGetAsync(request.Commercial.Id, _userApiVersion);

            UserModel consultant = _mapper.Map<UserModel>(await consultantData);
            UserModel customer = _mapper.Map<UserModel>(await customerData);
            UserModel commercial = null;
            if (!string.IsNullOrEmpty(request.Commercial?.Id) && commercialData != null)
                commercial = _mapper.Map<UserModel>(await commercialData);

            if (customer == null || consultant == null || (commercial == null && request.IsTripartite))
                throw new BusinessException(ErrorMessages.errorOccuredWhileGettingUsers);

            // Using this informations in order to create the contract + the mission object
            MissionDto mission = CreateConsolidatedMission(request, customer, consultant, commercial);

            // Call the Mission Api for creation
            var createdMission = _mapper.Map<MissionDto>(await _missionRepository.ApiMissionsPostAsync(mission, _missionApiVersion));
            if (createdMission is null)
                throw new BusinessException("Error while getting the newly created mission id");

            //// Adding additional data for restition purpose
            createdMission.Commercial = _mapper.Map<UserMissionDto>(commercial);
            createdMission.Consultant = _mapper.Map<UserMissionDto>(consultant);
            createdMission.Customer = _mapper.Map<UserMissionDto>(customer);

            // Updating the orga members status so they cannot be guests (otherwise they won't see the mission)
            Task[] updateMemberTasks = await UpdateMemberStatus(request, customer, consultant, commercial);
            await Task.WhenAll(updateMemberTasks);

            // Sending events and notifications
            await SendUserNotifications(customer, consultant, commercial, mission, createdMission.Id);
            return createdMission;
        }

        private async Task<Task<OrganizationMemberDto>> GetMemberUpdateTask(CreateMissionRequestDto request, Task<OrganizationMemberDto> memberTask)
        {
            var member = _mapper.Map<OrganizationMemberDto>(await memberTask);
            if (member is null)
                throw new BusinessException("One of the member is not found. Maybe he was deleted during the mission creation ? Please choose another one.");
            if (member.Role >= OrganizationMemberDto.RoleEnum.MEMBER)
                return null;

            member.Role = OrganizationMemberDto.RoleEnum.MEMBER;
            memberTask = _OrganizationDtoRepository.ApiOrganizationsByOrganizationIdMembersByUserIdPatchAsync(request.OrganizationId, member.UserId, _mapper.Map<OrganizationMemberDto>(member), _OrganizationDtoApiVersion);
            return memberTask;
        }

        public async Task UpdateCustomer(string userId, string missionId, MissionCustomerDto dto)
        {
            var mission = await _missionRepository.ApiMissionsByIdGetAsync(missionId);
            if (mission == null)
            {
                throw new BusinessException("Unable to find the mission");
            }

            if (mission.Customer is null)
            {
                throw new BusinessException("Customer does not exists");
            }

            if (mission.OwnerId != userId)
            {
                throw new BusinessException("You must be owner of this mission");
            }

            mission.ProjectName = dto.Name;
            mission.Customer = new UserMissionDto()
            {
                Firstname = dto.CustomerFirstName,
                Lastname = dto.CustomerLastName,
                GlobalRole = UserMissionDto.GlobalRoleEnum.Customer,
                IsDummy = true,
                Id = mission.Customer.Id
            };
            await _missionRepository.ApiMissionsPutAsync(mission.Id, mission);

            var customer = await _userRepository.ApiUsersByIdGetAsync(mission.Customer.Id);
            customer.Firstname = dto.CustomerFirstName;
            customer.Lastname = dto.CustomerLastName;
            customer.PhoneNumber = dto.CustomerPhone;
            customer.Email = dto.CustomerEmail;
            customer.SubscriptionDate = DateTime.UtcNow;
            customer.IsDummy = true;
            await _userRepository.ApiUsersPutAsync(customer.Id, customer);

            var owner = await _userRepository.ApiUsersByIdGetAsync(userId);
            _internalNotificationRepository.SendAsync($"✍🏼 {owner.Firstname} {owner.Lastname} vient de mettre à jour les informations de son client (${mission.Customer.Firstname} {mission.Customer.Lastname}).");
        }

        public async Task UpdateConsultant(string missionId, string userId)
        {
            var userTask = _userRepository.ApiUsersByIdGetAsync(userId);

            var mission = await _missionRepository.ApiMissionsByIdGetAsync(missionId);
            if (mission == null)
                throw new BusinessException("Unable to find the mission");

            mission.Consultant.Id = userId;
            await _missionRepository.ApiMissionsPutAsync(missionId, mission);
            var user = await userTask;
            await _mailRepository.SendAsync(user.Email, TextMessages.newMissionInvitation);
        }

        public async Task<MissionDto> UpdateMission(string id, MissionDto dto)
        {
            var mission = await _missionRepository.ApiMissionsPutAsync(id, _mapper.Map<MissionDto>(dto), _missionApiVersion);
            return _mapper.Map<MissionDto>(mission);
        }

        public async Task<MissionDto> GetMissionById(string id)
        {
            var mission = await _missionRepository.ApiMissionsByIdGetAsync(id, _missionApiVersion);

            if (mission is null)
                return null;

            // Call User Api to get the concerned users
            var customerData = _userRepository.ApiUsersByIdGetAsync(mission.Customer.Id, _userApiVersion);
            var consultantData = _userRepository.ApiUsersByIdGetAsync(mission.Consultant.Id, _userApiVersion);
            var commercialData = _userRepository.ApiUsersByIdGetAsync(mission.Commercial.Id, _userApiVersion);
            UserDto customer = _mapper.Map<UserDto>(await customerData);
            UserDto consultant = _mapper.Map<UserDto>(await consultantData);
            UserDto commercial = _mapper.Map<UserDto>(await commercialData);
            var dto = _mapper.Map<MissionDto>(mission);
            //dto.Customer = customer;
            //dto.Commercial = commercial;
            //dto.Consultant = consultant;

            return dto;
        }

        public async Task<MissionDto> GetMissionAggregate(string id)
        {
            var mission = _mapper.Map<MissionDto>(await _missionRepository.ApiMissionsByIdGetAsync(id, _missionApiVersion));
            if (mission == null)
                return null;

            var fetcher = new MissionDataFetcher(_mapper, _userRepository, _missionRepository, _eventRepository);
            var aggregateDto = await fetcher.GetDashboardMissionData(mission);

            return aggregateDto;
        }

        public async Task<MissionDto> CancelMission(string id)
        {
            var mission = _mapper.Map<MissionDto>(await _missionRepository.ApiMissionsByIdGetAsync(id, _missionApiVersion));
            mission.CanceledDate = DateTime.Now;
            mission.Status = MissionDto.StatusEnum.CANCELED;

            var missionToUpdate = _mapper.Map<MissionDto>(mission);
            var updatedMission = await _missionRepository.ApiMissionsPutAsync(id, missionToUpdate);
            var dto = _mapper.Map<MissionDto>(updatedMission);

            var createdMissionEvent = new EventModel()
            {
                Created = DateTime.UtcNow,
                EventType = EventModel.EventTypeEnum.ALERT,
                ContextId = id,
                Title = $"Mission cancelled."
            };
            await _eventRepository.ApiEventsPostAsync(_mapper.Map<EventDto>(createdMissionEvent), _eventApiVersion);

            return dto;
        }

        /// <summary>
        /// Génère un contrat cadre avec les informations données par l'utilisateur afin de le prévisualiser avant acceptation.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<FrameContractDto> GetContractPreview(CreateMissionRequestDto request)
        {
            if (request.Customer is null || request.Consultant is null || request.Commercial is null && request.IsTripartite)
                throw new BusinessException(ErrorMessages.atLeastTwoUsersNeeded);

            // Call User Api to get the concerned users
            var customerData = _userRepository.ApiUsersByIdGetAsync(request.Customer.Id, _userApiVersion);
            var consultantData = _userRepository.ApiUsersByIdGetAsync(request.Consultant.Id, _userApiVersion);
            Task<UserDto> commercialData = null;
            if (!string.IsNullOrEmpty(request.Commercial.Id))
                commercialData = _userRepository.ApiUsersByIdGetAsync(request.Commercial.Id, _userApiVersion);

            UserModel customer = _mapper.Map<UserModel>(await customerData);
            UserModel consultant = _mapper.Map<UserModel>(await consultantData);
            UserModel commercial = null;
            if (!string.IsNullOrEmpty(request.Commercial.Id) && commercialData != null)
                commercial = _mapper.Map<UserModel>(await commercialData);

            if (customer == null || consultant == null || (commercial == null && request.IsTripartite))
                throw new BusinessException(ErrorMessages.errorOccuredWhileGettingUsers);

            // Create a FrameContract and SubContract to attach onto the mission
            var frameContract = ContractWriter
                .Build()
                .From(request.StartDate)
                .To(request.EndDate)
                .WithCommercial(commercial)
                .WithConsultant(consultant)
                .WithCustomer(customer)
                .WithDailyPrice(request.DailyPrice)
                .WriteContract();

            return _mapper.Map<FrameContractDto>(frameContract);
        }

        public async Task<List<MissionDto>> GetFromOrganizationId(string id)
        {
            var results = await _missionRepository.ApiMissionsOrganizationsByIdGetAsync(id, _missionApiVersion);
            return _mapper.Map<List<MissionDto>>(results);
        }

        public async Task<List<MissionDto>> GetFromOwnerId(string userId)
        {
            var results = await _missionRepository.ApiMissionsUsersByIdGetAsync(userId, _missionApiVersion);
            return _mapper.Map<List<MissionDto>>(results);
        }

        public async Task DeleteMission(string id)
        {
            await _missionRepository.ApiMissionsByIdDeleteAsync(id, _missionApiVersion);
        }


        #region Private Methods

        private async Task SendUserNotifications(UserModel customer, UserModel consultant, UserModel commercial, MissionDto mission, string id)
        {
            var createdMissionEvent = new EventModel()
            {
                Created = DateTime.UtcNow,
                EventType = EventModel.EventTypeEnum.LOG,
                ContextId = id,
                Title = $"Mission created."
            };
            var logTask = _eventRepository.ApiEventsPostAsync(_mapper.Map<EventDto>(createdMissionEvent), _eventApiVersion);

            // The user may not have its firstname and lastname specified yet
            // So we specify its email instead
            string mainContent = string.IsNullOrEmpty(mission.Customer.Lastname) ? mission.Customer.Email : $"{mission.Customer.Firstname} {mission.Customer.Firstname}";

            EventModel invitation = new EventModel()
            {
                Created = DateTime.UtcNow,
                EventType = EventModel.EventTypeEnum.ACTION,
                Title = "Invitation de mission",
                MainContent = mainContent,
                SubContent = $"via {mission.OrganizationId}",
                Icon1 = $"{mission.Customer.ProfilePicUrl}",
                ContextId = id,
                ContextType = EventModel.ContextEnum.MISSION,
                IsEnabled = true
            };

            Task inviteCommercialTask = null;
            Task inviteConsultantTask = null;
            Task inviteCustomerTask = null;

            // Si la mission est tripartite, et qu'elle n'a pas été créée par le consultant
            if (mission.IsTripartite)
                invitation.Icon2 = $"{mission.Commercial.ProfilePicUrl}";

            // Invitation du commercial si ce n'est pas lui qui a initié la création
            if (mission.IsTripartite && mission.OwnerId != commercial.Id)
                inviteCommercialTask = GetInviteUserTask(commercial, "Commercial", invitation);

            // Invitation du Consultant si ce n'est pas lui qui a initié la création
            if (mission.OwnerId != consultant.Id)
                inviteConsultantTask = GetInviteUserTask(consultant, "Consultant", invitation);

            // Invitation du client si ce n'est pas lui qui a initié la création
            if (mission.OwnerId != customer.Id)
                inviteCustomerTask = GetInviteUserTask(customer, "Client", invitation);

            var notificationTask = SendNotifications(mission, mission.IsTripartite ? commercial : customer, consultant); //TODO: utiliser le JWT pour mettre le vrai initiateur de la mission
            await Task.WhenAll(new Task[] { logTask, inviteCommercialTask, inviteConsultantTask, inviteCustomerTask, notificationTask }.Where(i => i != null));
        }

        public async Task SendNotifications(MissionDto mission, UserModel sender, UserModel receiver)
        {
            if (mission is null || sender is null || receiver is null)
                throw new BusinessException("One of the user is null");

            var message = $"Vous avez été invité à rejoindre une mission par {sender.Firstname} {sender.LastName.ToUpper()}";

            var inviteUserEvent = new EventModel()
            {
                Created = DateTime.UtcNow,
                EventType = EventModel.EventTypeEnum.ACTION,
                ContextId = mission.Id,
                Title = message
            };
            var sendEventTask = _eventRepository.ApiEventsPostAsync(_mapper.Map<EventDto>(inviteUserEvent));
            var templateId = _configuration["Mail:Templates:InviteUserToMission"];
            var dynamicTemplateData = new
            {
                host = $"{sender.Firstname} {sender.LastName}",
                firstname = receiver.Firstname,
                missionUrl = $"{_configuration["WebApp:Url"]}/missions/{mission.Id}"
            };
            var sendMailTask = _mailRepository.SendAsync(sender.Email, templateId, dynamicTemplateData);

            // Send the message so that all template registrations that contain "messageParam"
            // receive the notifications. This includes APNS, GCM, WNS, and MPNS template registrations.
            Dictionary<string, string> templateParams = new Dictionary<string, string>();
            templateParams["messageParam"] = "Invitation à rejoindre une mission";
            templateParams["messageContent"] = message;

            // Send the push notification.
            var sendPushNotifTask = _notificationRepository.SendTemplatedNotification(templateParams, new List<string>() { receiver.Id });

            // Send email, Log Event
            await Task.WhenAll(sendMailTask, sendEventTask, sendPushNotifTask);
        }


        private async Task<Task[]> UpdateMemberStatus(CreateMissionRequestDto request, UserModel customer, UserModel consultant, UserModel commercial)
        {
            var consultantMemberTask = _OrganizationDtoRepository.ApiOrganizationsByOrganizationIdMembersByUserIdGetAsync(request.OrganizationId, consultant.Id, _OrganizationDtoApiVersion);
            var customerMemberTask = _OrganizationDtoRepository.ApiOrganizationsByOrganizationIdMembersByUserIdGetAsync(request.OrganizationId, customer.Id, _OrganizationDtoApiVersion);
            var commercialMemberTask = (request.IsTripartite) ? _OrganizationDtoRepository.ApiOrganizationsByOrganizationIdMembersByUserIdGetAsync(request.OrganizationId, commercial.Id, _OrganizationDtoApiVersion) : null;
            consultantMemberTask = await GetMemberUpdateTask(request, consultantMemberTask);
            customerMemberTask = await GetMemberUpdateTask(request, customerMemberTask);
            commercialMemberTask = (request.IsTripartite) ? await GetMemberUpdateTask(request, commercialMemberTask) : null;
            var tasks = new Task[] { consultantMemberTask, customerMemberTask, commercialMemberTask }.Where(x => x != null).ToArray();
            return tasks;
        }

        private MissionDto CreateConsolidatedMission(CreateMissionRequestDto request, UserModel customer, UserModel consultant, UserModel commercial)
        {
            // Create a FrameContract and SubContract to attach onto the mission
            var frameContract = ContractWriter
                .Build()
                .From(request.StartDate)
                .To(request.EndDate)
                .WithCommercial(commercial)
                .WithConsultant(consultant)
                .WithCustomer(customer)
                .WithDailyPrice(request.DailyPrice)
                .WriteContract();

            // Create a consolidated Mission model from the request and additional infos gathered from data apis
            var mission = new MissionDto()
            {
                Commercial = _mapper.Map<UserMissionDto>(commercial),
                Customer = _mapper.Map<UserMissionDto>(customer),
                Consultant = _mapper.Map<UserMissionDto>(consultant),
                DailyPrice = request.DailyPrice,
                CommercialFeePercentage = request.CommercialFeePercentage,
                IsTripartite = commercial != null,
                PaymentFrequency = MissionDto.FrequencyEnum.MONTHLY,
                Status = MissionDto.StatusEnum.CREATED,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                FrameContract = _mapper.Map<FrameContractDto>(frameContract),
                OrganizationId = request.OrganizationId,
                ProjectName = request.ProjectName,
                CreatedDate = DateTime.UtcNow,
                OwnerId = request.OwnerId
            };
            return mission;
        }

        private Task GetInviteUserTask(UserModel user, string userTypeTitle, EventModel invitation)
        {
            Task inviteUserTask;
            invitation.Subtitle = userTypeTitle;
            invitation.TargetId = user.Id;
            inviteUserTask = _eventRepository.ApiEventsPostAsync(_mapper.Map<EventDto>(invitation), _eventApiVersion);
            return inviteUserTask;
        }

        #endregion

    }
}
