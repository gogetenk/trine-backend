namespace WebApi.Controllers
{
    //[Route("api/admin")]
    //[ApiController]
    //public class AdminController : ControllerBase
    //{
    //    private readonly IUserService _userService;

    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="logger"></param>
    //    /// <param name="mapper"></param>
    //    /// <param name="userService"></param>
    //    public AdminController(ILogger<ControllerBase> logger,
    //        IMapper mapper,
    //        IUserService userService
    //        )
    //    {
    //        _userService = userService;
    //    }

    //    [HttpGet("users/{id}/token")]
    //    [ProducesResponseType(200, Type = typeof(TokenDto))]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> GetUserToken([FromRoute] string id)
    //    {
    //        var token = await _userService.RequestToken(id);
    //        return Ok(token);
    //    }

    //    [HttpGet("users")]
    //    [ProducesResponseType(200, Type = typeof(List<UserAdminDto>))]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> GetAll()
    //    {
    //        var users = await _userService.GetAllDataUsers();

    //        if (users == null)
    //            return NotFound(ErrorMessages.notFound);
    //        else
    //            return Ok(users);
    //    }

    //}
}