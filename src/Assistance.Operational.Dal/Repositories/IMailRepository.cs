using System.Threading.Tasks;
using Assistance.Operational.Model;

namespace Assistance.Operational.Dal.Repositories
{
    public interface IMailRepository
    {
        /// <summary>
        /// Sends an email to the specified user, using the specified event informations
        /// </summary>
        /// <param name="receiver">The user to send the email to.</param>
        /// <returns></returns>
        Task<bool> SendAsync(string mail, string templateId, object templateData = null, string subject = "", string content = "", MailAttachment attachment = null);
    }
}
