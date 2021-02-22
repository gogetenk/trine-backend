using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Operational.Bll.Impl.Errors;
using Assistance.Operational.Bll.Impl.Fetchers;
using Assistance.Operational.Bll.Services;
using Assistance.Operational.Dal.Repositories;
using AutoMapper;
using Dto;
using Dto.Responses;
using Microsoft.Extensions.Logging;
using Sogetrel.Sinapse.Framework.Bll.Services;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Bll.Impl.Services
{
    public class DashboardService : ServiceBase, IDashboardService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IMissionRepository _missionRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IOrganizationRepository _organizationRepository;

        private const string _userApiVersion = "1.0";
        private const string _missionApiVersion = "1.0";

        public DashboardService(ILogger<ServiceBase> logger,
            IMapper mapper,
            IUserRepository userRepository,
            IMissionRepository missionRepository,
            IEventRepository eventApi,
            IOrganizationRepository organizationRepository,
            IActivityRepository activityRepository) : base(logger)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _missionRepository = missionRepository;
            _eventRepository = eventApi;
            _organizationRepository = organizationRepository;
            _activityRepository = activityRepository;
        }

        /// <summary>
        /// Returns only the 5 last active missions
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userType"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public async Task<List<DashboardMissionDto>> GetLatestMissionsFromUser(string userId, bool isActive)
        {
            var missions = _mapper.Map<List<MissionDto>>(await _missionRepository.ApiMissionsByUserIdByActiveGetAsync(userId, isActive, _missionApiVersion));
            if (missions == null || !missions.Any())
                return null;

            int _missionCountToRetrieve = 5;
            missions = missions.OrderBy(x => x.StartDate).Take(_missionCountToRetrieve).ToList();

            int missionCount = missions.Count;
            var tasks = new Task<MissionDto>[missionCount];
            var fetcher = new MissionDataFetcher(_mapper, _userRepository, _missionRepository, _eventRepository);

            // TODO : faire BEAUCOUP mieux (chopper une méga liste de tous les users cross missions, enlever les doubles faire l'assignation dans un foreach
            for (int i = 0; i < missionCount; i++)
                tasks[i] = fetcher.GetDashboardMissionData(missions[i]);

            var missionDtos = (await Task.WhenAll(tasks)).ToList();
            var dashboardMissionDtos = missionDtos.Select(x => new DashboardMissionDto()
            {
                Id = x.Id,
                IsTripartite = x.IsTripartite,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                ProjectName = x.ProjectName,
                CustomerName = x.Customer.Firstname + " " + x.Customer.Lastname,
                ConsultantName = x.Consultant.Firstname + " " + x.Consultant.Lastname,
                CommercialName = x.IsTripartite ? x.Consultant.Firstname + " " + x.Consultant.Lastname : null,
                OrganizationName = x.OrganizationName,
                OrganizationIcon = x.OrganizationIcon
            }).ToList();

            return dashboardMissionDtos;
        }

        public async Task<DashboardCountDto> GetCountsFromUser(string userId)
        {
            var events = _eventRepository.ApiEventsTargetsByTargetIdCountGetAsync(userId);
            var user = _userRepository.ApiUsersByIdGetAsync(userId, _userApiVersion);
            var missions = _missionRepository.ApiMissionsByUserIdByActiveGetAsync(userId, true);

            var userDto = _mapper.Map<UserDto>(await user);
            if (userDto == null)
                throw new BusinessException(ErrorMessages.userCannotBeNull);

            var missionCount = (await missions) != null ? (await missions).Count : 0;
            var clientCount = (await missions) != null ? (await missions).GroupBy(x => x.Customer.Id).Count() : 0;

            return new DashboardCountDto()
            {
                CurrentUser = userDto,
                EventCount = await events,
                MissionCount = missionCount,
                ClientCount = clientCount
            };
        }

        public async Task<List<EventDto>> GetEventsFromUser(string userId)
        {
            var events = await _eventRepository.ApiEventsTargetsByTargetIdGetAsync(userId);
            return _mapper.Map<List<EventDto>>(events);
        }

        public async Task<List<PartialOrganizationDto>> GetOrganizationsFromMemberId(string id)
        {
            var orgas = _mapper.Map<List<PartialOrganizationDto>>(await _organizationRepository.ApiOrganizationsMembersByUserIdGetAsync(id));

            List<PartialOrganizationDto> orgaList = new List<PartialOrganizationDto>();
            foreach (var orga in orgas)
            {
                orga.MissionsNb = await GetOrganizationMissionCount(orga.Id);
                orgaList.Add(orga);
            }
            return orgaList;
        }

        public async Task<int> GetOrganizationMissionCount(string id)
        {
            var count = await _missionRepository.ApiMissionsCountOrganizationsByOrganizationIdByIsActiveGetAsync(id, true);
            return count.Value;
        }

        public async Task<int> GetMissionCountFromUser(string userId, bool isActive)
        {
            var count = await _missionRepository.ApiMissionsCountUsersByUserIdByIsActiveGetAsync(userId, true);
            return count.Value;
        }

        public async Task<SalesDashboardDto> GetSalesDashboard(string userId)
        {
            var user = await _userRepository.ApiUsersByIdGetAsync(userId);
            if (user is null)
            {
                throw new BusinessException(ErrorMessages.userDoesntExist);
            }

            if (user.GlobalRole != UserDto.GlobalRoleEnum.Sales)
            {
                throw new BusinessException(ErrorMessages.wrongUserRole);
            }

            var organization = _organizationRepository.GetOrganization(user.Id);
            if (organization is null)
            {
                throw new BusinessException(ErrorMessages.organizationDoesntExist);
            }

            var missions = await _missionRepository.ApiMissionsOrganizationsByIdGetAsync(organization.Id);
            var activities = await _activityRepository.ApiActivitiesMissionsByMissionIdsGetAsync(missions.Select(m => m.Id).ToList());

            var totalRevenue = 0.0;
            var totalWorkedDays = 0.0;
            var totalDays = 0;

            missions.ForEach(mission =>
            {
                var selectedActivities = activities.Where(a => a.MissionId == mission.Id);

                totalDays += selectedActivities.Sum(a =>
                {
                    return a.Days.Sum(d => d.IsOpen ? 1 : 0);
                });

                var missionWorkedDays = selectedActivities.Sum(a =>
                {
                    return a.Days.Sum(d =>
                    {
                        switch (d.WorkedPart)
                        {
                            case DayPartEnum.Afternoon:
                            case DayPartEnum.Morning:
                                return 0.5;
                            case DayPartEnum.Full:
                                return 1;
                            default:
                                return 0;
                        }
                    });
                });

                totalWorkedDays += missionWorkedDays;
                totalRevenue += missionWorkedDays * mission.DailyPrice;
            });

            return new SalesDashboardDto()
            {
                Missions = missions,
                SalesRevenue = totalRevenue,
                WorkedDays = totalWorkedDays,
                TotalDays = totalDays,
                Ratio = totalDays != 0 ? (totalWorkedDays / totalDays) : 0
            };
        }

        public async Task<DashboardResponseDto> GetDashboardData(string userId)
        {
            // First we need all the activities
            var activities = await _activityRepository.ApiActivitiesUsersByUserIdGetAsync(userId);
            if (activities is null || !activities.Any())
                return new DashboardResponseDto();

            // Cleaning the object so we reduce the response size
            activities.ForEach(x =>
            {
                x.Days = null;
                x.ModificationProposals = null;
                if (x.Customer != null)
                    x.Customer.CanSign = x.Status == ActivityStatusEnum.ConsultantSigned && x.Customer.Id == userId;
                if (x.Consultant != null)
                    x.Consultant.CanSign = x.Status == ActivityStatusEnum.Generated && x.Consultant.Id == userId;
            });
            activities = activities.OrderByDescending(x => x.StartDate).ToList();

            // Then we compute the indicators for the consultant
            var yearTotalDaysWorked = activities
                .Where(x => x.StartDate.Year == DateTime.UtcNow.Year) // Current year 
                .Sum(a => a.DaysNumber);
            yearTotalDaysWorked = (float)Math.Round(yearTotalDaysWorked, 2);
            var avgDaysWorked = yearTotalDaysWorked / activities.Count(x => x.StartDate.Year == DateTime.UtcNow.Year); // total days / nb of activities this year
            avgDaysWorked = (float)Math.Round(avgDaysWorked, 2);

            // Getting current month activity
            var currentMonthActivity = activities.FirstOrDefault(x => x.CreationDate.Year == DateTime.UtcNow.Year && x.CreationDate.Month == DateTime.UtcNow.Day);
            float currentMonthWorkedDays = 0;
            if (currentMonthActivity != null)
                currentMonthWorkedDays = currentMonthActivity.DaysNumber;

            // Getting an active activity for the month, if any
            var pendingActivity = activities.FirstOrDefault(x =>
                x.Status == ActivityStatusEnum.Generated &&
                x.CreationDate.Year == DateTime.UtcNow.Year &&
                x.CreationDate.Month == DateTime.UtcNow.Month);

            // Getting the trends
            // getting all the days worked for the last year
            var lastYearActivities = activities.Where(x => x.CreationDate.Year == DateTime.UtcNow.AddYears(-1).Year);
            float lastYearWorkedDays = 0f;
            float yearTotalDaysWorkedTrend = 0f;
            float lastYearAvgWorkedDays = 0f;
            float lastYearAvgWorkedDaysTrend = 0f;

            if (lastYearActivities != null && lastYearActivities.Any())
            {
                lastYearWorkedDays = lastYearActivities.Sum(x => x.DaysNumber);
                yearTotalDaysWorkedTrend = yearTotalDaysWorked == 0 ? 0 : (float)Math.Round(lastYearWorkedDays / yearTotalDaysWorked, 2);
                // average worked days last year
                lastYearAvgWorkedDays = lastYearWorkedDays / lastYearActivities.Count();
                lastYearAvgWorkedDaysTrend = avgDaysWorked == 0 ? 0 : (float)Math.Round(lastYearAvgWorkedDays / avgDaysWorked, 2);
            }

            // Wrapping up data
            DashboardNewActivityBannerDto bannerDto = null;
            if (pendingActivity != null)
                bannerDto = new DashboardNewActivityBannerDto()
                {
                    ActivityId = pendingActivity.Id
                };

            var dashboardData = new DashboardResponseDto()
            {
                Activities = activities,
                Indicators = new DashboardIndicatorsDto()
                {
                    Year = new DashboardYearIndicatorDto()
                    {
                        AverageDaysWorked = new DaysWorkedIndicatorDto()
                        {
                            Value = avgDaysWorked,
                            Trend = lastYearAvgWorkedDaysTrend
                        },
                        TotalDaysWorked = new DaysWorkedIndicatorDto()
                        {
                            Value = yearTotalDaysWorked,
                            Trend = yearTotalDaysWorkedTrend
                        }
                    },
                    Month = new DashboardMonthIndicatorDto()
                    {
                        TotalDaysWorked = new DaysWorkedIndicatorDto()
                        {
                            Value = currentMonthWorkedDays
                        }
                    }
                },
                NewActivityBanner = bannerDto
            };
            return dashboardData;
        }

        private bool CheckIfUserCanSign(string userId, ActivityDto activity)
        {
            // consultant
            if (activity.Consultant.Id == userId)
            {
                return (activity.Status == ActivityStatusEnum.Generated);
            }// customer
            else if (!string.IsNullOrEmpty(activity.Customer.Id) && activity.Customer.Id == userId)
            {
                return activity.Status == ActivityStatusEnum.ConsultantSigned && activity.Consultant.SignatureDate.HasValue;
            }
            else
            {
                return false;
            }
        }
    }
}
