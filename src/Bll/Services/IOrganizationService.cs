using System.Collections.Generic;
using System.Threading.Tasks;
using Assistance.Operational.Dto;
using Dto;

namespace Assistance.Operational.Bll.Services
{
    public interface IOrganizationService
    {
        Task<OrganizationDto> Insert(OrganizationDto organization);
        Task<OrganizationDto> GetById(string id);
        OrganizationDto GetByUserId(string userId);
        Task<List<OrganizationDto>> GetByIds(List<string> ids);
        Task<OrganizationDto> Update(string id, OrganizationDto organization);
        Task Delete(string id);
        Task<OrganizationDto> PartialUpdate(string organizationId, OrganizationDto organization);
        Task<OrganizationMemberDto> UpdateMember(string organizationId, OrganizationMemberDto member);
        Task<OrganizationMemberDto> GetMember(string organizationId, string userId);
        Task<List<UserDto>> GetAllMembers(string organizationId);
        Task RemoveMember(string organizationId, string userId);
    }
}
