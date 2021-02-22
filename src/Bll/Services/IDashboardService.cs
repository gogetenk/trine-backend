using System.Collections.Generic;
using System.Threading.Tasks;
using Dto;
using Dto.Responses;

namespace Assistance.Operational.Bll.Services
{
    public interface IDashboardService
    {
        Task<List<DashboardMissionDto>> GetLatestMissionsFromUser(string userId, bool isActive);
        Task<DashboardCountDto> GetCountsFromUser(string userId);
        Task<List<EventDto>> GetEventsFromUser(string userId);
        Task<List<PartialOrganizationDto>> GetOrganizationsFromMemberId(string id);
        Task<int> GetOrganizationMissionCount(string id);
        Task<int> GetMissionCountFromUser(string userId, bool isActive);
        Task<SalesDashboardDto> GetSalesDashboard(string userId);
        Task<DashboardResponseDto> GetDashboardData(string userId);
    }
}
