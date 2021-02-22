namespace Assistance.Operational.WebApi.Controllers
{
    ///// <summary>
    ///// Manages the events
    ///// </summary>
    //[Authorize]
    //[Route("api/events")]
    //[ApiController]
    //public class EventController : ControllerBase
    //{
    //    private readonly IEventService _eventService;

    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="logger"></param>
    //    /// <param name="mapper"></param>
    //    public EventController(ILogger<ControllerBase> logger, IMapper mapper, IEventService eventService)
    //    {
    //        _eventService = eventService;
    //    }

    //    /// <summary>
    //    /// Partially updates an event
    //    /// </summary>
    //    /// <param name="eventId">Event id</param>
    //    /// <param name="eventDto">Updated event</param>
    //    /// <returns></returns>
    //    [HttpPatch("events")]
    //    [ProducesResponseType(200, Type = typeof(List<EventDto>))]
    //    [ProducesResponseType(400)]
    //    public async Task<IActionResult> MarkEventAsRead(string eventId, EventDto eventDto)
    //    {
    //        if (string.IsNullOrEmpty(eventId))
    //            throw new ArgumentException("Parameter cannot be null", nameof(eventId));

    //        await _eventService.Update(eventId, eventDto);
    //        return Ok();
    //    }
    //}
}
