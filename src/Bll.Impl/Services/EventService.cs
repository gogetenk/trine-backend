using System.Threading.Tasks;
using Assistance.Operational.Bll.Services;
using Assistance.Operational.Dal.Repositories;
using Dto;
using Microsoft.Extensions.Logging;
using Sogetrel.Sinapse.Framework.Bll.Services;

namespace Assistance.Operational.Bll.Impl.Services
{
    public class EventService : ServiceBase, IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(ILogger<ServiceBase> logger, IEventRepository eventRepository) : base(logger)
        {
            _eventRepository = eventRepository;
        }

        public async Task Update(string eventId, EventDto eventDto)
        {
            await _eventRepository.ApiEventsByIdPutAsync(eventId, eventDto);
        }
    }
}
