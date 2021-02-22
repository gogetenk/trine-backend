namespace Assistance.Operational.WebApi.Controllers
{
    ///// <summary>
    ///// Handles every aspect of the account creation and users/companies management
    ///// </summary>
    //[Authorize]
    //[Route("api/organizations")]
    //[ApiController]
    //[ExcludeFromCodeCoverage]
    //public class OrganizationController : ControllerBase
    //{
    //    private readonly IMapper _mapper;
    //    private readonly IOrganizationService _organizationService;
    //    private readonly IInviteService _inviteService;
    //    private readonly IAccountService _accountService;
    //    private readonly IConfiguration _configuration;

    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="logger"></param>
    //    /// <param name="mapper"></param>
    //    public OrganizationController(IConfiguration configuration, ILogger<ControllerBase> logger,
    //        IMapper mapper, IOrganizationService organizationService, IInviteService inviteService, IAccountService accountService)
    //    {
    //        _configuration = configuration;
    //        _organizationService = organizationService;
    //        _inviteService = inviteService;
    //        _accountService = accountService;
    //    }

    //    /// <summary>
    //    /// Gets organization by its id.
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpGet("{organizationId}")]
    //    [ProducesResponseType(200, Type = typeof(OrganizationDto))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> Get(string organizationId)
    //    {
    //        var result = await _organizationService.GetById(organizationId);
    //        return Ok(result);
    //    }

    //    /// <summary>
    //    /// Gets organizations by their id.
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpGet("many/")]
    //    [ProducesResponseType(200, Type = typeof(List<OrganizationDto>))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> Get(List<string> ids)
    //    {
    //        if (!ids.Any())
    //            return BadRequest("Mandatory parameter missing");

    //        return Ok(await _organizationService.GetByIds(ids));
    //    }


    //    /// <summary>
    //    /// Creates an organization.
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpPost]
    //    [ProducesResponseType(200, Type = typeof(OrganizationDto))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> Post([FromBody] OrganizationDto organization)
    //    {
    //        return Ok(await _organizationService.Insert(organization));
    //    }

    //    /// <summary>
    //    /// Finds and replaces an organization.
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpPut("{id}")]
    //    [ProducesResponseType(200, Type = typeof(OrganizationDto))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> Put([FromRoute] string id, [FromBody] OrganizationDto organization)
    //    {
    //        return Ok(await _organizationService.Update(id, organization));
    //    }

    //    /// <summary>
    //    /// Gets an organization member.
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpGet("{organizationId}/members/{userId}")]
    //    [ProducesResponseType(200, Type = typeof(OrganizationMemberDto))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> Get(string organizationId, string userId)
    //    {
    //        return Ok(await _organizationService.GetMember(organizationId, userId));
    //    }

    //    /// <summary>
    //    /// Gets all members of an organization.
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpGet("{organizationId}/members")]
    //    [ProducesResponseType(200, Type = typeof(List<UserDto>))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> GetMembers(string organizationId)
    //    {
    //        return Ok(await _organizationService.GetAllMembers(organizationId));
    //    }

    //    /// <summary>
    //    /// Accept organization invite
    //    /// </summary>
    //    /// <param name="code"></param>
    //    /// <returns></returns>
    //    [HttpGet("{organizationId}/join")]
    //    [Produces("text/html")]
    //    [ProducesResponseType(200)]
    //    public async Task<IActionResult> AcceptInvite(string code)
    //    {
    //        try
    //        {
    //            var user = await _inviteService.AcceptInvite(code);
    //            return Redirect($"{_configuration["WebApp:Url"]}{_configuration["WebApp:DashboardPath"]}");
    //        }
    //        catch (BusinessException ex)
    //        {
    //            if (ex.Message == ErrorMessages.userDoesntExist)
    //            {
    //                return Redirect($"{_configuration["WebApp:Url"]}/app/join");
    //            }
    //        }
    //        return Redirect($"{_configuration["WebApp:Url"]}/app/invitations/error");
    //    }

    //    ///// <summary>
    //    ///// Gets total of organization's members.
    //    ///// </summary>
    //    ///// <returns></returns>
    //    //[HttpGet("{organizationId}/members/count")]
    //    //[ProducesResponseType(200)]
    //    //[ProducesResponseType(400)]
    //    //[ProducesResponseType(404)]
    //    //public async Task<IActionResult> GetMembersCount(string organizationId)
    //    //{
    //    //    return Ok(await _organizationService.GetMembersCount(organizationId));
    //    //}

    //    /// <summary>
    //    /// Adds an organization member.
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpPost("join")]
    //    [ProducesResponseType(200, Type = typeof(AcceptInvitationResult))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> AddMember([FromBody] JoinOrganizationRequestDto request)
    //    {
    //        if (string.IsNullOrEmpty(request.UserId))
    //        {
    //            var credentials = await _accountService.RegisterUser(request.AccountData);
    //            return Ok(new AcceptInvitationResult()
    //            {
    //                Credentials = credentials.Token,
    //                IsCreated = true
    //            });
    //        }
    //        else
    //        {
    //            await _inviteService.AcceptInvite(request.Code);
    //            return Ok(new AcceptInvitationResult()
    //            {
    //                IsCreated = false
    //            });
    //        }
    //    }

    //    /// <summary>
    //    /// Modifies an organization member.
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpPatch("{organizationId}/members")]
    //    [ProducesResponseType(200, Type = typeof(OrganizationMemberDto))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> ModifyMember([FromRoute] string organizationId, [FromBody] OrganizationMemberDto member)
    //    {
    //        return Ok(await _organizationService.UpdateMember(organizationId, member));
    //    }

    //    /// <summary>
    //    /// Partially updates an organization.
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpPatch("{organizationId}")]
    //    [ProducesResponseType(200, Type = typeof(OrganizationMemberDto))]
    //    public async Task<IActionResult> PartialUpdate([FromRoute] string organizationId, [FromBody] OrganizationDto organization)
    //    {
    //        return Ok(await _organizationService.PartialUpdate(organizationId, organization));
    //    }

    //    /// <summary>
    //    /// Deletes an organization member.
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpDelete("{organizationId}/members/{userId}")]
    //    [ProducesResponseType(200, Type = typeof(OrganizationMemberDto))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public IActionResult DeleteMember(string organizationId, string userId)
    //    {
    //        _organizationService.RemoveMember(organizationId, userId);
    //        return Ok();
    //    }

    //    /// <summary>
    //    /// Gets invite by its code.
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpGet("invites/{code}")]
    //    [ProducesResponseType(200, Type = typeof(InviteResponseDto))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> Get(Guid code)
    //    {
    //        return Ok(await _inviteService.Get(code));
    //    }

    //    /// <summary>
    //    /// Creates an invite.
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpPost("{organizationId}/invites")]
    //    [ProducesResponseType(200, Type = typeof(InviteDto[]))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> Post(string organizationId, [FromBody] CreateInvitationRequestDto request)
    //    {
    //        if (request.Email.Contains(','))
    //        {
    //            string[] mails = request.Email.Split(',');
    //            var invites = await _inviteService.CreateBulkInvite(mails, request.InviterId, organizationId);
    //            return Ok(invites);
    //        }

    //        var invite = await _inviteService.CreateInvite(request.Email, request.InviterId, organizationId);
    //        InviteDto[] results = new InviteDto[1];
    //        results[0] = invite;
    //        return Ok(results);
    //    }

    //    /// <summary>
    //    /// Gets all the invites of an organization.
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpGet("{organizationId}/invites")]
    //    [ProducesResponseType(200, Type = typeof(List<InviteDto>))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> GetInvites(string organizationId)
    //    {
    //        return Ok(await _inviteService.GetByOrganizationId(organizationId));
    //    }

    //    /// <summary>
    //    /// Deletes an organization.
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpDelete("{organizationId}")]
    //    [ProducesResponseType(200)]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> Delete(string organizationId)
    //    {
    //        await _organizationService.Delete(organizationId);
    //        return Ok();
    //    }

    //}
}
