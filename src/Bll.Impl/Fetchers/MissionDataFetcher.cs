using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Operational.Bll.Impl.Errors;
using Assistance.Operational.Dal.Repositories;
using AutoMapper;
using Dto;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Bll.Impl.Fetchers
{
    /// <summary>
    /// Does stuff that is shared amoung multiple services
    /// </summary>
    public class MissionDataFetcher
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IMissionRepository _missionRepository;
        private readonly IEventRepository _eventRepository;

        private const string _userApiVersion = "1.0";
        private const string _missionApiVersion = "1.0";
        private const string _invoiceApiVersion = "1.0";
        private const string _eventApiVersion = "1.0";

        public MissionDataFetcher(IMapper mapper, IUserRepository userRepository, IMissionRepository missionRepository, IEventRepository eventRepository)
        {
            _mapper = mapper;
            _missionRepository = missionRepository;
            _eventRepository = eventRepository;
            _userRepository = userRepository;
        }

        public async Task<MissionDto> GetMissionData(MissionDto mission)
        {
            //var invoices = await _invoiceRepository.ApiInvoicesMissionByMissionIdGetAsync(mission.Id, _invoiceApiVersion);
            var events = _eventRepository.ApiEventsTargetsByTargetIdGetAsync(mission.Id, _eventApiVersion);

            // Getting all users at once
            MissionDto missionDto = await GetAndAssignUsersToMission(mission);
            missionDto.Events = _mapper.Map<List<EventDto>>(await events);

            return missionDto;
        }

        private async Task<MissionDto> GetAndAssignUsersToMission(MissionDto mission)
        {
            if (mission.IsTripartite && mission.Commercial is null)
                throw new BusinessException(ErrorMessages.commercialCannotBeNull);

            var userIds = new List<string>()
            {
                mission.Consultant.Id,
                mission.Customer.Id
            };
            if (mission.IsTripartite && mission.Commercial != null)
                userIds.Add(mission.Commercial.Id);

            // Deleting doubles
            userIds = userIds.Distinct().ToList();

            var users = _mapper.Map<List<UserDto>>(await _userRepository.ApiUsersIdsGetAsync(userIds, _userApiVersion));
            if (users is null || !users.Any())
                throw new TechnicalException("An error occured while getting the users");

            var customer = users.FirstOrDefault(x => x.Id == mission.Customer.Id);
            var consultant = users.FirstOrDefault(x => x.Id == mission.Consultant.Id);
            UserDto commercial = null;
            if (mission.IsTripartite)
                commercial = users.FirstOrDefault(x => x.Id == mission.Commercial.Id);

            var missionDto = _mapper.Map<MissionDto>(mission);
            //missionDto.Commercial = commercial;
            //missionDto.Customer = customer;
            //missionDto.Consultant = consultant;
            return missionDto;
        }

        public async Task<MissionDto> GetDashboardMissionData(MissionDto mission)
        {
            // Getting all users at once
            MissionDto missionDto = await GetAndAssignUsersToMission(mission);

            return missionDto;
        }

    }
}
