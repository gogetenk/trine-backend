using System.Collections.Generic;
using Dto;

namespace Assistance.Operational.Dal.Repositories
{
    public interface IEventRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of void</returns>
        System.Threading.Tasks.Task ApiEventsByIdDeleteAsync(string id, string apiVersion = null);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of void</returns>
        System.Threading.Tasks.Task<EventDto> ApiEventsByIdGetAsync(string id, string apiVersion = null);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id"></param>
        /// <param name="value"> (optional)</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of void</returns>
        System.Threading.Tasks.Task<EventDto> ApiEventsByIdPutAsync(string id, EventDto value, string apiVersion = null);

        /// <summary>
        /// Get event count from contextId
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contextId">The contextId to fetch</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of int?</returns>
        System.Threading.Tasks.Task<int> ApiEventsContextsByContextIdCountGetAsync(string contextId, string apiVersion = null);

        /// <summary>
        /// Get events from contextId
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contextId">The context Id to fetch</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Event&gt;</returns>
        System.Threading.Tasks.Task<List<EventDto>> ApiEventsContextsByContextIdGetAsync(string contextId, string apiVersion = null);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Event&gt;</returns>
        System.Threading.Tasks.Task<List<EventDto>> ApiEventsGetAsync(string apiVersion = null);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="value"> (optional)</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of void</returns>
        System.Threading.Tasks.Task<EventDto> ApiEventsPostAsync(EventDto value, string apiVersion = null);

        /// <summary>
        /// Get event count from target id
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="targetId">The contextId to fetch</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of int?</returns>
        System.Threading.Tasks.Task<int> ApiEventsTargetsByTargetIdCountGetAsync(string targetId, string apiVersion = null);

        /// <summary>
        /// Get events from target id
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="targetId"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;Event&gt;</returns>
        System.Threading.Tasks.Task<List<EventDto>> ApiEventsTargetsByTargetIdGetAsync(string targetId, string apiVersion = null);
    }
}
