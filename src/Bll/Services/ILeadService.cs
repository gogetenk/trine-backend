using System.Threading.Tasks;

namespace Assistance.Operational.Bll.Services
{
    public interface ILeadService
    {
        Task<bool> CreateLead(string email);
    }
}
