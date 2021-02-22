using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Assistance.Operational.Bll.Services;
using AutoMapper;
using Dto;
using Dto.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Assistance.Operational.WebApi.Controllers
{
    /// <summary>
    /// Controller that handles activity reports and grids.
    /// </summary>
    [Authorize]
    [Route("api/activities")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IActivityService _activityService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="activityService"></param>
        public ActivityController(ILogger<ControllerBase> logger, IMapper mapper, IActivityService activityService)
        {
            _mapper = mapper;
            _activityService = activityService;
        }

        ///// <summary>
        ///// Gets all the activities of a mission.
        ///// </summary>
        ///// <param name="missionId"></param>
        ///// <returns></returns>
        //[HttpGet("missions/{missionId}")]
        //[ProducesResponseType(200, Type = typeof(List<ActivityDto>))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> GetActivityFromMission(string missionId)
        //{
        //    var userId = GetCurrentUserId();
        //    List<ActivityDto> dto = await _activityService.GetFromMission(userId, missionId);
        //    return Ok(dto);
        //}

        /// <summary>
        /// Gets an activity by id.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{activityId}")]
        [ProducesResponseType(200, Type = typeof(ActivityDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(string activityId)
        {
            var userId = GetCurrentUserId();
            ActivityDto dto = await _activityService.GetById(userId, activityId);
            return Ok(dto);
        }

        /// <summary>
        /// Gets an activity from an invitation token. No authentication needed.
        /// </summary>
        /// <returns>The requested activity.</returns>
        [AllowAnonymous]
        [HttpGet("token/{token}")]
        [ProducesResponseType(200, Type = typeof(ActivityDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByToken(string token)
        {
            ActivityDto dto = await _activityService.GetByToken(token);
            return Ok(dto);
        }

        [HttpPost("{activityId}/request-change")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RefuseActivity(string id, string userId, List<GridDayDto> modifications = null, string comment = "")
        {
            await _activityService.RefuseWithModificationProposal(id, userId, modifications, comment);
            return Ok();
        }

        ////[HttpPost]
        ////[ProducesResponseType(200, Type = typeof(ActivityDto))]
        ////[ProducesResponseType(400)]
        ////[ProducesResponseType(404)]
        ////public async Task<IActionResult> CreateActivityFromMissionAndPeriod([FromQuery]string missionId, [FromQuery]DateTime period)
        ////{
        ////    var activity = await _activityService.CreateActivityFromMissionAndPeriod(missionId, period);
        ////    return Ok(activity);
        ////}

        /// <summary>
        /// Creates a new activity for the specified period and current user.
        /// </summary>
        /// <param name="period">The period to generate the activity for</param>
        /// <returns>An activity dto</returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(ActivityDto))]
        public async Task<IActionResult> CreateActivityFromPeriod([FromBody] CreateActivityRequestDto period)
        {
            var activity = await _activityService.CreateActivityFromPeriod(GetCurrentUserId(), period.Date);
            return Created("", activity);
        }

        /// <summary>
        /// Shares an activity report to the specified user email (sends an invitation email).
        /// </summary>
        /// <param name="activityId">The activity report identifier.</param>
        /// <param name="request">The customer email, he might not be a Trine user yet.</param>
        /// <returns>The updated activity report</returns>
        [HttpPatch("{activityId}/share")]
        [ProducesResponseType(200, Type = typeof(ActivityDto))]
        public async Task<IActionResult> ShareActivityWithEmail([FromRoute] string activityId, [FromBody] ShareActivityRequestDto request)
        {
            var activity = await _activityService.ShareActivityWithEmail(GetCurrentUserId(), activityId, request.CustomerEmail);
            return Ok(activity);
        }

        /// <summary>
        /// Accepts activity report modifications
        /// </summary>
        /// <returns>The activity.</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="userId">User identifier.</param>
        [HttpPost("{id}/validate-change")]
        public async Task<IActionResult> AcceptActivityModificationProposal(string id, string userId)
        {
            await _activityService.AcceptLastModificationProposal(id, userId);
            return Ok();
        }

        ///// <summary>
        ///// Gets all the activities an user is involved in.
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //[HttpGet("users/{userId}")]
        //[ProducesResponseType(200, Type = typeof(List<ActivityDto>))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> GetActivitiesFromUser(string userId)
        //{
        //    var dto = await _activityService.GetFromUser(userId);
        //    return Ok(dto);
        //}

        ///// <summary>
        ///// Gets all the activities of the current authenticated user.
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //[HttpGet("users/me")]
        //[ProducesResponseType(200, Type = typeof(List<ActivityDto>))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> GetActivitiesFromUser()
        //{
        //    var id = GetCurrentUserId();
        //    var dto = await _activityService.GetFromUser(id);
        //    return Ok(dto);
        //}

        /// <summary>
        /// Gets all the activities an user is involved in.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<ActivityDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetActivities()
        {
            var userId = GetCurrentUserId();
            var dto = await _activityService.GetFromUser(userId);
            return Ok(dto);
        }

        /// <summary>
        /// Gets all the days of the current month, with a flag specifying if the day is workable.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("generate")]
        [ProducesResponseType(200, Type = typeof(ActivityDto))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GenerateGrid()
        {
            ActivityDto dto = await _activityService.Generate();
            return Ok(dto);
        }


        /// <summary>
        /// Generate excel file
        /// </summary>
        /// <param name="id">Activity id</param>
        [AllowAnonymous]
        [HttpGet("{id}/export")]
        [ProducesResponseType(200, Type = typeof(FileContentResult))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ExportActivity(string id)
        {
            byte[] fileContents = await _activityService.ExportActivity(id);

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: $"rapport-activite-{id}.xlsx"
            );
        }

        ///// <summary>
        ///// Gets the mission activity from a given month
        ///// </summary>
        //[HttpGet("missions/{missionId}/{date}")]
        //[ProducesResponseType(200, Type = typeof(ActivityDto))]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> GetActivityFromMissionAndMonth(string missionId, DateTime date)
        //{
        //    string userId = GetCurrentUserId();
        //    var dto = await _activityService.GetFromMissionAndMonth(userId, missionId, date);
        //    if (dto is null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(dto);
        //}

        /// <summary>
        /// Triggers the activity notifications sender at the end the month (called by a web job). Do not use this endpoint by yourself.
        /// </summary>
        [HttpGet("trigger")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SendReports()
        {
            await _activityService.CreateFillActivityNotifications();
            return Ok();
        }

        //[HttpPut("signature")]
        //public async Task<IActionResult> SignActivity([FromBody]SignatureActivityDto signature)
        //{
        //    string userId = GetCurrentUserId();
        //    if (!string.IsNullOrEmpty(signature.SignatureUri))
        //    {
        //        var activity = await _activityService.SignActivityReport(signature.ActivityId, userId,
        //            extension: "xlsx",
        //            contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", signatureUri: signature.SignatureUri);
        //        return Ok(activity);
        //    }
        //    return BadRequest();
        //}

        /// <summary>
        /// Signs an activity report and returns back the updated object.
        /// </summary>
        /// <param name="activityId">The activity report to sign</param>
        [HttpPost("{activityId}/sign")]
        [ProducesResponseType(200, Type = typeof(ActivityDto))]
        public async Task<IActionResult> SignActivity([FromRoute] string activityId)
        {
            string userId = GetCurrentUserId();
            var activity = await _activityService.SignActivityReport(activityId, userId);
            return Ok(activity);
        }

        /// <summary>
        /// [Consultant onboarding] Saves a new activity report as "signed by consultant" and returns back the object.
        /// </summary>
        /// <param name="activity">The activity report to save and sign.</param>
        [HttpPost("first")]
        [ProducesResponseType(201, Type = typeof(ActivityDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateAndSignActivity([FromBody] ActivityDto activity)
        {
            var updatedActivity = await _activityService.CreateAndSignActivity(activity, GetCurrentUserId());
            return Created("", updatedActivity);
        }

        /// <summary>
        /// Upates an activity
        /// </summary>
        [HttpPut]
        [ProducesResponseType(200, Type = typeof(ActivityDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateActivityReport(string activityId, ActivityDto activity)
        {
            if (string.IsNullOrWhiteSpace(activityId) || activity is null)
                return BadRequest("Mandatory parameter(s) missing.");

            var userId = GetCurrentUserId();
            var newActivity = await _activityService.UpdateActivity(userId, activityId, activity);
            if (newActivity == null)
                return BadRequest("An error occured");
            else
                return Ok(newActivity);
        }

        /// <summary>
        /// Deletes an activity report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _activityService.DeleteActivity(id);
            return Ok();
        }

        private string GetCurrentUserId()
        {
            var id = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "https://trineapp.io/trine_id")?.Value
                ?? throw new AuthenticationException("The Trine id couldn't be found.");
            return id;
        }
    }
}
