using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Operational.Dal.MongoImpl.Entities;
using Assistance.Operational.Dal.Repositories;
using AutoMapper;
using Dto;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Sogetrel.Sinapse.Framework.Dal.MongoDb;
using Sogetrel.Sinapse.Framework.Dal.MongoDb.Repositories;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Dal.MongoImpl.Repositories
{
    public class MissionRepository : Impl.Repositories.CrudRepositoryBase<Mission>, IMissionRepository
    {
        protected override string CollectionName => "missions";

        public MissionRepository(IMongoDbContext mongoDbContext, ILogger<RepositoryBase> logger, IMapper mapper)
            : base(mongoDbContext, logger, mapper)
        {
        }

        public async Task<List<MissionDto>> ApiMissionsActiveByActiveGetAsync(bool? active, string apiVersion = null)
        {
            var results = FindBy(x => x.Status != Mission.StatusEnum.CANCELED).ToList();
            return Mapper.Map<List<MissionDto>>(results);
        }

        public async Task ApiMissionsByIdDeleteAsync(string id, string apiVersion = null)
        {
            Delete(id);
        }

        public async Task<MissionDto> ApiMissionsByIdGetAsync(string id, string apiVersion = null)
        {
            return Mapper.Map<MissionDto>(GetById(id));
        }

        public async Task<List<MissionDto>> ApiMissionsByUserIdByActiveGetAsync(string userId, bool? active, string apiVersion = null)
        {
            var results = FindBy(x => x.Consultant.Id == userId || x.Commercial.Id == userId || x.Customer.Id == userId).ToList();
            return Mapper.Map<List<MissionDto>>(results);
        }

        // TODO: Prendre en compte le isactive
        public async Task<int?> ApiMissionsCountOrganizationsByOrganizationIdByIsActiveGetAsync(string organizationId, bool? isActive, string apiVersion = null)
        {
            if (!isActive.HasValue)
                return FindBy(x => x.OrganizationId == organizationId).Count();
            else if (isActive.Value)
                return FindBy(x => x.OrganizationId == organizationId && x.Status == Mission.StatusEnum.CONFIRMED || x.Status == Mission.StatusEnum.CREATED).Count();
            else
                return FindBy(x => x.OrganizationId == organizationId && x.Status == Mission.StatusEnum.CANCELED).Count();
        }

        // TODO: Prendre en compte le isactive
        public async Task<int?> ApiMissionsCountUsersByUserIdByIsActiveGetAsync(string userId, bool? isActive, string apiVersion = null)
        {
            var results = await ApiMissionsByUserIdByActiveGetAsync(userId, isActive);
            return results.Count;
        }

        public async Task<List<MissionDto>> ApiMissionsOrganizationsByIdGetAsync(string id, string apiVersion = null)
        {
            var results = FindBy(x => x.OrganizationId == id).ToList();
            return Mapper.Map<List<MissionDto>>(results);
        }

        public async Task<MissionDto> ApiMissionsPostAsync(MissionDto mission = null, string apiVersion = null)
        {
            var createdMission = Insert(Mapper.Map<Mission>(mission));
            return Mapper.Map<MissionDto>(mission);
        }

        public async Task<MissionDto> ApiMissionsPutAsync(string id = null, MissionDto mission = null, string apiVersion = null)
        {
            if (!Update(id, Mapper.Map<Mission>(mission)))
                throw new DalException("The update didn't succeeed");
            else
                return Mapper.Map<MissionDto>(GetById(id));
        }

        public async Task<List<MissionDto>> ApiMissionsUsersByIdGetAsync(string id, string apiVersion = null)
        {
            var results = FindBy(x => x.Consultant.Id == id || x.Commercial.Id == id || x.Customer.Id == id).ToList();
            return Mapper.Map<List<MissionDto>>(results);
        }

        public async Task<MissionDto> GetUserCurrentMission(string userId)
        {
            var result = FindBy(x => x.Consultant.Id == userId
            && x.EndDate > DateTime.UtcNow
            && x.Status == Mission.StatusEnum.CONFIRMED)
                .OrderBy(x => x.StartDate).FirstOrDefault();
            return Mapper.Map<MissionDto>(result);
        }
    }
}
