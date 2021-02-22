namespace Assistance.Operational.WebApi.Controllers
{
    ///// <summary>
    ///// Service that manages leads
    ///// </summary>
    //[AllowAnonymous]
    //[Route("api/leads")]
    //[ApiController]
    //public class LeadController : ControllerBase
    //{
    //    private readonly ILeadService _leadService;

    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="logger"></param>
    //    /// <param name="mapper"></param>
    //    /// <param name="userService"></param>
    //    public LeadController(ILogger<ControllerBase> logger, IMapper mapper, ILeadService leadService)
    //    {
    //        _leadService = leadService;
    //    }

    //    /// <summary>
    //    /// Register a new lead (now considered as an User in Intercom)
    //    /// </summary>
    //    /// <returns>The mission</returns>
    //    [HttpPost]
    //    [ProducesResponseType(200, Type = typeof(bool))]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> Post([FromBody] CreateLeadRequestDto dto)
    //    {
    //        var result = await _leadService.CreateLead(dto.Email);
    //        return Ok(result);
    //    }

    //    /// <summary>
    //    /// Register a new lead 
    //    /// </summary>
    //    /// <returns>The mission</returns>
    //    [HttpPost("files")]
    //    [ProducesResponseType(200, Type = typeof(byte[]))]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> PostFile(string id, [FromForm] IFormFile file)
    //    {
    //        var filePath = Path.GetTempFileName();
    //        byte[] profilePic;

    //        if (file.Length > 0)
    //        {
    //            using (var stream = new FileStream(filePath, FileMode.Create))
    //            {
    //                await file.CopyToAsync(stream);

    //                try
    //                {
    //                    byte[] bytes = new byte[stream.Length];
    //                    stream.Read(bytes, 0, Convert.ToInt32(stream.Length));
    //                    stream.Close();
    //                    profilePic = bytes;
    //                }
    //                finally
    //                {
    //                    stream.Close();
    //                }
    //            }
    //        }

    //        return Ok();
    //    }
    //}
}
