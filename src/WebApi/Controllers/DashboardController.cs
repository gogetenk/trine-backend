using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Assistance.Operational.Bll.Services;
using AutoMapper;
using Dto.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Assistance.Operational.WebApi.Controllers
{
    /// <summary>
    /// Manages the user dashboard
    /// </summary>
    [Authorize]
    [Route("api/dashboards")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="dashboardService"></param>
        public DashboardController(ILogger<ControllerBase> logger, IMapper mapper, IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        ///// <summary>
        ///// Gets the organizations the user is a member of.
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("organizations/users/{userId}")]
        //[ProducesResponseType(200, Type = typeof(List<PartialOrganizationDto>))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> GetOrganizationsFromMemberId(string userId)
        //{
        //    if (string.IsNullOrEmpty(userId))
        //        return BadRequest();

        //    var orgas = await _dashboardService.GetOrganizationsFromMemberId(userId);
        //    if (orgas == null)
        //        return Ok(new List<PartialOrganizationDto>());

        //    return Ok(orgas);
        //}

        //[HttpGet("sales")]
        //[ProducesResponseType(200, Type = typeof(List<SalesDashboardDto>))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> GetSalesDashboard()
        //{
        //    string userId = GetCurrentUserId();
        //    var data = await _dashboardService.GetSalesDashboard(userId);
        //    return Ok(data);
        //}

        ////Todo: Do that in DB so its faster
        ///// <summary>
        ///// Gets the organization count the user is a member of.
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("organizations/users/{userId}/count")]
        //[ProducesResponseType(200, Type = typeof(int))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> GetOrganizationCountFromMemberId(string userId)
        //{
        //    if (string.IsNullOrEmpty(userId))
        //        return BadRequest();

        //    var orgas = await _dashboardService.GetOrganizationsFromMemberId(userId);
        //    if (orgas == null)
        //        return Ok(0);

        //    return Ok(orgas.Count);
        //}

        ///// <summary>
        ///// Gets the 5 last missions the specified user is involved in.
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("missions/users/{userId}")]
        //[ProducesResponseType(200, Type = typeof(List<DashboardMissionDto>))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> GetLatestMissions(string userId)
        //{
        //    if (string.IsNullOrEmpty(userId))
        //        return BadRequest();

        //    var missions = await _dashboardService.GetLatestMissionsFromUser(userId, true);
        //    if (missions == null)
        //        return Ok(new List<DashboardMissionDto>());

        //    return Ok(missions);
        //}

        ///// <summary>
        ///// Counts the missions where the specified user is involved in.
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("missions/users/{userId}/count")]
        //[ProducesResponseType(200, Type = typeof(int))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> GetMissionCount(string userId, bool isActive)
        //{
        //    int count = await _dashboardService.GetMissionCountFromUser(userId, isActive);
        //    return Ok(count);
        //}

        /////// <summary>
        /////// Gets the 5 last customers of the specified user.
        /////// </summary>
        /////// <returns></returns>
        ////[HttpGet("customers/users/{id}")]
        ////[ProducesResponseType(200, Type = typeof(List<DashboardCustomerDto>))]
        ////[ProducesResponseType(400)]
        ////[ProducesResponseType(404)]
        ////public async Task<IActionResult> GetLatestCustomers(string id)
        ////{
        ////    if (string.IsNullOrEmpty(id))
        ////        return BadRequest();

        ////    var customers = await _dashboardService.GetLatestCustomersFromUser(id, true);

        ////    return Ok(customers);
        ////}

        ///// <summary>
        ///// Gets list counts for Missions, Clients and Events
        ///// </summary>
        ///// <returns>DashboardCountDto</returns>
        //[HttpGet("{userId}")]
        //[ProducesResponseType(200, Type = typeof(DashboardCountDto))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> GetDashboardCounts(string userId)
        //{
        //    DashboardCountDto dto = await _dashboardService.GetCountsFromUser(userId);
        //    return Ok(dto);
        //}

        ///// <summary>
        ///// Gets mission count of an organization
        ///// </summary>
        ///// <returns>OrganizationDto</returns>
        //[HttpGet("missions/count/{organizationId}")]
        //[ProducesResponseType(200, Type = typeof(int))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> GetOrganizationMissionCount(string organizationId)
        //{
        //    var count = await _dashboardService.GetOrganizationMissionCount(organizationId);
        //    return Ok(count);
        //}

        ///// <summary>
        ///// Gets all events from user
        ///// </summary>
        //[HttpGet("events")]
        //[ProducesResponseType(200, Type = typeof(List<EventDto>))]
        //[ProducesResponseType(400)]
        //public async Task<IActionResult> GetEventsByUserId(string userId)
        //{
        //    if (string.IsNullOrEmpty(userId))
        //        throw new ArgumentException("Parameter cannot be null", nameof(userId));

        //    var dtos = await _dashboardService.GetEventsFromUser(userId);
        //    return Ok(dtos);
        //}

        /// <summary>
        /// Gets all the data needed to show the dashboard of the current authenticated user
        /// </summary>
        /// <returns>Data to show on the dashboard (indicators, banners, activities)</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(DashboardResponseDto))]
        public async Task<IActionResult> GetDashboardData()
        {
            var data = await _dashboardService.GetDashboardData(GetCurrentUserId());
            return Ok(data);
        }

        private string GetCurrentUserId()
        {
            var id = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "https://trineapp.io/trine_id")?.Value
                ?? throw new AuthenticationException("The Trine id couldn't be found.");
            return id;
        }
    }
}
