using System.Collections.Generic;
using System.Threading.Tasks;
using Dto;

namespace Assistance.Operational.Bll.Services
{
    public interface IInvoiceService
    {
        Task<InvoiceDto> GetById(string missionId);
        Task<List<InvoiceDto>> GetByMissionId(string missionId, int? quantity);
        Task<List<InvoiceDto>> GetByOrganizationId(string missionId, int? quantity);
        Task<List<InvoiceDto>> GetByRecipientId(string recepientId, int? quantity);
        Task<List<InvoiceDto>> GetByIssuerId(string issuerId, int? quantity);
        Task<List<InvoiceDto>> GetByMissionIds(string[] ids);
    }
}
