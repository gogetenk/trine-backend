namespace Assistance.Operational.WebApi.Controllers
{
    ///// <summary>
    ///// Service that manages invoices
    ///// </summary>
    //[Authorize]
    //[Route("api/invoices")]
    //[ApiController]
    //public class InvoiceController : ControllerBase
    //{
    //    private readonly IInvoiceService _invoiceService;

    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="logger"></param>
    //    /// <param name="mapper"></param>
    //    /// <param name="invoiceService"></param>
    //    public InvoiceController(ILogger<ControllerBase> logger, IMapper mapper, IInvoiceService invoiceService)
    //    {
    //        _invoiceService = invoiceService;
    //    }

    //    /// <summary>
    //    /// Gets an invoice.
    //    /// </summary>
    //    /// <returns>The mission</returns>
    //    [HttpGet("{id}")]
    //    [ProducesResponseType(200, Type = typeof(InvoiceDto))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> Get(string id)
    //    {
    //        if (string.IsNullOrEmpty(id))
    //            return BadRequest(ErrorMessages.mandatoryParameterMissing);

    //        var dto = await _invoiceService.GetById(id);

    //        if (dto == null)
    //            return NotFound(ErrorMessages.notFound);
    //        else
    //            return Ok(dto);
    //    }

    //    /// <summary>
    //    /// Get by mission id, in the limit of the quantity specified
    //    /// </summary>
    //    /// <param name="missionId">Mission id to retreive the invoices from</param>
    //    /// <param name="quantity">The quantity of elements to return. Specifying zero will return all elements.</param>
    //    /// <returns>Invoice list</returns>
    //    [HttpGet("missions/{missionId}")]
    //    [ProducesResponseType(200, Type = typeof(List<InvoiceDto>))]
    //    [ProducesResponseType(400)]
    //    [ProducesResponseType(404)]
    //    public async Task<IActionResult> GetAllByMissionId(string missionId, int? quantity)
    //    {
    //        if (quantity < 0)
    //            quantity = 0;

    //        var results = await _invoiceService.GetByMissionId(missionId, quantity);
    //        return Ok(results);
    //    }

    //    /// <summary>
    //    /// Get all by multiple mission ids
    //    /// </summary>
    //    /// <param name="ids">id list</param>
    //    /// <returns>Invoice list</returns>
    //    [HttpGet("missions/{ids}")]
    //    [ProducesResponseType(200, Type = typeof(List<InvoiceDto>))]
    //    [ProducesResponseType(400)]
    //    public async Task<IActionResult> GetAllByMissionIds(List<string> ids)
    //    {
    //        var results = await _invoiceService.GetByMissionIds(ids.ToArray());
    //        return Ok(results);
    //    }

    //    /// <summary>
    //    /// Get all the invoices from the issuer id
    //    /// </summary>
    //    /// <param name="issuerId">The id of the issuer of the invoice(s)</param>
    //    /// <param name="quantity">The quantity of elements to return</param>
    //    /// <returns>Invoice list</returns>
    //    [HttpGet("issuers/{issuerId}")]
    //    [ProducesResponseType(200, Type = typeof(List<InvoiceDto>))]
    //    [ProducesResponseType(400)]
    //    public async Task<IActionResult> GetAllByIssuer(string issuerId, int? quantity)
    //    {
    //        if (quantity < 0)
    //            quantity = 0;

    //        var results = await _invoiceService.GetByIssuerId(issuerId, quantity);
    //        return Ok(results);
    //    }

    //    /// <summary>
    //    /// Get the invoices from the recipient id
    //    /// </summary>
    //    /// <param name="recepientId">The id of the recipient of the invoice(s)</param>
    //    /// <param name="quantity">The quantity of elements to return</param>
    //    /// <returns>Invoice list</returns>
    //    [HttpGet("recipients/{recipientId}")]
    //    [ProducesResponseType(200, Type = typeof(List<InvoiceDto>))]
    //    [ProducesResponseType(400)]
    //    public async Task<IActionResult> GetAllByReceiverId(string recepientId, int? quantity)
    //    {
    //        if (quantity < 0)
    //            quantity = 0;

    //        var results = await _invoiceService.GetByRecipientId(recepientId, quantity);
    //        return Ok(results);
    //    }

    //    /// <summary>
    //    /// Get the invoices from the organization ID
    //    /// </summary>
    //    /// <param name="organizationId">The id of the organization of the invoice(s)</param>
    //    /// <param name="quantity">The quantity of elements to return</param>
    //    /// <returns>Invoice list</returns>
    //    [HttpGet("organizations/{organizationId}")]
    //    [ProducesResponseType(200, Type = typeof(List<InvoiceDto>))]
    //    [ProducesResponseType(400)]
    //    public async Task<IActionResult> GetByReceiverId(string organizationId, int? quantity)
    //    {
    //        if (quantity < 0)
    //            quantity = 0;

    //        var results = await _invoiceService.GetByOrganizationId(organizationId, quantity);
    //        return Ok(results);
    //    }

    //}
}
