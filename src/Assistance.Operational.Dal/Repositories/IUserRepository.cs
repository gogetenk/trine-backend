using System.Collections.Generic;
using System.Threading.Tasks;
using Dto;

namespace Assistance.Operational.Dal.Repositories
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IUserRepository
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
        System.Threading.Tasks.Task ApiUsersByIdDeleteAsync(string id, string apiVersion = null);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id"></param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of User</returns>
        System.Threading.Tasks.Task<UserDto> ApiUsersByIdGetAsync(string id, string apiVersion = null);
        /// <summary>
        /// Gets all users and their companies
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;UserAggregate&gt;</returns>
        System.Threading.Tasks.Task<List<UserDto>> ApiUsersGetAsync(string apiVersion = null);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="ids"> (optional)</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;User&gt;</returns>
        System.Threading.Tasks.Task<List<UserDto>> ApiUsersIdsGetAsync(List<string> ids = null, string apiVersion = null);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="user"> (optional)</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of string</returns>
        System.Threading.Tasks.Task<UserDto> ApiUsersPostAsync(UserDto user = null, string apiVersion = null);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id"> (optional)</param>
        /// <param name="user"> (optional)</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of void</returns>
        System.Threading.Tasks.Task ApiUsersPutAsync(string id = null, UserDto user = null, string apiVersion = null);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="Assistance.Operational.Dal.SwaggerImpl.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="email"> (optional)</param>
        /// <param name="firstname"> (optional)</param>
        /// <param name="lastname"> (optional)</param>
        /// <param name="apiVersion">The requested API version (optional, default to 1.0)</param>
        /// <returns>Task of List&lt;User&gt;</returns>
        System.Threading.Tasks.Task<List<UserDto>> ApiUsersSearchGetAsync(string email = null, string firstname = null, string lastname = null, string apiVersion = null);
        Task<UserDto> GetByEmailAndPassword(string mail, string password);
        Task<UserDto> GetByEmail(string mail);
    }
}
