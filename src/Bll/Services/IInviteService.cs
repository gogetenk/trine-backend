using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dto;

namespace Assistance.Operational.Bll.Services
{
    public interface IInviteService
    {
        Task<InviteResponseDto> Get(Guid code);
        Task<AcceptInvitationResultDto> AcceptInvite(string code);
        Task<List<InviteDto>> GetByOrganizationId(string organizationId);
        Task<InviteDto> CreateInvite(string mail, string inviterId, string organizationId, string missionId = "");
        Task<InviteDto[]> CreateBulkInvite(string[] mails, string inviterId, string organizationId, string missionId = "");
        Task Delete(string id);
        Task<List<InviteDto>> GetUserInvitesAsync(string id);
    }
}
