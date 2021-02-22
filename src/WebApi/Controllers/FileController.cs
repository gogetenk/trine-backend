namespace Assistance.Operational.WebApi.Controllers
{
    ///// <summary>
    ///// Service that manages files
    ///// </summary>
    //[Authorize]
    //[Route("api/files")]
    //[ApiController]
    //public class FileController : ControllerBase
    //{
    //    private readonly IFileService _fileService;

    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="logger"></param>
    //    /// <param name="mapper"></param>
    //    /// <param name="fileService"></param>
    //    public FileController(ILogger<ControllerBase> logger, IMapper mapper, IFileService fileService)
    //    {
    //        _fileService = fileService;
    //    }

    //    /// <summary>
    //    /// Uploads an user profile pic
    //    /// </summary>
    //    /// <returns>The mission</returns>
    //    [HttpPost("users/{id}/icon")]
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

    //    /// <summary>
    //    /// Uploads an organization profile pic
    //    /// </summary>
    //    /// <returns>The mission</returns>
    //    [HttpPost("organizations/{id}/icon")]
    //    [ProducesResponseType(200, Type = typeof(byte[]))]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> PostOrgaFile(string id, [FromForm] IFormFile file)
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
}
//}
