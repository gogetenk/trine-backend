using System.Collections.Generic;
using System.Threading.Tasks;
using Dto;

namespace Assistance.Operational.Bll.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> SearchUsers(string email = null, string firstname = null, string lastname = null, string companyName = null);
        Task<List<UserDto>> GetAllUsers();
        Task<List<UserAdminDto>> GetAllDataUsers();
        Task<TokenDto> RequestToken(string userId);
        Task DeleteUser(string id);
        Task UpdateUserAsync(string id, UserDto user);
        Task UpdateRequiredData(string id, string password, string phoneNumber);
        Task<bool> UpdatePassword(string id, string newPassword);
        Task<string> CreateUser(ExternalUserDto dto);
    }
}