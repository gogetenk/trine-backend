/* 
 * Assistance.Mission.WebApi (Development) 1.0
 *
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: 1.0
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using Dto;

namespace Assistance.Operational.Dal.Repositories
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IMissionRepository
    {
        #region Asynchronous Operations

        /// <summary>
        /// Gets all the missions
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="active"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Mission&gt;</returns>
        System.Threading.Tasks.Task<List<MissionDto>> ApiMissionsActiveByActiveGetAsync(bool? active, string apiVersion = null);

        /// <summary>
        /// Deletes a mission
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">Mission id to delete</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of void</returns>
        System.Threading.Tasks.Task ApiMissionsByIdDeleteAsync(string id, string apiVersion = null);

        /// <summary>
        /// Get specific mission from Id
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">Mission Id</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of Mission</returns>
        System.Threading.Tasks.Task<MissionDto> ApiMissionsByIdGetAsync(string id, string apiVersion = null);


        /// <summary>
        /// Get all missions the specified user is involved in
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="userId">User id</param>
        /// <param name="active">Set it to true if you want to retreive only active missions</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Mission&gt;</returns>
        System.Threading.Tasks.Task<List<MissionDto>> ApiMissionsByUserIdByActiveGetAsync(string userId, bool? active, string apiVersion = null);

        /// <summary>
        /// Get mission count from orga Id
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="organizationId">The organization Id to fetch</param>
        /// <param name="isActive">Return only active missions</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of int?</returns>
        System.Threading.Tasks.Task<int?> ApiMissionsCountOrganizationsByOrganizationIdByIsActiveGetAsync(string organizationId, bool? isActive, string apiVersion = null);

        /// <summary>
        /// Get mission count from user Id
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="userId">The user Id to fetch</param>
        /// <param name="isActive">Return only active missions</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of int?</returns>
        System.Threading.Tasks.Task<int?> ApiMissionsCountUsersByUserIdByIsActiveGetAsync(string userId, bool? isActive, string apiVersion = null);

        /// <summary>
        /// Gets all missions of the specified organization.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">Organization Id</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Mission&gt;</returns>
        System.Threading.Tasks.Task<List<MissionDto>> ApiMissionsOrganizationsByIdGetAsync(string id, string apiVersion = null);

        /// <summary>
        /// Creates a mission
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="mission">Mission to create (optional)</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of Mission</returns>
        System.Threading.Tasks.Task<MissionDto> ApiMissionsPostAsync(MissionDto mission = null, string apiVersion = null);
        Task<MissionDto> GetUserCurrentMission(string userId);

        /// <summary>
        /// Updates a mission
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id"> (optional)</param>
        /// <param name="mission">Mission to Update (optional)</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of Mission</returns>
        System.Threading.Tasks.Task<MissionDto> ApiMissionsPutAsync(string id = null, MissionDto mission = null, string apiVersion = null);

        /// <summary>
        /// Gets all missions owned by the specified user.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">User Id</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Mission&gt;</returns>
        System.Threading.Tasks.Task<List<MissionDto>> ApiMissionsUsersByIdGetAsync(string id, string apiVersion = null);

        #endregion Asynchronous Operations
    }

}
