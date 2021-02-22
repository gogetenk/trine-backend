using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Operational.Dal.MongoImpl.Configurations;
using Assistance.Operational.Dal.MongoImpl.Entities;
using Assistance.Operational.Dal.Repositories;
using AutoMapper;
using Dto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sogetrel.Sinapse.Framework.Dal.MongoDb;
using Sogetrel.Sinapse.Framework.Dal.MongoDb.Repositories;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Dal.MongoImpl.Repositories
{
    public class ActivityRepository : Impl.Repositories.CrudRepositoryBase<Activity>, IActivityRepository
    {
        private readonly DatabaseConfiguration _config;
        protected override string CollectionName => _config.ActivitiesCollectionName;

        public ActivityRepository(IMongoDbContext mongoDbContext, ILogger<RepositoryBase> logger, IMapper mapper, IOptionsSnapshot<DatabaseConfiguration> config) : base(mongoDbContext, logger, mapper)
        {
            _config = config.Value;
        }

        public async Task ApiActivitiesByIdDeleteAsync(string id, string apiVersion = null)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException(nameof(id));

            Delete(id);
        }

        public async Task<ActivityDto> ApiActivitiesByIdGetAsync(string id, string apiVersion = null)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException(nameof(id));

            return Mapper.Map<ActivityDto>(GetById(id));
        }

        public async Task<ActivityDto> ApiActivitiesByIdPatchAsync(string id, ActivityDto activity, string apiVersion = null)
        {
            if (activity == null || string.IsNullOrEmpty(id))
                throw new ArgumentNullException();

            var oldActivity = GetById(id);
            if (oldActivity is null)
                throw new DalException("The specified activity has to exist before updating it. Please use insert instead of update.");

            var success = Update(id, Mapper.Map<Entities.Activity>(activity));
            if (!success)
                throw new DalException("Error while updating activity report");

            return Mapper.Map<ActivityDto>(GetById(id));
        }

        public async Task<ActivityDto> ApiActivitiesByStartDateendDateGetAsync(DateTime startDate, DateTime endDate, string apiVersion = null)
        {
            // appel d'un WS d'api de jours ouvrés en fonction du pays
            //var holidays = new HolidayChecker(_configuration).GetHolidays("FR"); //TODO: à gerer ailleurs
            //if (holidays == null || !holidays.Any())
            //    throw new BusinessException("Country not supported");

            var activity = new ActivityDto();
            activity.Days = Enumerable.Range(1, DateTime.DaysInMonth(startDate.Year, startDate.Month))
                    .Select(day => new DateTime(startDate.Year, startDate.Month, day, 0, 0, 0, DateTimeKind.Utc))
                    .Select(x => new GridDayDto()
                    {
                        Day = x,
                        IsOpen = !(x.DayOfWeek == DayOfWeek.Saturday || x.DayOfWeek == DayOfWeek.Sunday /*|| holidays.Contains(x)*/)
                    })
                    .ToList();

            //// Suppression des jours feriés et WE
            //activity.Days
            //    .Where(x => x.Day.DayOfWeek == DayOfWeek.Saturday || x.Day.DayOfWeek == DayOfWeek.Sunday || holidays.Contains(x.Day))
            //    .ToList()
            //    .ForEach(x => activity.Days.Select(y => y.IsOpen = false));

            activity.CreationDate = DateTime.UtcNow;
            activity.StartDate = startDate;
            activity.EndDate = endDate;

            return activity;
        }

        public async Task<List<ActivityDto>> ApiActivitiesDatesGetAsync(DateTime startDate, DateTime endDate, string apiVersion = null)
        {
            if (endDate <= startDate)
                throw new ArgumentException("the start date must be before the end date");

            var activities = FindBy(a => a.StartDate >= startDate && a.EndDate <= endDate);
            return Mapper.Map<List<ActivityDto>>(activities);
        }

        public async Task<List<ActivityDto>> ApiActivitiesGetAsync(string apiVersion = null)
        {
            return Mapper.Map<List<ActivityDto>>(FindBy(x => true));
        }

        public async Task<ActivityDto> ApiActivitiesMissionsByMissionIdByDateGetAsync(string missionId, DateTime date, string apiVersion = null)
        {
            var startDt = new DateTime(date.Year, date.Month, 1);
            DateTime lastDayOfMonth = startDt.AddMonths(1).AddDays(-1);
            return Mapper.Map<ActivityDto>(FindBy(x => x.MissionId == missionId && x.CreationDate >= startDt && x.CreationDate <= lastDayOfMonth).FirstOrDefault());
        }

        public async Task<List<ActivityDto>> ApiActivitiesMissionsByMissionIdGetAsync(string missionId, string apiVersion = null)
        {
            if (string.IsNullOrEmpty(missionId))
                throw new ArgumentException(nameof(missionId));

            return Mapper.Map<List<ActivityDto>>(FindBy(x => x.MissionId == missionId));
        }

        public async Task<List<ActivityDto>> ApiActivitiesMissionsByMissionIdsGetAsync(List<string> missionIds, string apiVersion = null)
        {
            if (missionIds == null)
                throw new ArgumentNullException(nameof(missionIds));

            return Mapper.Map<List<ActivityDto>>(FindBy(x => missionIds.Contains(x.MissionId)));
        }

        public async Task<ActivityDto> ApiActivitiesPostAsync(ActivityDto activity = null, string apiVersion = null)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity));

            // On force le utc
            if (activity.Days != null)
            {
                List<GridDayDto> days = new List<GridDayDto>();
                foreach (var day in activity.Days)
                {
                    DateTime utcDay = day.Day;
                    utcDay = DateTime.SpecifyKind(day.Day, DateTimeKind.Utc);
                    day.Day = utcDay;
                    days.Add(day);
                }
                activity.Days = days;
            }

            return Mapper.Map<ActivityDto>(Insert(Mapper.Map<Activity>(activity)));
        }

        public async Task<List<ActivityDto>> ApiActivitiesUsersByUserIdGetAsync(string userId, string apiVersion = null)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException(nameof(userId));

            var activities = FindBy(x => (x.Consultant.Id == userId
                || x.Commercial.Id == userId
                || x.Customer.Id == userId));

            return Mapper.Map<List<ActivityDto>>(activities);
        }

        public async Task<List<ActivityDto>> ApiActivitiesUsersByUserIdsByDateGetAsync(List<string> userIds, DateTime date, string apiVersion = null)
        {
            if (userIds == null || !userIds.Any())
                throw new ArgumentException(nameof(userIds));

            var dt = new DateTime(date.Year, date.Month, 1);
            return Mapper.Map<List<ActivityDto>>(FindBy(x => userIds.Contains(x.Consultant.Id) && x.CreationDate >= dt));

        }

        public async Task<List<ActivityDto>> ApiActivitiesUsersByUserIdsGetAsync(List<string> userIds, string apiVersion = null)
        {

            if (userIds == null || !userIds.Any())
                throw new ArgumentException(nameof(userIds));

            return Mapper.Map<List<ActivityDto>>(FindBy(x =>
                userIds.Contains(x.Consultant.Id) ||
                userIds.Contains(x.Commercial.Id) ||
                userIds.Contains(x.Customer.Id))
            );
        }

        public async Task<List<ActivityDto>> GetByEmail(string email)
        {
            var activities = FindBy(x =>
                x.Consultant.Email == email ||
                x.Commercial.Email == email ||
                x.Customer.Email == email);

            return Mapper.Map<List<ActivityDto>>(activities);
        }
    }
}
