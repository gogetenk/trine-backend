using System.Collections.Generic;
using System.Threading.Tasks;
using Assistance.Operational.Dal.Repositories;
using AutoMapper;
using Dto;
using Microsoft.Extensions.Logging;
using Sogetrel.Sinapse.Framework.Dal.MongoDb;
using Sogetrel.Sinapse.Framework.Dal.MongoDb.Repositories;

namespace Assistance.Operational.Dal.MongoImpl.Repositories
{
    public class EventRepository : Impl.Repositories.CrudRepositoryBase<Entities.Event>, IEventRepository
    {
        protected override string CollectionName => "events";

        public EventRepository(IMongoDbContext mongoDbContext, ILogger<RepositoryBase> logger, IMapper mapper) : base(mongoDbContext, logger, mapper)
        {
        }

        public async Task ApiEventsByIdDeleteAsync(string id, string apiVersion = null)
        {
            Delete(id);
        }

        public async Task<EventDto> ApiEventsByIdGetAsync(string id, string apiVersion = null)
        {
            return Mapper.Map<EventDto>(GetById(id));
        }

        public async Task<EventDto> ApiEventsByIdPutAsync(string id, EventDto value, string apiVersion = null)
        {
            return Mapper.Map<EventDto>(Update(id, Mapper.Map<Entities.Event>(value)));
        }

        public async Task<int> ApiEventsContextsByContextIdCountGetAsync(string contextId, string apiVersion = null)
        {
            return FindBy(x => x.ContextId == contextId).Count;
        }

        public async Task<List<EventDto>> ApiEventsContextsByContextIdGetAsync(string contextId, string apiVersion = null)
        {
            return Mapper.Map<List<EventDto>>(FindBy(x => x.ContextId == contextId));
        }

        public async Task<List<EventDto>> ApiEventsGetAsync(string apiVersion = null)
        {
            return Mapper.Map<List<EventDto>>(FindBy(x => true));
        }

        public async Task<EventDto> ApiEventsPostAsync(EventDto value, string apiVersion = null)
        {
            return Mapper.Map<EventDto>(Insert(Mapper.Map<Entities.Event>(value)));
        }

        public async Task<int> ApiEventsTargetsByTargetIdCountGetAsync(string targetId, string apiVersion = null)
        {
            return FindBy(x => x.TargetId == targetId).Count;
        }

        public async Task<List<EventDto>> ApiEventsTargetsByTargetIdGetAsync(string targetId, string apiVersion = null)
        {
            return Mapper.Map<List<EventDto>>(FindBy(x => x.TargetId == targetId));
        }
    }
}
