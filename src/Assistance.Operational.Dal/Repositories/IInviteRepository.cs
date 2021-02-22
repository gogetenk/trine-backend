using System;
using System.Collections.Generic;
using Dto;

namespace Assistance.Operational.Dal.Repositories
{
    public interface IInviteRepository
    {
        /// <summary>
        /// Gets invite by its code.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="code"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of Invite</returns>
        System.Threading.Tasks.Task<InviteDto> ApiInvitesByCodeGetAsync(Guid code, string apiVersion = null);

        /// <summary>
        /// Gets invite by its id.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of Invite</returns>
        System.Threading.Tasks.Task<InviteDto> ApiInvitesByIdGetAsync(string id, string apiVersion = null);

        /// <summary>
        /// Gets an invite from the guest email
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="email">The user email of the guest</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Invite&gt;</returns>
        System.Threading.Tasks.Task<List<InviteDto>> ApiInvitesEmailByEmailGetAsync(string email, string apiVersion = null);

        /// <summary>
        /// Gets all invites (be careful using this endpoint).
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Invite&gt;</returns>
        System.Threading.Tasks.Task<List<InviteDto>> ApiInvitesGetAsync(string apiVersion = null);

        /// <summary>
        /// Creates an invite.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="invite"> (optional)</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of Invite</returns>
        System.Threading.Tasks.Task<InviteDto> ApiInvitesPostAsync(InviteDto invite = null, string apiVersion = null);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="organizationId"> (optional)</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Invite&gt;</returns>
        System.Threading.Tasks.Task<List<InviteDto>> ApiInvitesSearchGetAsync(string organizationId = null, string apiVersion = null);
    }
}
