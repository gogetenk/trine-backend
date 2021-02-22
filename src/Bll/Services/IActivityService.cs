using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dto;

namespace Assistance.Operational.Bll.Services
{
    public interface IActivityService
    {
        Task<ActivityDto> Generate();
        Task<List<ActivityDto>> GetFromMission(string userId, string missionId);
        Task<List<ActivityDto>> GetFromUser(string userId);
        Task<ActivityDto> SignActivityReport(string activityId, string userId, string extension = "", string contentType = "", byte[] signature = null, string signatureUri = "");
        Task<ActivityDto> CreateActivityFromMissionAndPeriod(string missionId, DateTime period);
        Task DeleteActivity(string id);
        //Task<ActivityDto> Create(CreateActivityRequestDto dto);
        Task CreateFillActivityNotifications();
        Task<ActivityDto> GetById(string userId, string id);
        Task<ActivityDto> GetFromMissionAndMonth(string userId, string missionId, DateTime date);
        Task<ActivityDto> UpdateActivity(string userId, string activityId, ActivityDto activity);
        Task<byte[]> ExportActivity(string id);
        Task RefuseWithModificationProposal(string activityId, string userId, List<GridDayDto> modifications, string comment = "");
        Task AcceptLastModificationProposal(string activityId, string userId);
        Task<ActivityDto> CreateActivityFromEmailAndPeriod(string currentUserId, string customerEmail, DateTime period);
        Task<ActivityDto> CreateActivityFromPeriod(string currentUserId, DateTime period);
        Task<ActivityDto> ShareActivityWithEmail(string currentUserId, string activityId, string customerEmail);
        Task<ActivityDto> CreateAndSignActivity(ActivityDto activity, string currentUserId);
        Task<ActivityDto> GetByToken(string token);
    }
}
