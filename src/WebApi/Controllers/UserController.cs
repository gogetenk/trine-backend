using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Assistance.Operational.Bll.Impl.Errors;
using Assistance.Operational.Bll.Services;
using AutoMapper;
using Dto;
using Dto.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Assistance.Operational.WebApi.Controllers
{
    /// <summary>
    /// Service that manages users
    /// </summary>
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IInviteService _inviteService;
        private readonly IOrganizationService _organizationService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="userService"></param>
        /// <param name="inviteService"></param>
        /// <param name="organizationService"></param>
        public UserController(ILogger<ControllerBase> logger,
            IMapper mapper,
            IUserService userService,
            IInviteService inviteService,
            IOrganizationService organizationService
            )
        {
            _userService = userService;
            _inviteService = inviteService;
            _organizationService = organizationService;
        }

        ///// <summary>
        ///// Unsubscribe an user
        ///// </summary>
        ///// <returns>The mission</returns>
        //[HttpDelete]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (string.IsNullOrEmpty(id))
        //        throw new System.ArgumentException("Id", nameof(id));

        //    await _userService.DeleteUser(id);
        //    return Ok();
        //}

        /// <summary>
        /// Creates an user
        /// </summary>
        /// <returns>The created Id</returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(201, Type = typeof(string))]
        public async Task<IActionResult> Post(CreateUserRequestDto dto)
        {
            if (dto.SecretToken != "6C6BC8B4-DB72-4657-9A15-98922AC90D70")
                return BadRequest("Wrong secret.");

            var id = await _userService.CreateUser(dto.User);
            return Ok(id);
        }

        /// <summary>
        /// Search users 
        /// </summary>
        /// <returns>The mission</returns>
        [HttpGet("search")]
        [ProducesResponseType(200, Type = typeof(List<UserDto>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(string email = null, string firstname = null, string lastname = null, string companyName = null)
        {
            List<UserDto> users = await _userService.SearchUsers(email, firstname, lastname, companyName);

            if (users == null)
                return NotFound(ErrorMessages.notFound);
            else
                return Ok(users);
        }

        //[HttpGet("{id}/invites")]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(200, Type = typeof(List<InviteDto>))]
        //public async Task<IActionResult> GetPendingInvitations([FromRoute] string id)
        //{
        //    var invites = await _inviteService.GetUserInvitesAsync(id);
        //    if (invites == null || invites.Count == 0)
        //        return NoContent();

        //    return Ok(invites);
        //}

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UserDto user)
        {
            await _userService.UpdateUserAsync(id, user);
            return Ok();
        }

        //[HttpPost("me/required-data")]
        //[ProducesResponseType(200)]
        //public async Task<IActionResult> UpdateRequiredData(UserRequiredDataDto dto)
        //{
        //    var userId = GetCurrentUserId();
        //    await _userService.UpdateRequiredData(userId, dto.Password, dto.PhoneNumber);
        //    return Ok();
        //}

        //[HttpPut("me/password")]
        //[ProducesResponseType(200)]
        //public async Task<IActionResult> UpdatePassword(UpdatePasswordDto values)
        //{
        //    var userId = GetCurrentUserId();
        //    await _userService.UpdatePassword(userId, values.Password);
        //    return Ok();
        //}

        ///// <summary>
        ///// Get user organization
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("me/organization")]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(401)]
        //public IActionResult GetOrganization()
        //{
        //    var userId = GetCurrentUserId();
        //    var organization = _organizationService.GetByUserId(userId);
        //    if (organization is null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(organization);
        //}

        private string GetCurrentUserId()
        {
            var id = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "https://trineapp.io/trine_id")?.Value
                ?? throw new AuthenticationException("The Trine id couldn't be found.");
            return id;
        }
    }
}
