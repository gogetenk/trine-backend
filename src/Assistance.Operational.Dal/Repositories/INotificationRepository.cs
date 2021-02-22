using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assistance.Operational.Dal.Repositories
{
    public interface INotificationRepository
    {
        Task SendTemplatedNotification(Dictionary<string, string> Content, List<string> userIds);
    }
}
