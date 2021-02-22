using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dto;

namespace Assistance.Operational.Dal.Repositories
{
    public interface IActivityRepository
    {
        /// <summary>
        /// Deletes an activity report
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of void</returns>
        System.Threading.Tasks.Task ApiActivitiesByIdDeleteAsync(string id, string apiVersion = null);

        /// <summary>
        /// Gets an activity by its id
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of Activity</returns>
        System.Threading.Tasks.Task<ActivityDto> ApiActivitiesByIdGetAsync(string id, string apiVersion = null);

        /// <summary>
        /// Updates an activity (change worked days, add signature etc...)
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">ActivityDto identifier</param>
        /// <param name="activity">ActivityDto (optional)</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of Activity</returns>
        System.Threading.Tasks.Task<ActivityDto> ApiActivitiesByIdPatchAsync(string id, ActivityDto activity = null, string apiVersion = null);

        /// <summary>
        /// Generates an fillable activity report between start and end dates.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of Activity</returns>
        System.Threading.Tasks.Task<ActivityDto> ApiActivitiesByStartDateendDateGetAsync(DateTime startDate, DateTime endDate, string apiVersion = null);

        /// <summary>
        /// Gets all the activities during this period
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="startDate"> (optional)</param>
        /// <param name="endDate"> (optional)</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Activity&gt;</returns>
        System.Threading.Tasks.Task<List<ActivityDto>> ApiActivitiesDatesGetAsync(DateTime startDate, DateTime endDate, string apiVersion = null);

        /// <summary>
        /// Gets all the activities (dangerous, keep this unexposed)
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Activity&gt;</returns>
        System.Threading.Tasks.Task<List<ActivityDto>> ApiActivitiesGetAsync(string apiVersion = null);

        /// <summary>
        /// Gets the mission activity from a given month
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="missionId"></param>
        /// <param name="date"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of Activity</returns>
        System.Threading.Tasks.Task<ActivityDto> ApiActivitiesMissionsByMissionIdByDateGetAsync(string missionId, DateTime date, string apiVersion = null);

        /// <summary>
        /// Gets all the activities of a mission
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="missionId"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Activity&gt;</returns>
        System.Threading.Tasks.Task<List<ActivityDto>> ApiActivitiesMissionsByMissionIdGetAsync(string missionId, string apiVersion = null);

        /// <summary>
        /// Gets all the activities of several missions
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="missionIds"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Activity&gt;</returns>
        System.Threading.Tasks.Task<List<ActivityDto>> ApiActivitiesMissionsByMissionIdsGetAsync(List<string> missionIds, string apiVersion = null);

        /// <summary>
        /// Persists an filled activity report ready for signature
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="activity"> (optional)</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of Activity</returns>
        System.Threading.Tasks.Task<ActivityDto> ApiActivitiesPostAsync(ActivityDto activity = null, string apiVersion = null);

        /// <summary>
        /// Gets all the activities of an user
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="userId"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Activity&gt;</returns>
        System.Threading.Tasks.Task<List<ActivityDto>> ApiActivitiesUsersByUserIdGetAsync(string userId, string apiVersion = null);

        /// <summary>
        /// Gets all the activities of several users depending on a date
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="userIds"></param>
        /// <param name="date"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Activity&gt;</returns>
        System.Threading.Tasks.Task<List<ActivityDto>> ApiActivitiesUsersByUserIdsByDateGetAsync(List<string> userIds, DateTime date, string apiVersion = null);

        /// <summary>
        /// Gets all the activities of several users
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="userIds"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Activity&gt;</returns>
        System.Threading.Tasks.Task<List<ActivityDto>> ApiActivitiesUsersByUserIdsGetAsync(List<string> userIds, string apiVersion = null);

        /// <summary>
        /// Gets a list of activities from an user email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<List<ActivityDto>> GetByEmail(string email);

    }
}
