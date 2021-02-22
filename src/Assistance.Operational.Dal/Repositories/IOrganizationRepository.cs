using System.Collections.Generic;
using Assistance.Operational.Dto;
using Dto;

namespace Assistance.Operational.Dal.Repositories
{
    public interface IOrganizationRepository
    {
        /// <summary>
        /// Gets organization by its id.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of Organization</returns>
        System.Threading.Tasks.Task<OrganizationDto> ApiOrganizationsByIdGetAsync(string id, string apiVersion = null);

        /// <summary>
        /// Deletes an organization.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="organizationId"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of void</returns>
        System.Threading.Tasks.Task ApiOrganizationsByOrganizationIdDeleteAsync(string organizationId, string apiVersion = null);


        /// <summary>
        /// Gets all the invites of an organization.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="organizationId"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Invite&gt;</returns>
        System.Threading.Tasks.Task<List<InviteDto>> ApiOrganizationsByOrganizationIdInvitesGetAsync(string organizationId, string apiVersion = null);


        /// <summary>
        /// Deletes an OrganizationDto member.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="organizationId"></param>
        /// <param name="userId"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of OrganizationMember</returns>
        System.Threading.Tasks.Task<OrganizationMemberDto> ApiOrganizationsByOrganizationIdMembersByUserIdDeleteAsync(string organizationId, string userId, string apiVersion = null);

        OrganizationDto GetOrganization(string userId);

        /// <summary>
        /// Gets an OrganizationDto member.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="organizationId"></param>
        /// <param name="userId"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of OrganizationMember</returns>
        System.Threading.Tasks.Task<OrganizationMemberDto> ApiOrganizationsByOrganizationIdMembersByUserIdGetAsync(string organizationId, string userId, string apiVersion = null);


        /// <summary>
        /// Modifies an OrganizationDto member.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="organizationId"></param>
        /// <param name="userId"></param>
        /// <param name="member"> (optional)</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of OrganizationMember</returns>
        System.Threading.Tasks.Task<OrganizationMemberDto> ApiOrganizationsByOrganizationIdMembersByUserIdPatchAsync(string organizationId, string userId, OrganizationMemberDto member = null, string apiVersion = null);


        /// <summary>
        /// Gets total of OrganizationDto members
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="organizationId"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of Count</returns>
        System.Threading.Tasks.Task<int> ApiOrganizationsByOrganizationIdMembersCountGetAsync(string organizationId, string apiVersion = null);

        /// <summary>
        /// Gets all members of an organization.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="organizationId"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;OrganizationMember&gt;</returns>
        System.Threading.Tasks.Task<List<OrganizationMemberDto>> ApiOrganizationsByOrganizationIdMembersGetAsync(string organizationId, string apiVersion = null);


        /// <summary>
        /// Adds an OrganizationDto member.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="organizationId"></param>
        /// <param name="member"> (optional)</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of OrganizationMember</returns>
        System.Threading.Tasks.Task<OrganizationMemberDto> ApiOrganizationsByOrganizationIdMembersPutAsync(string organizationId, OrganizationMemberDto member = null, string apiVersion = null);

        /// <summary>
        /// Partially updates an organization.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="organizationId"></param>
        /// <param name="organization"> (optional)</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of Organization</returns>
        System.Threading.Tasks.Task<OrganizationDto> ApiOrganizationsByOrganizationIdPatchAsync(string organizationId, OrganizationDto OrganizationDto = null, string apiVersion = null);

        /// <summary>
        /// Gets all organizations (be careful using this endpoint).
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Organization&gt;</returns>
        System.Threading.Tasks.Task<List<OrganizationDto>> ApiOrganizationsGetAsync(string apiVersion = null);

        /// <summary>
        /// Gets organizations by their id.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="ids"> (optional)</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Organization&gt;</returns>
        System.Threading.Tasks.Task<List<OrganizationDto>> ApiOrganizationsManyGetAsync(List<string> ids = null, string apiVersion = null);


        /// <summary>
        /// Gets all the organizations the user is member of.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="userId"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;PartialOrganization&gt;</returns>
        System.Threading.Tasks.Task<List<PartialOrganizationDto>> ApiOrganizationsMembersByUserIdGetAsync(string userId, string apiVersion = null);


        /// <summary>
        /// Creates an organization.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="organization"> (optional)</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of Organization</returns>
        System.Threading.Tasks.Task<OrganizationDto> ApiOrganizationsPostAsync(OrganizationDto OrganizationDto = null, string apiVersion = null);


        /// <summary>
        /// Finds and replaces an organization.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id"> (optional)</param>
        /// <param name="organization"> (optional)</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of Organization</returns>
        System.Threading.Tasks.Task<OrganizationDto> ApiOrganizationsPutAsync(string id = null, OrganizationDto OrganizationDto = null, string apiVersion = null);

    }
}
