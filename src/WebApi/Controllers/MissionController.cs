namespace Assistance.Operational.WebApi.Controllers
{
    ///// <summary>
    ///// Manages the mission (details and listing) page
    ///// </summary>
    //[Authorize]
    //[Route("api/missions")]
    //[ApiController]
    //public class MissionController : ControllerBase
    //{
    //    private readonly IMapper _mapper;
    //    private readonly IMissionService _missionService;

    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="logger"></param>
    //    /// <param name="mapper"></param>
    //    /// <param name="missionService"></param>
    //    public MissionController(ILogger<ControllerBase> logger, IMapper mapper, IMissionService missionService)
    //    {
    //        _mapper = mapper;
    //        _missionService = missionService;
    //    }

    //    /// <summary>
    //    /// Get a mission aggregate root from it's id
    //    /// </summary>
    //    /// <returns>The mission</returns>
    //    [HttpGet("{id}")]
    //    [ProducesResponseType(200, Type = typeof(MissionDto))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> Get(string id)
    //    {
    //        if (string.IsNullOrEmpty(id))
    //            return BadRequest(ErrorMessages.mandatoryParameterMissing);

    //        MissionDto mission = await _missionService.GetMissionAggregate(id);

    //        if (mission == null)
    //            return NotFound(ErrorMessages.notFound);
    //        else
    //            return Ok(mission);
    //    }



    //    /// <summary>
    //    /// Gets all missions of the specified organization.
    //    /// </summary>
    //    /// <param name="id">Organization Id</param>
    //    /// <returns>Missions</returns>
    //    [HttpGet("organizations/{id}")]
    //    [ProducesResponseType(200, Type = typeof(List<MissionDto>))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> GetFromOrganization(string id)
    //    {
    //        var result = await _missionService.GetFromOrganizationId(id);
    //        return Ok(result);
    //    }


    //    /// <summary>
    //    /// Creates a mission and returns its id.
    //    /// </summary>
    //    /// <returns>The created mission id</returns>
    //    [HttpPost]
    //    [ProducesResponseType(201, Type = typeof(MissionDto))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(422)]
    //    public async Task<IActionResult> Post(CreateMissionRequestDto createMissionRequestDto)
    //    {
    //        if (createMissionRequestDto == null)
    //            return BadRequest(ErrorMessages.mandatoryParameterMissing);

    //        var dto = await _missionService.CreateMission(createMissionRequestDto);

    //        if (dto is null)
    //            return UnprocessableEntity(ErrorMessages.couldNotProcess);
    //        else
    //            return Created($"missions/{dto.Id}", dto);
    //    }

    //    [HttpPut("{id}/customer")]
    //    [ProducesResponseType(200, Type = typeof(MissionDto))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(422)]
    //    public async Task<IActionResult> UpdateMissionCustomer([FromRoute] string id, MissionCustomerDto dto)
    //    {
    //        var userId = GetCurrentUserId();
    //        await _missionService.UpdateCustomer(userId, id, dto);
    //        return Ok();
    //    }

    //    /// <summary>
    //    /// Updates a mission and returns its new value.
    //    /// </summary>
    //    /// <returns>The updates mission</returns>
    //    [HttpPut]
    //    [ProducesResponseType(200, Type = typeof(MissionDto))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(422)]
    //    public async Task<IActionResult> Put(string id, [FromBody] MissionDto dto)
    //    {
    //        if (dto == null || string.IsNullOrEmpty(id))
    //            return BadRequest(ErrorMessages.mandatoryParameterMissing);

    //        var mission = await _missionService.UpdateMission(id, dto);

    //        if (mission == null)
    //            return UnprocessableEntity(ErrorMessages.couldNotProcess);
    //        else
    //            return Ok(mission);
    //    }

    //    /// <summary>
    //    /// Invite and attach a consultant to a mission
    //    /// </summary>
    //    [HttpPut("{id}/consultant")]
    //    [ProducesResponseType(200, Type = typeof(MissionDto))]
    //    [ProducesResponseType(400)]
    //    public async Task<IActionResult> PutConsultant(string id, [FromBody] string userId)
    //    {
    //        if (string.IsNullOrEmpty(id))
    //            return BadRequest(ErrorMessages.mandatoryParameterMissing);

    //        await _missionService.UpdateConsultant(id, userId);
    //        return Ok();
    //    }

    //    /// <summary>
    //    /// Gets all the missions the specified user is involved in.
    //    /// </summary>
    //    /// <returns>A collection of missions</returns>
    //    [HttpGet("users/{id}")]
    //    [ProducesResponseType(200, Type = typeof(List<MissionDto>))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> GetMissionsFromUser(string id, bool isActive)
    //    {
    //        if (string.IsNullOrEmpty(id))
    //            return BadRequest(ErrorMessages.mandatoryParameterMissing);

    //        List<MissionDto> missions = new List<MissionDto>();
    //        missions = await _missionService.GetMissionsFromUser(id, isActive);
    //        return Ok(missions);
    //    }

    //    /// <summary>
    //    /// Gets the current mission of the consultant. (MVP Only ?)
    //    /// </summary>
    //    /// <returns>A collection of missions</returns>
    //    [HttpGet("consultant")]
    //    [ProducesResponseType(200, Type = typeof(MissionDto))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(401)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> GetCurrentMissionForConsultant()
    //    {
    //        string userId = GetCurrentUserId();
    //        if (string.IsNullOrEmpty(userId))
    //        {
    //            return Unauthorized();
    //        }
    //        var mission = await _missionService.GetUserCurrentMission(userId);
    //        return Ok(mission);
    //    }

    //    /// <summary>
    //    /// Flags a mission as inactive
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpPut("{id}")]
    //    [ProducesResponseType(200, Type = typeof(MissionDto))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> CancelMission(string id)
    //    {
    //        if (string.IsNullOrEmpty(id))
    //            return BadRequest(ErrorMessages.mandatoryParameterMissing);

    //        MissionDto mission = await _missionService.CancelMission(id);

    //        if (mission == null)
    //            return NotFound(ErrorMessages.notFound);
    //        else
    //            return Ok(mission);
    //    }

    //    /// <summary>
    //    /// Deletes a mission (used only for testing)
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpDelete("{id}")]
    //    [ProducesResponseType(200, Type = typeof(MissionDto))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> DeleteMission(string id)
    //    {
    //        if (string.IsNullOrEmpty(id))
    //            return BadRequest(ErrorMessages.mandatoryParameterMissing);

    //        await _missionService.DeleteMission(id);
    //        return Ok();
    //    }

    //    /// <summary>
    //    /// Gets a preview of the frame contract before creating the mission
    //    /// </summary>
    //    /// <param name="request"></param>
    //    /// <returns></returns>
    //    [HttpPost("contractPreview")]
    //    [ProducesResponseType(200, Type = typeof(FrameContractDto))]
    //    [ProducesResponseType(401)]
    //    [ProducesResponseType(500)]
    //    public async Task<IActionResult> GetContractPreview([FromBody] CreateMissionRequestDto request)
    //    {
    //        if (request == null)
    //            return BadRequest("Mandatory field missing.");

    //        var contract = await _missionService.GetContractPreview(request);

    //        if (contract == null)
    //            return BadRequest();
    //        else
    //            return Ok(contract);
    //    }

    //    private string GetCurrentUserId()
    //    {
    //        var id = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "https://trineapp.io/trine_id")?.Value
    //            ?? throw new AuthenticationException("The Trine id couldn't be found.");
    //        return id;
    //    }
    //}
}
