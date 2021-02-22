using System.Threading.Tasks;

namespace Assistance.Event.Dal.Repositories
{
    /// <summary>
    /// Used for internal backoffice communication channels
    /// </summary>
    public interface IInternalNotificationRepository
    {
        Task SendAsync(string message);
    }
}
