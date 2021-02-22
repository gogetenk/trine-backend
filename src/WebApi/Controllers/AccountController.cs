using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Assistance.Operational.Bll.Services;
using AutoMapper;
using Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Assistance.Operational.WebApi.Controllers
{
    /// <summary>
    /// Handles every aspect of the account creation and users/companies management
    /// </summary>
    [Authorize]
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="accountService"></param>
        public AccountController(IConfiguration configuration, ILogger<ControllerBase> logger, IMapper mapper, IAccountService accountService)
        {
            _configuration = configuration;
            _accountService = accountService;
        }

        ///// <summary>
        ///// Registers a new user
        ///// </summary>
        //[AllowAnonymous]
        //[HttpPost("users/register")]
        //[ProducesResponseType(201, Type = typeof(CreatedUserDto))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(500)]
        //public async Task<IActionResult> RegisterUser(RegisterUserRequestDto dto)
        //{
        //    var result = await _accountService.RegisterUser(dto);
        //    if (result is null)
        //        return Conflict();

        //    return Created("/users/", result);
        //}

        ///// <summary>
        ///// Logs an user in from its email and password, and returns a bearer token.
        ///// </summary>
        ///// <param name="login"></param>
        ///// <returns>A bearer token</returns>
        //[AllowAnonymous]
        //[HttpPost("login")]
        //[ProducesResponseType(200, Type = typeof(TokenDto))]
        //[ProducesResponseType(401)]
        //[ProducesResponseType(500)]
        //public async Task<IActionResult> CreateToken([FromBody] UserCredentialsDto login)
        //{
        //    if (login == null)
        //        return BadRequest("Mandatory field missing.");

        //    var token = await _accountService.AuthenticateUser(login);
        //    if (token == null)
        //        return Unauthorized();
        //    else
        //        return Ok(token);
        //}

        ///// <summary>
        ///// Sends a subscription invitation to the specified emails
        ///// </summary>
        //[HttpPost("email-invite")]
        //[ProducesResponseType(200)]
        //public async Task<IActionResult> SendEmailInvitations(SubscriptionInvitationDto dto)
        //{
        //    await _accountService.SendEmailInvitations(dto, GetCurrentUserId());
        //    return Ok();
        //}

        ///// <summary>
        ///// Sends a password recovery email to the user
        ///// </summary>
        //[AllowAnonymous]
        //[HttpPost("users/password")]
        //[ProducesResponseType(202)]
        //[ProducesResponseType(404, Type = typeof(ForgotPasswordResult))]
        //[ProducesResponseType(500)]
        //public async Task<IActionResult> RecoverPassword(ForgotPasswordDto dto)
        //{
        //    var result = await _accountService.SendPasswordRecoveryEmail(dto);
        //    if (result)
        //        return Ok();
        //    else
        //        return NoContent();
        //}

        ///// <summary>
        ///// Updates an user's forgotten password from a token
        ///// </summary>
        //[AllowAnonymous]
        //[HttpGet("users/password/{token}")]
        //[Produces("text/html")]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //public async Task<IActionResult> UpdateUserPassword(string token)
        //{
        //    string email = await _accountService.UpdateUserPassword(token);
        //    return Redirect($"{_configuration["WebApp:Url"]}/app/login?email={email}&mode=1");
        //}

        /// <summary>
        /// Gets an user from its Id
        /// </summary>
        [HttpGet("users/{id}")]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUser(string id)
        {
            var result = await _accountService.GetUserById(id);
            return Ok(result);
        }

        /// <summary>
        /// Checks if an user already exist
        /// </summary>
        [AllowAnonymous]
        [HttpGet("users/exists")]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CheckUserExists(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Argument cannot be null");

            var user = await _accountService.CheckMail(email);
            if (user == null)
                return NotFound();

            return Ok(user);
        }



        private string GetCurrentUserId()
        {
            var id = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "https://trineapp.io/trine_id")?.Value
                ?? throw new AuthenticationException("The Trine id couldn't be found.");
            return id;
        }
    }
}
