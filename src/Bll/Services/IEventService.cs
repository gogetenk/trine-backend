using System.Threading.Tasks;
using Dto;

namespace Assistance.Operational.Bll.Services
{
    public interface IEventService
    {
        Task Update(string eventId, EventDto eventDto);
    }
}
