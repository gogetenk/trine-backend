using System.Collections.Generic;
using System.Threading.Tasks;
using Dto;

namespace Assistance.Operational.Bll.Services
{
    public interface IMissionService
    {
        Task<List<MissionDto>> GetMissionsFromUser(string userId, bool isActive);
        Task<MissionDto> CreateMission(CreateMissionRequestDto request);
        Task<MissionDto> UpdateMission(string id, MissionDto dto);
        Task UpdateConsultant(string missionId, string userId);
        Task UpdateCustomer(string id, string missionId, MissionCustomerDto dto);
        Task<MissionDto> GetMissionAggregate(string id);
        Task<MissionDto> CancelMission(string id);
        Task<FrameContractDto> GetContractPreview(CreateMissionRequestDto request);
        Task<MissionDto> GetMissionById(string id);
        Task<List<MissionDto>> GetFromOrganizationId(string id);
        Task<List<MissionDto>> GetFromOwnerId(string userId);
        Task DeleteMission(string id);
        Task<MissionDto> GetUserCurrentMission(string userId);
    }
}
